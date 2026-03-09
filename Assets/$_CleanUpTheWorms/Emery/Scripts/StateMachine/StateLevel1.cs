using System.Collections.Generic;
using UnityEngine;

public enum StateLevel1TextRobot
{
	APPARITION_LARVE,
	INGERATION_LARVE,
	AMELIORATION_PINCE,
	TEST_ALLONGE,
}

public class StateLevel1 : MonoBehaviour
{
	[Header("List of collectables in the level")]
	[SerializeField]
	List<GameObject> listCollectables;

	public static StateLevel1 Instance { get; private set; }

	[Header("State of the text to display")]
	public StateLevel1TextRobot currentTextToDisplay;

	[Header("UI Text")]
	public GameObject uiTextLevel1;
	public GameObject apparitionLarveText;
	public GameObject ingerationLarveText;
	public GameObject ameliorationPinceText;
	public GameObject testAllongeText;

	bool isCatchingAvailable = false;
	float timerText = 0f;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			currentTextToDisplay = StateLevel1TextRobot.APPARITION_LARVE;
		}
	}

	// Update is called once per frame
	void Update()
	{
		switch (currentTextToDisplay)
		{
			case StateLevel1TextRobot.APPARITION_LARVE:
				ApparitionLarve();
				break;
			case StateLevel1TextRobot.INGERATION_LARVE:
				IngerationLarve();
				break;
			case StateLevel1TextRobot.TEST_ALLONGE:
				TestAllonge();
				break;
		}
	}

	void ApparitionLarve()
	{
		uiTextLevel1.SetActive(true);
		isCatchingAvailable = true;
	}

	void IngerationLarve()
	{
		isCatchingAvailable = false;
		if (timerText < 10f)
		{
			timerText += Time.deltaTime;
		}
		else
		{
			currentTextToDisplay = StateLevel1TextRobot.AMELIORATION_PINCE;
			ingerationLarveText.SetActive(false);
			ameliorationPinceText.SetActive(true);
			timerText = 0f;
		}
	}

	public void AmeliorationPince()
	{
		currentTextToDisplay = StateLevel1TextRobot.TEST_ALLONGE;
		ameliorationPinceText.SetActive(false);
		testAllongeText.SetActive(true);
	}

	void TestAllonge()
	{
		if (timerText < 10f)
		{
			timerText += Time.deltaTime;
		}
		else
		{
			currentTextToDisplay = StateLevel1TextRobot.INGERATION_LARVE;
			testAllongeText.SetActive(false);
			timerText = 0f;

			if (listCollectables.Count == 0)
			{
				StateMachineGame.Instance.state = GameState.LEVEL2;
			}
		}
	}

	//Fonction appelée par l'aspirateur lorsqu'il aspire quelque chose
	public void DeleteCollectableFromList(GameObject go)
	{
		Debug.Log("StateLevel1: DeleteCollectableFromList");
		if (go == null) return;
		if (!isCatchingAvailable)
			return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
			Destroy(go);
		}
	}
}
