using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{

    private Animator animator;
    public bool topCamera_flag = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetButtonDown("SwitchView"))
        {
            SwitchState();
        }*/
    }

    private void SwitchState()
    {
        if (!topCamera_flag)
        {
            animator.Play("TopCamera");
            AudioManager.Instance.PlaySFX("ViewChange");
        }
        else
        {
            animator.Play("FollowCamera");
            AudioManager.Instance.PlaySFX("ViewChange");
        }
        topCamera_flag = !topCamera_flag;
    }
}
