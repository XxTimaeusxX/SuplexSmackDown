using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerMovementManager playerMovementManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        playerMovementManager.HandleAllMovement();
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraActions();
    }
}
