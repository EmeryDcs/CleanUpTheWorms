using UnityEngine;

public class S_RobotAnim : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();    
    }


    public void SetAnimOpening(bool isOpen)
    {
        anim.SetBool("isOpening", isOpen);
    }
}
