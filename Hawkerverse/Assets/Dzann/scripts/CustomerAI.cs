using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    public Transform targetPoint; // where the customer stops (near the stall)
    public UnityEngine.AI.NavMeshAgent myAgent;
    public bool hasReached = false;
    public bool readyToOrder = false;

    void Start()
    {
        myAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        myAgent.SetDestination(targetPoint.position);
    }

    void Update()
    {
        if (!hasReached && myAgent.remainingDistance <= myAgent.stoppingDistance)
        {
            hasReached = true;
            readyToOrder = true;
            myAgent.isStopped = true;
        }
    }
}
 