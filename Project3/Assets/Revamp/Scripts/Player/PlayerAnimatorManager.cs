using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;
    PlayerMovementManager playerMovementManager;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        playerMovementManager = GetComponent<PlayerMovementManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        player.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        player.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayTargetActionAniamtion(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        player.applyRootMotion = applyRootMotion;
        player.animator.CrossFade(targetAnimation, 0.2f);
        player.isPerformingAction = isPerformingAction;
        player.canRotate = canRotate;
        player.canMove = canMove;
    }

    private void OnAnimatorMove()
    {
        if (player.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;
            player.characterController.Move(playerMovementManager.moveDirection * playerMovementManager.dashSpeed * Time.deltaTime);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }
}
