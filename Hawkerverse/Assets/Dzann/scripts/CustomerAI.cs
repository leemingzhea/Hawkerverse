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

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.SetDestination(orderingPos.position);

        customerInteraction = GetComponent<CustomerInteraction>();
    }

    void Update()
    {
        // First: stop at ordering position
        if (!hasReachedOrderingPos && myAgent.remainingDistance <= myAgent.stoppingDistance)
        {
            hasReachedOrderingPos = true;
            myAgent.isStopped = true;
        }

        // After drink is delivered, go to leaving position
        if (!isLeaving && customerInteraction != null && customerInteraction.drinkDelivered)
        {
            isLeaving = true;
            myAgent.isStopped = false;
            myAgent.SetDestination(leavingPos.position);
        }
    }
}
