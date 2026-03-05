using System.Collections.Generic;
using UnityEngine;

public class StateTutorial : MonoBehaviour
{
	[SerializeField]
	List<GameObject> listCollectables;

	public static StateTutorial Instance { get; private set; }

	//Variables pour les indices du robot
	float timerBeforeClue = 20f;
	bool clueDisplayed = false;

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

	// Update is called once per frame
	void Update()
    {
        if (listCollectables.Count == 0)
		{
			StateMachineGame.Instance.state = GameState.LEVEL1;
		}

		timerBeforeClue -= Time.deltaTime;
		if (timerBeforeClue <= 0 && !clueDisplayed)
		{
			clueDisplayed = true;
			RobotAIAgent.Instance.RushToNearestCollectable(listCollectables);
		}
	}

	public void DeleteCollectableFromList(GameObject go)
	{
		if (go == null) return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
			Destroy(go);
			clueDisplayed = false;
			timerBeforeClue = 20f;
		}
	}
}
