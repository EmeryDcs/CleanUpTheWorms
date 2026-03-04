using UnityEngine;

public class S_VacuumObject : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("GrabbableElm"))
		{
			switch (StateMachineGame.Instance.state)
			{
				case GameState.TUTORIAL:
					StateTutorial.Instance.DeleteCollectableFromList(other.gameObject);
					break;
				case GameState.LEVEL1:
					StateLevel1.Instance.DeleteCollectableFromList(other.gameObject);
					break;
				case GameState.LEVEL2:
					StateLevel2.Instance.DeleteCollectableFromList(other.gameObject);
					break;
			}
		}
	}
}
