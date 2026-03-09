using UnityEngine;
using UnityEngine.Events;

public class S_TriggerEventButtonLift : MonoBehaviour
{
	public UnityEvent OnButtonPushed;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("ButtonLift"))
			OnButtonPushed.Invoke();
	}
}
