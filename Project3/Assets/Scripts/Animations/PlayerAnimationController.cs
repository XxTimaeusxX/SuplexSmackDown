using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    SuplexController suplexController;
    MovementController movementController;
    MovementConfig movementConfig;
    PlayerDash playerDash;

    public Animator CoheteAnimator;

    private string CurrentAnimation = string.Empty;

    private static readonly int HurtHash = Animator.StringToHash("HURT");

    public bool IsHurt = false;
    private bool isPlayingHurt = false;

    private void Awake()
    {
        suplexController = GetComponent<SuplexController>();
        movementController = GetComponent<MovementController>();
        movementConfig = GetComponent<MovementConfig>();
        playerDash = GetComponent<PlayerDash>();
    }
    //---------------- Animation ---------------------------//
    public void ChangeAnimtion(string animation, float crossfade = 0.2f)
    {
        if (CurrentAnimation != animation)
        {
            CurrentAnimation = animation;
            CoheteAnimator.CrossFade(animation, crossfade);

        }
    }
    public void CheckAnimation()
    {
        //--------- HURT handling -----------//
        if (isPlayingHurt)// If we are playing HURT (no transitions), don't switch to anything else until the clip finishes.
        {
            var currentstate = CoheteAnimator.GetCurrentAnimatorStateInfo(0);
            // If Animator is blending, let it settle
            if (CoheteAnimator.IsInTransition(0))
                return;

            // Keep HURT locked until its clip finishes
            if (currentstate.shortNameHash == HurtHash && currentstate.normalizedTime < 0.99f)
                return;

            // Clip finished, unlock and fall through to normal selection
            isPlayingHurt = false;
            CurrentAnimation = string.Empty; // allow next state change this frame
        }

        // Enter HURT when health drops; set lock so nothing overrides it
        if (IsHurt)
        {
            ChangeAnimtion("HURT");
            isPlayingHurt = true;
            IsHurt = false; // consume request
            return;
        }

        // ----- GRAB / GRABAIR / GRABWALK -----
        if (suplexController.carriedObject != null)
        {

            if (!movementController.isGrounded)
            {
                ChangeAnimtion("GRABAIR");
                return;
            }
            // Make GRABWALK behave like WALK: every time movement resumes, switch to GRABWALK.
            if (movementController.InputDirection.magnitude >= 0.1f)
            {
                Debug.Log("Changing to GRABWALK animation");
                ChangeAnimtion("GRABWALK");
                return;
            }

            return;
        }
        // ----- DASHING -----//
        if (playerDash.isDashing)
        {
            ChangeAnimtion("GRABAIR");
            return;
        }
        //----- jump / freefall / walk / idle settiings -----//
        if (!movementController.isGrounded)// Jumping takes priority if not grounded
        {
            if (movementController.velocity.y > 0.01f) ChangeAnimtion("JUMP");
            else
            {
                // only switch to FREEFALL after the configured delay AND a sufficient downward velocity
                if (movementConfig.airTime >= movementConfig.freefallDelay && movementController.velocity.y <= movementConfig.freefallVelocityThreshold)
                    ChangeAnimtion("FREEFALL");
                else
                    ChangeAnimtion("JUMP"); // still considered jump/rise or early fall
            }
            return;
        }

        // ----- Grounded movement -----
        if (movementController.InputDirection.magnitude >= 0.1f) ChangeAnimtion("WALK");
        else ChangeAnimtion("IDLE");
    }
}
