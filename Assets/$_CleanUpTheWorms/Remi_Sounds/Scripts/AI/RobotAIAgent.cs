using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RobotAIAgent : MonoBehaviour
{

    [SerializeField] float updateInterval = 0.5f;
    [SerializeField] float minDistanceToPlayer = 2f;

    private NavMeshAgent agent;
    private Transform metaCameraRig;
    private Coroutine followCoroutine;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        OVRCameraRig rig = Object.FindAnyObjectByType<OVRCameraRig>();
        if (rig != null)
        {
            metaCameraRig = rig.transform;
        }
        else
        {
            Debug.LogError("OVRCameraRig not found in the scene.");
        }
    }
    private void Start()
    {
        agent.stoppingDistance = minDistanceToPlayer;
        ResumeFollowing();
    }

    public void OnEventGoToLocation(Vector3 specificLocation)
    {
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
        agent.SetDestination(specificLocation);
    }

    public void ResumeFollowing()
    {
        if (followCoroutine == null)
        {
            followCoroutine = StartCoroutine(FollowRoutine());
        }
    }

    private IEnumerator FollowRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(updateInterval);

        while (true)
        {
            if (metaCameraRig != null)
            {
                agent.SetDestination(metaCameraRig.position);
            }
            yield return wait;
        }
    }
}