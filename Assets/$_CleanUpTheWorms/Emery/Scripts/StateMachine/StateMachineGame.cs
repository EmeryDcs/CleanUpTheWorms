using UnityEngine;

public enum GameState
{
	TUTORIAL, //Ramassage des boulettes de papiers
	LEVEL1, //Débloquage de la lance téléscopique
	LEVEL2, //.....à définir.....
	END, //Cinématique avec la grosse bébéte
}

public class StateMachineGame : MonoBehaviour
{
    public static StateMachineGame Instance { get; private set; }

	public GameState state;

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
	}

	private void Update()
	{
		
	}
}
