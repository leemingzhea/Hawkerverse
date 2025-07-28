using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    public Transform orderingPos;  // Where the customer stops to order
    public Transform leavingPos;   // Where the customer leaves to
    public NavMeshAgent myAgent;
    private Transform playerTransform;


    private bool hasReachedOrderingPos = false;
    private bool isLeaving = false;

    private CustomerInteraction customerInteraction;
    private CustomerQueueManager queueManager;

    private Animator animator; // Reference to the Animator component

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator component

        customerInteraction = GetComponent<CustomerInteraction>();
        queueManager = FindObjectOfType<CustomerQueueManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

    }

    void Update()
    {
        // Update animation speed based on actual movement
        if (animator != null && myAgent != null)
        {
            float speed = myAgent.isStopped ? 0f : myAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }


        // Handle reaching ordering position
        if (!hasReachedOrderingPos && myAgent.remainingDistance <= myAgent.stoppingDistance && !myAgent.pathPending)
        {
            hasReachedOrderingPos = true;
            myAgent.isStopped = true;
        }

        // Handle leaving
        if (!isLeaving && customerInteraction != null && customerInteraction.drinkDelivered)
        {
            isLeaving = true;
            myAgent.isStopped = false;
            myAgent.SetDestination(leavingPos.position);
        }

        if (isLeaving && Vector3.Distance(transform.position, leavingPos.position) <= 0.5f)
        {
            queueManager.OnCustomerLeaves(this.gameObject);
            OnCustomerLeaves(); // Destroys customer
        }

        if (!hasReachedOrderingPos && myAgent.remainingDistance <= myAgent.stoppingDistance && !myAgent.pathPending)
        {
            hasReachedOrderingPos = true;
            myAgent.isStopped = true;
        }

        if (hasReachedOrderingPos && !isLeaving && playerTransform != null)
        {
            Vector3 lookDirection = playerTransform.position - transform.position;
            lookDirection.y = 0; // Ignore vertical difference
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

    }


    public void SetQueueManager(CustomerQueueManager manager)
    {
        queueManager = manager;
    }

    public void MoveTo(Vector3 destination)
    {
        if (myAgent != null && myAgent.isOnNavMesh)
        {
            myAgent.SetDestination(destination);
            myAgent.isStopped = false; // Make sure movement starts
            Debug.Log("Moving customer to: " + destination);
        }
        else
        {
            Debug.LogWarning("Agent not on NavMesh or missing!");
        }
    }


    public void OnCustomerLeaves()
    {
        // Handle customer leaving logic
        myAgent.isStopped = true;
        Destroy(gameObject, 1f); // Optional: delay for walking away
    }
}
