using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class S_RecenterPosition : MonoBehaviour
{
	[Header("Position pour recenter")]
	public Transform head;
	public Transform origin;
	public Transform target;
	public Transform targetDesk;

	[Header("Input pour recenter")]
	public InputActionReference recenterInput;


	float timer = 0;

	public void Awake()
	{
		StartCoroutine(WaitForStart());
	}

	public IEnumerator WaitForStart()
	{
		yield return new WaitForEndOfFrame();
		Recenter(target);
	}

	public void TeleportDesk()
	{
		Recenter(targetDesk);
    }

	public void Recenter(Transform _target)
	{
		Vector3 offset = head.position - origin.position;
		offset.y = 0f;
		origin.position = _target.position - offset;
		
		Vector3 targetForward = _target.forward;
		targetForward.y = 0f;
		Vector3 cameraForward = head.forward;
		cameraForward.y = 0f;

		float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
		origin.RotateAround(head.position, Vector3.up, angle);
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
            if (recenterInput.action.WasPressedThisFrame())
            {
                Recenter(target);
            }
        }
		else
        {
            if (recenterInput.action.IsPressed())
			{
                timer += Time.deltaTime;
                if (timer >= 3)
                {
                    timer = 0;
                    Recenter(target);
                }
            }
            else
            {
                timer = 0;
            }
        }


	}
}
