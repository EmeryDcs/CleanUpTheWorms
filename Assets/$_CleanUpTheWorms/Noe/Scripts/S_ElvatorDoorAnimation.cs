using UnityEngine;

public class S_ElvatorDoorAnimation : MonoBehaviour
{
    public Animator animator;

    public void switchAnim()
    {
        animator.SetBool("isTutoOver", true);
    }
}
