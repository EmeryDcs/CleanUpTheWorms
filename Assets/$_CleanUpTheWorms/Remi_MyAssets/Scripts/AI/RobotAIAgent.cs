using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class RobotAIAgent : MonoBehaviour
{
	public static RobotAIAgent Instance { get; private set; }

	[SerializeField] float updateInterval = 0.5f;
    [SerializeField] float minDistanceToPlayer = 2f;

    public NavMeshAgent agent;
    private Transform metaCameraRig;
    private Coroutine followCoroutine;
	private GameObject objectClued;

    private void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}

		agent = GetComponent<NavMeshAgent>();

        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
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

	private void Update()
	{
		if (objectClued != null && agent.remainingDistance <= agent.stoppingDistance)
		{
			Debug.Log("Arrived at the collectable. Waiting for it to be picked up...");
			S_StateRobot.Instance.currentState = RobotState.CLUE;
			StartCoroutine(WaitForObjectPickedUp(objectClued));
		}
		else if (objectClued == null && followCoroutine == null)
		{
			ResumeFollowing();
		}
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

	// Coroutine to wait until the collectable is picked up (i.e., becomes null)
	private IEnumerator WaitForObjectPickedUp(GameObject collectable)
	{
		yield return new WaitUntil(() => collectable == null); 
		ResumeFollowing();
		Debug.Log("Collectable picked up. Resuming follow.");
		S_StateRobot.Instance.currentState = RobotState.IDLE;
	}

	// Method to find the nearest collectable (if it exist) and move towards it
	public void RushToNearestCollectable(List<GameObject> listCollectables)
	{
		float temporaryMinDistance = 200f;
		GameObject nearestCollectable = null;

		foreach (GameObject collectable in listCollectables)
		{
			if (collectable == null) continue;
			float distanceToCollectable = Vector3.Distance(transform.position, collectable.transform.position);
			if (distanceToCollectable < temporaryMinDistance)
			{
				nearestCollectable = collectable;
			}
		}
		if (nearestCollectable != null)
		{
			OnEventGoToLocation(nearestCollectable.transform.position);
			objectClued = nearestCollectable;
		}
	}
}