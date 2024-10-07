using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum CharacterState
{
    Normal,
    Carry
}
public class CharacterMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public GameObject moveIndicatorPrefab;
    private GameObject activeMoveIndicator;
    private NavMeshAgent treasureNavMeshAgent;  // Reference to the treasure's NavMeshAgent
    public CharacterState currentState = CharacterState.Normal;
    public string characterType;  // character types: "Fire", "Water", "Electric"
    public LayerMask obstacleLayer;  // check layer
    private List<Obstacle> disabledObstacles = new List<Obstacle>();

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Movable = true;
    }

    public bool Movable;

    void Update()
    {
        // If the treasure is moving, the character should follow the treasure
        if (treasureNavMeshAgent != null && treasureNavMeshAgent.hasPath)
        {
            navMeshAgent.SetDestination(treasureNavMeshAgent.destination);  // Follow the treasure
        }

        if (navMeshAgent.hasPath && !navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // Reactivate obstacles
                    EnableDisabledObstacles();

                    // Destroy indicator
                    if (activeMoveIndicator != null)
                    {
                        Destroy(activeMoveIndicator);
                    }
                }
            }
        }
        if (currentState == CharacterState.Carry)
        {
            
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), Time.deltaTime);
        }
    }

    public void SetTargetPosition(Vector3 position)
    {
        if (!Movable)
        {
            return;
        }
        navMeshAgent.SetDestination(position);

        Debug.Log(name + "go to:" + position);

        // Disable matching obstacles
        DisableMatchingObstacles();

        // Instantiate move indicator
        if (moveIndicatorPrefab != null)
        {
            if (activeMoveIndicator != null)
            {
                Destroy(activeMoveIndicator);
            }
            activeMoveIndicator = Instantiate(moveIndicatorPrefab, position, Quaternion.identity);
        }
    }

    public void FollowTreasure(NavMeshAgent treasureAgent)
    {
        treasureNavMeshAgent = treasureAgent;  // Set the reference to the treasure's NavMeshAgent
    }

    private void DisableMatchingObstacles()
    {
        // Check for obstacles in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, obstacleLayer);
        foreach (Collider collider in colliders)
        {
            Obstacle obstacle = collider.GetComponent<Obstacle>();
            if (obstacle != null && obstacle.obstacleType == characterType)
            {
                // Disable obstacle
                obstacle.SetObstacleActive(false);
                disabledObstacles.Add(obstacle);
            }
        }
    }

    private void EnableDisabledObstacles()
    {
        foreach (Obstacle obstacle in disabledObstacles)
        {
            if (obstacle != null)
            {
                obstacle.SetObstacleActive(true);
            }
        }
        disabledObstacles.Clear();
    }
    public void EnterCarryState()
    {
        currentState = CharacterState.Carry;
    }
}
