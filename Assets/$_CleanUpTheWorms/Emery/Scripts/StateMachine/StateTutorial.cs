using System.Collections.Generic;
using UnityEngine;

public class StateTutorial : MonoBehaviour
{
	[SerializeField]
	List<GameObject> listCollectables;

	public static StateTutorial Instance { get; private set; }

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
