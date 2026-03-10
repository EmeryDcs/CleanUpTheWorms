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

    private void OnEnable()
    {
        agent.speed = movementSpeed;
        RestartBehaviorAndMakeDynamic();
    }

    private void OnDisable()
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

        transform.localPosition = new Vector3(0f, -0.0839999989f, 0f);
        transform.localRotation = Quaternion.Euler(-180f, 0f, 0f);
    }

    public void RestartBehaviorAndMakeDynamic()
    {
        if (agent != null)
        {
            agent.enabled = false;
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

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 3.0f, NavMesh.AllAreas))
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