using UnityEngine;

public enum GameState
{
	TUTORIAL, //Ramassage des boulettes de papiers
	LEVEL1, //Débloquage de la lance téléscopique
	LEVEL2, //Ramassage d'une tonne de bébéte
	END, //Cinématique avec la grosse bébéte
}

public class StateMachineGame : MonoBehaviour
{
    public static StateMachineGame Instance { get; private set; }

	public GameState state;
	public GameObject stateTutorial;
	public GameObject stateLevel1;
	public GameObject stateLevel2;

	[SerializeField]
	GameObject ui;

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

		state = GameState.TUTORIAL;
	}

	private void Update()
	{
		stateTutorial.SetActive(state == GameState.TUTORIAL);
		stateLevel1.SetActive(state == GameState.LEVEL1);
		stateLevel2.SetActive(state == GameState.LEVEL2);
	}

	public void AfficherFinDuJeu()
	{
		ui.SetActive(true);
	}
}
