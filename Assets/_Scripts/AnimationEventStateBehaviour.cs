using UnityEngine;
using UnityEngine.Events;

public class AnimationEventStateBehaviour : StateMachineBehaviour {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationManager.Instance.LockAnimation();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimationManager.Instance.DelockAnimation();
    }
}