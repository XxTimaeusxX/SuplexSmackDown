using UnityEngine;

public class RockyAnimations : MonoBehaviour
{
    public RockyRhodes Rhodes;
    public RhockyAbilities RockyRhodesStates;
    [Header("RockyAnimations")]
    public Animator RockyAnimator;
    private string CurrentMicroAnimation = "";
    [SerializeField] private float runVelocityThreshold = 0.1f;
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
        //    ChangeAnimation("RockyGrabbed_demo");
            return;
        }
      
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.BullRush)
        {
            //    Debug.Log("Changing to Bullrush animation");
         //   ChangeAnimation("BullRushCharge");
            return;
        }
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.CannonBall)
        {
            AudioManager.PlaySFX(AudioManager.Instance.CannonballPhrase1, 1f);
        //    Debug.Log("Changing to Cannonball animation");
         //   ChangeAnimation("CannonBallLaunch");
            return;
        }
        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Chestbump)
        {
         //   Debug.Log("Changing to Chestbump animation");
        //    ChangeAnimation("Chestbump_demo");
            return;
        }
        if(RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.HeelTaunt)
        {
            ChangeAnimation("HeelTaunt");
            return;
        }
            if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Haymaker)
            {
               // Debug.Log("Changing to Haymaker animation");
             //   ChangeAnimation("HayMakerCharge"); // Or whichever animation name is the actual active punch
                return;
            }
        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.QTEFail)
        {
            ChangeAnimation("ChestBumpPunch"); // or your punch animation
            return;
        }
        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Exhausted)
        {
         //   Debug.Log("Changing to Exhausted animation");
            ChangeAnimation("Dizzy");
            return;
        }
     /*   if (Rhodes.rb != null && Rhodes.rb.linearVelocity.magnitude > runVelocityThreshold)
        {
            ChangeAnimation("RockyRun");
            return;
        }*/

        if (RockyRhodesStates.CurrentRockyState == global::RockyRhodesStates.Idle)
        {
            ChangeAnimation("RockyIdle");
            return;
        }
    }
}
