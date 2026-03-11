using UnityEngine;

public class S_VacuumObject : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (grab.Instance.GetGrabbedElm() == null)
		{
			if (!other.CompareTag("Pince"))
				return;
			if (StateMachineGame.Instance != null && StateMachineGame.Instance.state == GameState.LEVEL1 && StateLevel1.Instance != null && StateLevel1.Instance.currentTextToDisplay == StateLevel1TextRobot.AMELIORATION_PINCE)
			{
				StateLevel1.Instance.AmeliorationPince();
			}
			return;
		}
		if (other.CompareTag("GrabbableElm"))
		{
			switch (StateMachineGame.Instance.state)
			{
				case GameState.ASCENCEUR:
					StateAscenceur.Instance.DeleteCollectableFromList(other.gameObject);
					break;
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
