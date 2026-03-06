using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

enum StateRobotText
{
	TRIGGER,
	ANIMATION_OPENING_TRASH,
	BUTTON_PUSHED
}

public class StateAscenceur : MonoBehaviour
{
	[Header("UI & Texts")]
	public GameObject uiRobotTalk;
	public GameObject triggerText;
	public GameObject animationOpeningTrashText;
	public GameObject buttonPushedText;

	[Header("Input")]
	public InputActionReference trigger;

	[Header("Button")]
	public GameObject buttonLift;

	StateRobotText currentState;
	float timerForAnimation = 0f;
	int testTrigger = 0;
	bool hasTriggered = false;

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
			timerForAnimation = 0f;
			currentState = StateRobotText.ANIMATION_OPENING_TRASH;
		}
	}

	private void HandleAnimationOpeningTrashState()
	{
		timerForAnimation += Time.deltaTime;
		if (timerForAnimation >= 10f)
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
		}
	}
}
