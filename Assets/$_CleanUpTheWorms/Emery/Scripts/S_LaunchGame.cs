using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_LaunchGame : MonoBehaviour
{
    public InputActionReference inputTrigger;
	public Renderer imageBackground;
	public GameObject ui;

	Color color;

	private void Start()
	{
		color = imageBackground.material.GetColor("_Color");
	}

	// Update is called once per frame
	void Update()
	{
		if (inputTrigger.action.IsPressed())
		{
			ui.SetActive(false);
			color.a += Time.deltaTime;
			imageBackground.material.SetColor("_Color", color);
			if (color.a > 1f)
			{
				SceneManager.LoadScene("Game");
			}
		}
		else if (color.a > 0f)
		{
			ui.SetActive(true);
			color.a -= Time.deltaTime;
			imageBackground.material.SetColor("_Color", color);
			if (color.a < 0)
			{
				color.a = 0;
			}
		}
	}
}
