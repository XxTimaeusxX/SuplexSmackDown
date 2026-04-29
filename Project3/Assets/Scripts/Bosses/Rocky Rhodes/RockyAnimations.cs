using UnityEngine;

public class RockyAnimations : MonoBehaviour
{
    public RockyRhodes Rhodes;
    public RhockyAbilities RockyRhodesStates;
    [Header("RockyAnimations")]
    public Animator RockyAnimator;
    private string CurrentMicroAnimation = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rhodes = GetComponent<RockyRhodes>();
       RockyRhodesStates = GetComponentInParent<RhockyAbilities>();
        RockyAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAnimation();
    }
    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (CurrentMicroAnimation != animation)
        {
            CurrentMicroAnimation = animation;
           RockyAnimator.CrossFade(animation, crossfade);

        }
    }
    public void CheckAnimation()
    {
        if (Rhodes.isGrabbed)
        {
        //    Debug.Log("ROCKY IS CURRENTLY GRABBED");
            ChangeAnimation("RockyGrabbed_demo");
            return;
        }
      
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.BullRush)
        {
        //    Debug.Log("Changing to Bullrush animation");
          //  ChangeAnimation("BullRush_demo");
            return;
        }
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.CannonBall)
        {
        //    Debug.Log("Changing to Cannonball animation");
            ChangeAnimation("CannonBallLaunch");
            return;
        }
        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Chestbump)
        {
         //   Debug.Log("Changing to Chestbump animation");
        //    ChangeAnimation("Chestbump_demo");
            return;
        }
        /*    if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Haymaker)
            {
                Debug.Log("Changing to Haymaker animation");
                ChangeAnimation("RockyPunchChargeUp_demo"); // Or whichever animation name is the actual active punch
                return;
            }*/
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Exhausted)
        {
         //   Debug.Log("Changing to Exhausted animation");
          //  ChangeAnimation("Exhausted_demo");
            return;
        }
        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Idle)
        {
            ChangeAnimation("RockyIdle");
            return;
        }
    }
}
