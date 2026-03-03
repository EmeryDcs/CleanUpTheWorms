using System.Collections.Generic;
using UnityEngine;

public class StateLevel1 : MonoBehaviour
{
	[SerializeField]
	List<GameObject> listCollectables;

	// Update is called once per frame
	void Update()
	{
		if (listCollectables.Count == 0)
		{
			StateMachineGame.Instance.state = GameState.LEVEL2;
		}
	}

	public void DeleteCollectableFromList(GameObject go)
	{
		if (go == null) return;

		if (listCollectables.Contains(go))
		{
			listCollectables.Remove(go);
		}
	}
}
