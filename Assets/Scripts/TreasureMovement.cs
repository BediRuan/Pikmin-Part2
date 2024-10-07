using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TreasureMovement : MonoBehaviour
{
    public int requiredCharacters = 3;  // Minimum characters needed to activate the treasure
    private List<CharacterMovement> attachedCharacters = new List<CharacterMovement>();  // List of characters within range
    private NavMeshAgent navMeshAgent;
    public bool isActive = false;  // Whether the treasure is active and selectable

    public int NumberAttached;
    void Start()
    {
        EventMgr.Instance.AddListener(DefaultEventName.MovePosEvent, OnMovePos);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;  // Treasure can't move initially

        NumberAttached = 0;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (NumberAttached >= 3)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
              
                RaycastHit hit2;
                
                bool res = Physics.Raycast(ray, out hit2);

                navMeshAgent.SetDestination(hit2.point);
            }
        }
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener(DefaultEventName.MovePosEvent, OnMovePos);
    }

    private void OnMovePos(object sender, EventArgs e)
    {
        if(e is MovePosEventArgs data)
        {
            if (transform.childCount > 0)
            {

                MoveToTarget(data.movePos);
                Debug.Log(name + "clicked, move to treasure");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterMovement character = other.GetComponent<CharacterMovement>();
        if (character != null && !attachedCharacters.Contains(character))
        {
            Debug.Log("collided" + other.name);

            if(other.tag == "KeYiDong")
            {
                NumberAttached++;
                if (NumberAttached >= 3)
                {
                    navMeshAgent.enabled = true;  // Treasure can't move initially
                }
                other.GetComponent<CharacterMovement>().Movable = false;
                other.GetComponent<NavMeshAgent>().enabled = false;
                other.transform.parent = transform;
                other.transform.DestroyAllChildren();

              //  other.transform.position = new Vector3(0,0,0);
            }
            attachedCharacters.Add(character);  // Add character to the list
            character.EnterCarryState();

            CheckIfCanActivate();
            other.transform.localScale *= 1.5f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        CharacterMovement character = other.GetComponent<CharacterMovement>();
        if (character != null && attachedCharacters.Contains(character))
        {
            attachedCharacters.Remove(character);  // Remove character from the list
            CheckIfCanActivate();
        }
    }

    // Method to check if enough characters are within the range to activate the treasure
    private void CheckIfCanActivate()
    {
        //if (attachedCharacters.Count >= requiredCharacters)
        //{
        //    isActive = true;  // Activate the treasure
        //    navMeshAgent.enabled = true;  // Enable NavMeshAgent for movement
        //}
        //else
        //{
        //    isActive = false;
        //    navMeshAgent.enabled = false;  // Disable movement if not enough characters
        //}
    }

    // Method to move the treasure
    public void MoveToTarget(Vector3 targetPosition)
    {
        if (isActive && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(targetPosition);  // Move treasure to the target position
        }
    }
}

