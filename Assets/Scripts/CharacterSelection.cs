using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    private GameObject selectedCharacter;
    public GameObject conePrefab;  // Assign the cone prefab in the Inspector
    private GameObject activeCone; // Keeps track of the active cone
    public GameObject moveIndicatorPrefab;  // Prefab for the move indicator
    private GameObject activeMoveIndicator; // Keeps track of the move indicator
    public LayerMask selectableLayer; // Set this to detect only characters
    public LayerMask groundLayer;  // Layer for detecting the ground
    public LayerMask treasureLayer;  // Layer for detecting treasure
    public LayerMask obstacleLayer;  // Layer for obstacles

    void Update()
    {
        HandleCharacterSelection();

        // If a character is selected, handle click-based movement
        if (selectedCharacter != null && Input.GetMouseButtonDown(0))
        {
            MoveSelectedCharacterOrTreasure();
        }
    }
    MovePosEventArgs movePosEventArgs = new MovePosEventArgs();
    void HandleCharacterSelection()
    {
        // Left-click to select a character
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            #region
            
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit2;
            
            bool res = Physics.Raycast(ray, out hit2);

           
            if (res == true)
            {
                // send location
                transform.position = hit2.point;
                if (hit2.collider.gameObject.name == "Treasure")
                {
                    Debug.Log("location£º" + hit2.point + "name£º" + hit2.collider.gameObject.name);
                    movePosEventArgs.movePos = hit2.point;
                    //this.TriggerEvent(DefaultEventName.MovePosEvent, movePosEventArgs);
                }
                // 
            }
            #endregion

            // Check if the player clicked on a character
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {

                GameObject clickedObject = hit.collider.gameObject;

                Debug.Log("ray cast hit£º" + clickedObject.name);
                // Reset obstacles when selecting a new character
                ResetObstacles();

                // Deselect the previous character (if any)
                if (selectedCharacter != null)
                {
                    DeselectCharacter();
                }

                // Select the new character
                SelectCharacter(clickedObject);
            }
        }

        // Right-click to deselect the current character
        if (Input.GetMouseButtonDown(1) && selectedCharacter != null)
        {
            DeselectCharacter();
        }
    }

    void SelectCharacter(GameObject character)
    {
        selectedCharacter = character;

        // Instantiate the cone above the character
        Vector3 conePosition = selectedCharacter.transform.position + new Vector3(0, 2, 0); // 2 units above
        activeCone = Instantiate(conePrefab, conePosition, Quaternion.identity);
        activeCone.transform.SetParent(selectedCharacter.transform);  // Make sure the cone follows the character
    }

    void DeselectCharacter()
    {
        // Destroy the active cone when deselecting
        if (activeCone != null)
        {
            Destroy(activeCone);
        }

        // Destroy the move indicator as well
        if (activeMoveIndicator != null)
        {
            Destroy(activeMoveIndicator);
        }

        selectedCharacter = null;
    }
    // Move the selected character to the clicked ground position or treasure
    void MoveSelectedCharacterOrTreasure()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Check if the player clicked on a treasure
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, treasureLayer))
        {
            TreasureMovement treasure = hit.collider.GetComponent<TreasureMovement>();
            if (treasure != null)
            {
                // Case 1: Treasure is active and can move
                if (treasure.isActive)
                {
                    if (selectedCharacter == null)
                    {
                        // No character is selected, so allow treasure movement
                        Vector3 targetPosition = hit.point;  // Get the position of the click
                        treasure.MoveToTarget(targetPosition);  // Move the treasure

                        // Move the indicator to the clicked position above the treasure
                        Vector3 treasureIndicatorPosition = targetPosition + new Vector3(0, 2, 0);  // 2 units above the treasure

                        if (activeMoveIndicator != null)
                        {
                            Destroy(activeMoveIndicator);  // Destroy the previous indicator
                        }

                        // Instantiate a new indicator at the clicked position
                        activeMoveIndicator = Instantiate(moveIndicatorPrefab, treasureIndicatorPosition, Quaternion.identity);
                    }
                    else
                    {
                        // Case 2: A character is selected, and I want the character to move toward the treasure
                        CharacterMovement characterMovement = selectedCharacter.GetComponent<CharacterMovement>();
                        if (characterMovement != null)
                        {
                            // Move the selected character to the treasure's position
                            characterMovement.SetTargetPosition(treasure.transform.position);  // Move character to treasure

                            // Move the indicator to the treasure's position
                            Vector3 treasureIndicatorPosition = treasure.transform.position + new Vector3(0, 2, 0);  // 2 units above the treasure

                            if (activeMoveIndicator != null)
                            {
                                Destroy(activeMoveIndicator);  // Destroy the previous indicator
                            }

                            // Instantiate a new indicator above the treasure
                            activeMoveIndicator = Instantiate(moveIndicatorPrefab, treasureIndicatorPosition, Quaternion.identity);
                        }
                    }
                }
            }
        }
        // Otherwise, check if the player clicked on the ground
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer) && selectedCharacter != null)
        {
            CharacterMovement characterMovement = selectedCharacter.GetComponent<CharacterMovement>();

            if (!characterMovement.Movable)
            {
                return;
            }
            if (characterMovement != null)
            {
                // Move the selected character to the clicked ground position

                characterMovement.SetTargetPosition(hit.point);

                // Move the indicator to the ground point
                if (moveIndicatorPrefab != null)
                {
                    if (activeMoveIndicator != null)
                    {
                        Destroy(activeMoveIndicator);  // Destroy the previous indicator
                    }
                    activeMoveIndicator = Instantiate(moveIndicatorPrefab, hit.point, Quaternion.identity);
                }
            }
        }
    }



    // Reset obstacles when selecting a new character
    void ResetObstacles()
    {
        Collider[] obstacles = Physics.OverlapSphere(Vector3.zero, 100f, obstacleLayer);
        foreach (Collider collider in obstacles)
        {
            Obstacle obstacle = collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.SetObstacleActive(true);  // Reactivate all obstacles
            }
        }
    }
}
