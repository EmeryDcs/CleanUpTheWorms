using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public enum GameState
{
	ASCENCEUR, //Dialogue avec robot
	TUTORIAL, //Ramassage des boulettes de papiers
	LEVEL1, //D�bloquage de la lance t�l�scopique
	LEVEL2, //Ramassage d'une tonne de b�b�te
	END, //Cin�matique avec la grosse b�b�te
	ENDING, //Fin du jeu
}

public class StateMachineGame : MonoBehaviour
{
    public static StateMachineGame Instance { get; private set; }

	public GameState state;
	public GameObject stateAscenceur;
	public GameObject stateTutorial;
	public GameObject stateLevel1;
	public GameObject stateLevel2;


	public bool hasWin = false;

	[SerializeField]
	GameObject ui;


	bool ending = false;
	private bool isTuto = false;
	private bool isFirstLevel = false;
	private bool isTips = true;


	[SerializeField] S_TriggerEventButtonLift triggerEventButtonLift;

	[SerializeField] UnityEvent endBubble;

	[SerializeField] private UnityEvent tipsBubble;
	
	[SerializeField] private GameObject larvaUp;

    private void Start()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		} 
		else
		{
			Instance = this;
		}

		state = GameState.ASCENCEUR;
	}

	private void Update()
	{
		stateAscenceur.SetActive(state == GameState.ASCENCEUR);
		stateTutorial.SetActive(state == GameState.TUTORIAL);
		stateLevel1.SetActive(state == GameState.LEVEL1);
		stateLevel2.SetActive(state == GameState.LEVEL2);

		if (state == GameState.END && !ending)
		{
			ending = true;
			StartCoroutine(ShowEnding());
        }
		else if (state == GameState.TUTORIAL && !isTuto)
		{
			isTuto = true;
			RobotAIAgent.Instance.ResumeFollowing(true);
		}
		else if (state == GameState.LEVEL1 && !isFirstLevel)
		{
			isFirstLevel = true;
			RobotAIAgent.Instance.ResumeFollowing(false);
		}
		else if (state  == GameState.LEVEL2 && isTips)
		{
			isTips = false;
			StartCoroutine(ShowTips());
		}
    }

	public void AfficherFinDuJeu()
	{
		ui.SetActive(true);
	}

	IEnumerator ShowTips()
	{
		yield return new WaitForSeconds(45f);
		if (larvaUp != null)
		{
			tipsBubble.Invoke();
		}
	}


	IEnumerator ShowEnding()
	{
		yield return new WaitForSeconds(2f);

		Debug.Log("ShowEnding");
		
		endBubble.Invoke();

		yield return new WaitForSeconds(6f);

        triggerEventButtonLift.TriggerBlinking(true);
        triggerEventButtonLift.SetCanTriggerEnd(true);

        yield return new WaitForSeconds(10f);

		if (StateMachineGame.Instance.state != GameState.ENDING)
		{
			triggerEventButtonLift.SetCanActivateHasNotLose(false);
			triggerEventButtonLift.Ending(false);
        }

    }


}
