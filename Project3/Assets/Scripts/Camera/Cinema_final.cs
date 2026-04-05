using Unity.Cinemachine;
using System.Collections;
using UnityEngine;


public class Cinema_final : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject introCameraObj;
    public CinemachineCamera introCamera;
    public CinemachineCamera freeLookCamera;
    public CinemachineCamera towerCamera;

    [Header("Trigger Settings")]
    public GameObject player;
    public float triggerRange = 25f;
    public bool introPlayed = false;
    private PlayerMovement playerMovementScript;

    [Header("UI Settings")]
    public GameObject introUI;

    [Header("Sounds")]
  //  public AudioClip introSound;
 //   public bool audio1;
 //   public bool audio2;
 //   public bool audio3;

    [Header("Camera Transition Settings")]
    public float transitionSpeed = 2f;
    private Transform introCameraTransform;
    public float positionThreshold = 1f;
    public float rotationThreshold = 2f;

    [Header("---------------Level 1 Setting ------------------")]
    [Header("Phase Settings")]
    public bool isMainMenuIntro;
    public bool isPhase1Intro; // first intro before Shoal defeat
    public bool isPhase2Intro; //  second intro after Shoal defeat
  
    private CinemachineCamera lastActiveCamera;
   // private AudioSource audioSource;
    public GameObject shoalHealth;
    public GameObject bossTrigger2;

    [Header("---------------Level 2 Setting ------------------")]
    public bool triggerLevel2Intro; // Set to true to trigger Level 2 intro
    public bool triggerDefeatCam1; // Set to true to trigger Cam1
    public bool triggerDefeatCam2; // Set to true to trigger Cam2
    public float defeatCameraDuration = 8f; // How long the camera stays active

    public GameObject Level2IntroCam;
    public GameObject DefeatCam1;
    public GameObject DefeatCam2;

    //----------- Level3 settings-----------//
    [Header("---------------Level 3 Setting ------------------")]
    [Header("RockyRhodes Camera Settings")]
    public bool Boss3Intro;
    public bool IsRockyPhase1;
    public bool IsRockyPhase2;
    public bool IsRockyPhase3;
    public CinemachineCamera Boss3ClashCloseUp;
    public float CameraTransitionDuration = 7f;
    private void Start()
    {
        playerMovementScript = player.GetComponent<PlayerMovement>();
        introCameraTransform = introCamera.transform;
     //   audioSource = GetComponent<AudioSource>();

        if (!isMainMenuIntro)
            introCameraObj.SetActive(false);

        lastActiveCamera = freeLookCamera.Priority > towerCamera.Priority ? freeLookCamera : towerCamera;
       // isPhase1Intro = true;
    }

    private void Update()
    {
        if (introPlayed)
        {
            StartCoroutine(StartIntro());
        }

        if(triggerLevel2Intro)
        {
            triggerLevel2Intro = false; // Reset toggle
            StartCoroutine(HandleLevel2Cameras(Level2IntroCam));
        }
        // LEVEL 2 - Defeat Camera Triggers
        if (triggerDefeatCam1)
        {
            triggerDefeatCam1 = false; // Reset toggle
            StartCoroutine(HandleLevel2Cameras(DefeatCam1));
        }

        if (triggerDefeatCam2)
        {
            triggerDefeatCam2 = false; // Reset toggle
            StartCoroutine(HandleLevel2Cameras(DefeatCam2));
        }
        
      /*  if (Boss3Intro)
        {
            StartCoroutine(StartBoss3PanIn());
        }*/
    }

    private IEnumerator StartIntro()
    {
        introPlayed = false;

        playerMovementScript.enabled = false;

        lastActiveCamera = freeLookCamera.Priority > towerCamera.Priority ? freeLookCamera : towerCamera;

        introCameraObj.SetActive(true);

        introCamera.Priority = 100;

        yield return new WaitForSeconds(0.5f);

        while (!IsCameraInPosition(introCameraTransform))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        if (isPhase1Intro) 
        {
            AudioManager.PlayBossIntro();
        }
        else if (isPhase2Intro)
        {
            AudioManager.PlayBossPhase();
        }
        //introUI.SetActive(true);
        //  audioSource.Play();
      //  AudioManager.PlayBossIntro();
        

        yield return new WaitForSeconds(4f);
        EndIntro();
    }

    //----------------------------------- LEVEL 2-------------------------------------//
    
    private IEnumerator HandleLevel2Cameras(GameObject camObject)
    {
        if (camObject == null) yield break;

        CinemachineCamera cinemachineCam = camObject.GetComponent<CinemachineCamera>();
        
        // Enable object and raise priority
        camObject.SetActive(true);
        playerMovementScript.enabled = false;
        if (cinemachineCam != null) cinemachineCam.Priority = 100;

        // Wait designated time
        yield return new WaitForSeconds(defeatCameraDuration);

        // Lower priority back to 1 and disable
        if (cinemachineCam != null) cinemachineCam.Priority = 1;
        camObject.SetActive(false);
        playerMovementScript.enabled = true;
    }

    //----------------------------------- LEVEL 3-------------------------------------//
    public IEnumerator StartBoss3PanIn()
    {
        Boss3Intro = false;
        playerMovementScript.enabled = false;
        lastActiveCamera = freeLookCamera.Priority > towerCamera.Priority ? freeLookCamera : towerCamera;
        introCameraObj.SetActive(true);
        introCamera.Priority = 100;
        yield return new WaitForSeconds(0.5f);
        while (!IsCameraInPosition(introCameraTransform))
        {
            yield return null;
        }
    //    yield return new WaitForSeconds(CameraTransitionDuration);
      //  EndRockyPanIn();
    }

    public void EndRockyPanIn()
    {
        introCameraObj.SetActive(false);
        IsRockyPhase1 = false;
        IsRockyPhase2 = false;
        IsRockyPhase3 = false;
        introCamera.Priority = 1;
        playerMovementScript.enabled = true;
    }
    private void EndIntro()
    {
        //introUI.SetActive(false);
        introCameraObj.SetActive(false);

        // set the states to false to prevent the intro from playing again
        isPhase1Intro = false;
        isPhase2Intro = false;
        introCamera.Priority = 1;

        playerMovementScript.enabled = true;

        if (bossTrigger2 == true)
        {
            bossTrigger2.SetActive(false);
        }
    }

    private bool IsCameraInPosition(Transform targetTransform)
    {
        Transform mainCam = Camera.main.transform;

        float positionDiff = Vector3.Distance(mainCam.position, targetTransform.position);
        float rotationDiff = Quaternion.Angle(mainCam.rotation, targetTransform.rotation);

        return positionDiff < positionThreshold && rotationDiff < rotationThreshold;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
