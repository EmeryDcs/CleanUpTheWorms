using UnityEngine;

public enum RobotState
{
	WALK,
	JUMP,
	IDLE,
	CLUE,
}

public class S_StateRobot : MonoBehaviour
{
	public static S_StateRobot Instance { get; private set; }
	public RobotState currentState;

	public Material idleMaterial;
	public Material walkMaterial;
	public Material jumpMaterial;
	public Material clueMaterial;

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
	}

	private void Update()
	{
		if (currentState != RobotState.CLUE)
		{
			if (RobotAIAgent.Instance.agent.velocity.magnitude == 0)
			{
				currentState = RobotState.IDLE;
			}
			if (RobotAIAgent.Instance.agent.velocity.magnitude > 0 && RobotAIAgent.Instance.agent.isOnOffMeshLink == false)
			{
				currentState = RobotState.WALK;
			}
			if (RobotAIAgent.Instance.agent.isOnOffMeshLink == true)
			{
				currentState = RobotState.JUMP;
			}
		}

		switch (currentState)
		{
			case RobotState.WALK:
				Walking();
				break;
			case RobotState.JUMP:
				Jumping();
				break;
			case RobotState.IDLE:
				Idling();
				break;
			case RobotState.CLUE:
				Clueing();
				break;
		}
	}

	private void Walking()
	{
		//All code for the walking
		gameObject.GetComponent<MeshRenderer>().material = walkMaterial;
	}

	private void Jumping()
	{
		//All code for the jumping
		gameObject.GetComponent<MeshRenderer>().material = jumpMaterial;
	}

	private void Idling()
	{
		//All code for the idling
		gameObject.GetComponent<MeshRenderer>().material = idleMaterial;
	}

	private void Clueing()
	{
		//All code for the clueing
		gameObject.GetComponent<MeshRenderer>().material = clueMaterial;
	}
}
