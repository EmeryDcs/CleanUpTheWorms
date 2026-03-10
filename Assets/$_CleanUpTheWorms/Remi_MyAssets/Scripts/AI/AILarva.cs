using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class AILarva : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 3f;
    [SerializeField] private float movementSpeed = 3.5f;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private Coroutine wanderCoroutine;
    private Coroutine fallCoroutine;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        agent.speed = movementSpeed;
        RestartBehaviorAndMakeDynamic();
    }

    public void StopBehaviorAndMakeKinematic()
    {
        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }

        if (wanderCoroutine != null)
        {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void RestartBehaviorAndMakeDynamic()
    {
        if (agent != null)
        {
            agent.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (fallCoroutine == null && gameObject.activeInHierarchy)
        {
            fallCoroutine = StartCoroutine(WaitForGroundRoutine());
        }
    }

    private IEnumerator WaitForGroundRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        while (rb.linearVelocity.sqrMagnitude > 0.05f)
        {
            yield return null;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            if (agent != null)
            {
                agent.enabled = true;
                agent.speed = movementSpeed;
            }

            if (wanderCoroutine == null && gameObject.activeInHierarchy)
            {
                wanderCoroutine = StartCoroutine(WanderRoutine());
            }
        }

        fallCoroutine = null;
    }

    private IEnumerator WanderRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(wanderInterval);

        while (true)
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
                {
                    Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                    randomDirection += transform.position;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                }
            }
            yield return wait;
        }
    }
}