using System.Collections.Generic;
using UnityEngine;

public class StateLevel2 : MonoBehaviour
{
	[SerializeField]
	List<GameObject> listCollectables;

	public static StateLevel2 Instance { get; private set; }

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
			StateMachineGame.Instance.state = GameState.END;
			StateMachineGame.Instance.AfficherFinDuJeu();
		}
	}

	public void DeleteCollectableFromList(GameObject go)
	{
		Debug.Log("StateLevel2: DeleteCollectableFromList");
		if (go == null) return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
			Destroy(go);
		}
	}
}
