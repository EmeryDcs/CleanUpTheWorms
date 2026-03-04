using UnityEngine;

public class S_DestroyOnVacuuming : MonoBehaviour
{
	private void OnDestroy()
	{
		switch (StateMachineGame.Instance.state)
		{
			case GameState.TUTORIAL:
				StateTutorial.Instance.DeleteCollectableFromList(gameObject);
				break;
			case GameState.LEVEL1:
				StateLevel1.Instance.DeleteCollectableFromList(gameObject);
				break;
			case GameState.LEVEL2:
				StateLevel2.Instance.DeleteCollectableFromList(gameObject);
				break;
		}
	}
}
