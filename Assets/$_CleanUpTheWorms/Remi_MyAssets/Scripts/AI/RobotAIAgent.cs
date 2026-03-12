using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class RobotAIAgent : MonoBehaviour
{
	public static RobotAIAgent Instance { get; private set; }

	[SerializeField] float updateInterval = 0.5f;
    [SerializeField] float minDistanceToPlayer = 2f;

    public Transform metaCameraRig;
	public NavMeshAgent agent;
	private Coroutine followCoroutine;
	private GameObject objectClued;
	private bool isWaitingForCollectablePickup = false;

	public float GetMinDistanceToPlayer()
	{
		return minDistanceToPlayer;
	}

	public void SetMinDistanceToPlayer(float newDistance)
	{
		agent.stoppingDistance = newDistance;
	}

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
    }
    private void Start()
    {
        agent.stoppingDistance = minDistanceToPlayer;
        ResumeFollowing();
    }

	private void Update()
	{
		if (objectClued != null)
		{
			agent.SetDestination(objectClued.transform.position);
		}

		if (objectClued != null && agent.remainingDistance <= agent.stoppingDistance && !isWaitingForCollectablePickup)
		{
			Debug.Log("Arrived at collectable, waiting for pickup...");
			isWaitingForCollectablePickup = true;
			S_StateRobot.Instance.currentState = RobotState.CLUE;
			StartCoroutine(WaitForObjectPickedUp(objectClued));
		} 
		else if (objectClued != null && agent.remainingDistance > agent.stoppingDistance && isWaitingForCollectablePickup)
		{
			Debug.Log("Collectible moved, going to join it...");
			StopCoroutine(WaitForObjectPickedUp(objectClued));
			S_StateRobot.Instance.currentState = RobotState.WALK;
			isWaitingForCollectablePickup = false;
		}
		else if (objectClued == null && followCoroutine == null)
		{
			if (StateLevel1.Instance != null && StateLevel1.Instance.currentTextToDisplay != StateLevel1TextRobot.APPARITION_LARVE)
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