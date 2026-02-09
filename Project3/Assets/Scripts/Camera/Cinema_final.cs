using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Audio;


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
    public AudioClip introSound;
    public bool audio1;
    public bool audio2;
    public bool audio3;

    [Header("Camera Transition Settings")]
    public float transitionSpeed = 2f;
    private Transform introCameraTransform;
    public float positionThreshold = 1f;
    public float rotationThreshold = 2f;

    public bool isMainMenuIntro;
    private CinemachineCamera lastActiveCamera;
    private AudioSource audioSource;
    public GameObject shoalHealth;
    public GameObject bossTrigger2;

    private void Start()
    {
        playerMovementScript = player.GetComponent<PlayerMovement>();
        introCameraTransform = introCamera.transform;
        audioSource = GetComponent<AudioSource>();

        if (!isMainMenuIntro)
            introCameraObj.SetActive(false);

        lastActiveCamera = freeLookCamera.Priority > towerCamera.Priority ? freeLookCamera : towerCamera;
    }

    private void Update()
    {
        if (introPlayed)
        {
            StartCoroutine(StartIntro());
        }
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

        //introUI.SetActive(true);
        audioSource.Play();
        

        yield return new WaitForSeconds(4f);
        EndIntro();
    }

    private void EndIntro()
    {
        //introUI.SetActive(false);
        introCameraObj.SetActive(false);

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
