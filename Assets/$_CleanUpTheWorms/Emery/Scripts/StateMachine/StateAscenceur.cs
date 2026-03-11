using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Rendering;

enum StateRobotText
{
	TRIGGER,
	ANIMATION_OPENING_TRASH,
	BUTTON_PUSHED
}

public class StateAscenceur : MonoBehaviour
{
	public static StateAscenceur Instance { get; private set; }

	[Header("UI & Texts")]
	public GameObject uiRobotTalk;
	public GameObject triggerText;
	public GameObject animationOpeningTrashText;
	public GameObject buttonPushedText;

	[Header("Input")]
	public InputActionReference trigger;

	[Header("Button")]
	public GameObject buttonLift;

	[Header("List element to pickup")]
	public List<GameObject> listCollectables;

	StateRobotText currentState;
	int testTrigger = 0;
	bool hasTriggered = false;
	bool isCatchingAvailable = false;

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

	private void Start()
	{
		currentState = StateRobotText.TRIGGER;
		triggerText.SetActive(true);
		animationOpeningTrashText.SetActive(false);
		buttonPushedText.SetActive(false);
	}

	private void Update()
	{
		switch (currentState)
		{
			case StateRobotText.TRIGGER:
				HandleTriggerState();
				break;
			case StateRobotText.ANIMATION_OPENING_TRASH:
				HandleAnimationOpeningTrashState();
				break;
		}
	}

	private void HandleTriggerState()
	{
		if (trigger.action.ReadValue<float>() >= 0.8f && !hasTriggered)
		{
			hasTriggered = true;
			testTrigger++;
		} 
		else if (trigger.action.ReadValue<float>() < 0.2f)
		{
			hasTriggered = false;
		}

		if (testTrigger >= 3)
		{
			triggerText.SetActive(false);
			animationOpeningTrashText.SetActive(true);
			currentState = StateRobotText.ANIMATION_OPENING_TRASH;
		}
	}

	private void HandleAnimationOpeningTrashState()
	{
		if (listCollectables.Count <= 0)
		{
			animationOpeningTrashText.SetActive(false);
			buttonPushedText.SetActive(true);
			currentState = StateRobotText.BUTTON_PUSHED;
			buttonLift.GetComponent<BoxCollider>().enabled = true;
		}
	}

	public void OnButtonPushed() 
	{
		if (currentState == StateRobotText.BUTTON_PUSHED)
		{
			uiRobotTalk.SetActive(false);
			StateMachineGame.Instance.state = GameState.TUTORIAL;
			FindFirstObjectByType<S_TriggerEventButtonLift>().SetCanBePushed(false);
		}
	}
	public void DeleteCollectableFromList(GameObject go)
	{
		if (go == null) return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
			Destroy(go);
		}
	}
}
