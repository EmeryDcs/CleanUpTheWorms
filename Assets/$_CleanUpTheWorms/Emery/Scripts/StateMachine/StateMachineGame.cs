using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public enum GameState
{
	ASCENCEUR, //Dialogue avec robot
	TUTORIAL, //Ramassage des boulettes de papiers
	LEVEL1, //Débloquage de la lance téléscopique
	LEVEL2, //Ramassage d'une tonne de bébéte
	END, //Cinématique avec la grosse bébéte
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

	[SerializeField] S_TriggerEventButtonLift triggerEventButtonLift;

	[SerializeField] UnityEvent endBubble;


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
    }

	public void AfficherFinDuJeu()
	{
		ui.SetActive(true);
	}


	IEnumerator ShowEnding()
	{
		yield return new WaitForSeconds(2f);

		Debug.Log("ShowEnding");
		
		endBubble.Invoke();

		yield return new WaitForSeconds(1.5f);

        triggerEventButtonLift.TriggerBlinking(true);

        yield return new WaitForSeconds(10f);

		if (StateMachineGame.Instance.state != GameState.ENDING)
		{
			triggerEventButtonLift.SetCanActivate(false);
			triggerEventButtonLift.Ending(false);
        }

    }


}
