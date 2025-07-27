using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerQueueManager : MonoBehaviour
{
    [Header("Customer Settings")]
    public GameObject customerPrefab;
    public List<Transform> queuePositions; // Position[0] is the ordering spot
    public float spawnIntervalMin = 5f;
    public float spawnIntervalMax = 10f;
    public Transform spawnPoint;  // Assign in Inspector

    private CustomerInteraction currentCustomerAtFront;

    private Queue<GameObject> customerQueue = new Queue<GameObject>();
    private bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnCustomerRoutine());
        // Removed invalid line here
    }

    IEnumerator SpawnCustomerRoutine()
    {
        while (isSpawning)
        {
            float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(waitTime);

            if (customerQueue.Count < queuePositions.Count)
            {
                SpawnCustomer();
            }
        }
    }

    void SpawnCustomer()
    {
        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        customerQueue.Enqueue(newCustomer);

        CustomerAI ai = newCustomer.GetComponent<CustomerAI>();
        if (ai != null)
        {
            ai.SetQueueManager(this);
        }

        CustomerInteraction interactionScript = newCustomer.GetComponent<CustomerInteraction>();
        if (interactionScript != null)
        {
            interactionScript.queueManager = this;  // Assign the queueManager reference
        }

        UpdateCustomerQueuePositions();
    }

    public void OnCustomerLeaves(GameObject customer)
    {
        if (customerQueue.Contains(customer))
        {
            customerQueue.Dequeue();
            UpdateCustomerQueuePositions();
        }
    }

    void UpdateCustomerQueuePositions()
    {
        GameObject[] customers = customerQueue.ToArray();

        for (int i = 0; i < customers.Length && i < queuePositions.Count; i++)
        {
            CustomerAI ai = customers[i].GetComponent<CustomerAI>();
            if (ai != null)
            {
                ai.MoveTo(queuePositions[i].position);
            }

            // Assign the front customer (i == 0)
            CustomerInteraction interaction = customers[i].GetComponent<CustomerInteraction>();
            if (interaction != null)
            {
                interaction.isAtFront = (i == 0);
            }
        }
    }

    public GameObject GetFrontCustomer()
    {
        return customerQueue.Count > 0 ? customerQueue.Peek() : null;
    }

    public bool IsCustomerAtFront(GameObject customer)
    {
        if (customerQueue.Count == 0) return false;
        return customerQueue.Peek() == customer;
    }
}
