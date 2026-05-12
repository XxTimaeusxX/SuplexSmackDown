using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    PlayerManager player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<PlayerManager>();
        }

        player.isPerformingAction = false;
        player.canRotate = true;
        player.canMove = true;
        player.applyRootMotion = false;
        player.isJumping = false;
    }
}
