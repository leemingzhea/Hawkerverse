using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    public Transform orderingPos;  // Where the customer stops to order
    public Transform leavingPos;   // Where the customer leaves to
    public NavMeshAgent myAgent;

    private bool hasReachedOrderingPos = false;
    private bool isLeaving = false;

    private CustomerInteraction customerInteraction;
    private CustomerQueueManager queueManager; 


    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();

        customerInteraction = GetComponent<CustomerInteraction>();
        queueManager = FindObjectOfType<CustomerQueueManager>();
    }

    void Update()
    {
        if (!hasReachedOrderingPos && myAgent.remainingDistance <= myAgent.stoppingDistance && !myAgent.pathPending)
        {
            hasReachedOrderingPos = true;
            myAgent.isStopped = true;
        }

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
