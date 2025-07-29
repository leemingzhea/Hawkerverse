using UnityEngine;
using UnityEngine.AI;

public class SpawnAndMoveAI : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject agentPrefab;
    public Transform spawnPoint;
    public Transform destinationPoint;

    private GameObject spawnedAgent;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        Debug.Log("SpawnAndMoveAI: Start called.");
        SpawnAndMove();
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    void SpawnAndMove()
    {
        if (agentPrefab == null || spawnPoint == null || destinationPoint == null)
        {
            Debug.LogWarning("Missing references: assign prefab, spawnPoint, and destinationPoint in Inspector.");
            return;
        }

        // Instantiate the prefab
        spawnedAgent = Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("SpawnAndMoveAI: Agent spawned at " + spawnPoint.position);

        agent = spawnedAgent.GetComponent<NavMeshAgent>();
        animator = spawnedAgent.GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("Spawned object does not have a NavMeshAgent.");
            return;
        }

        if (destinationPoint != null)
        {
            agent.SetDestination(destinationPoint.position);
            Debug.Log("Destination set to " + destinationPoint.position);
        }
    }
}
