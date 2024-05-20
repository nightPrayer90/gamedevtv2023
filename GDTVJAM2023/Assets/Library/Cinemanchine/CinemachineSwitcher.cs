using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{

    private Animator animator;
    public bool topCamera_flag = true;
    public PlayerInputHandler inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputHandler.OnCameraSwitchInputChanged += SwitchState;
    }

    private void OnDisable()
    {
        inputHandler.OnCameraSwitchInputChanged -= SwitchState;
    }


    public void SwitchState()
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
