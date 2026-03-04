using System.Collections.Generic;
using UnityEngine;

public class StateLevel1 : MonoBehaviour
{
	[SerializeField]
	List<GameObject> listCollectables;

	public static StateLevel1 Instance { get; private set; }

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
			StateMachineGame.Instance.state = GameState.LEVEL2;
		}
	}

	//Fonction appelée par l'aspirateur lorsqu'il aspire quelque chose
	public void DeleteCollectableFromList(GameObject go)
	{
		Debug.Log("StateLevel1: DeleteCollectableFromList");
		if (go == null) return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
			Destroy(go);
		}
	}
}
