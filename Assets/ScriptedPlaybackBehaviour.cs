using UnityEngine;
using System.Collections;

public class ScriptedPlaybackBehaviour : StateMachineBehaviour
{
    public float time;
    public string state;
    public AnimationCurve curve;
    float oldSpeed;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time = 0;
        oldSpeed = animator.speed;
        animator.speed = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(state, -1, curve.Evaluate(time));
        if (time >= 0.9f)
        {
            animator.speed = oldSpeed;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = oldSpeed;
    }

}
