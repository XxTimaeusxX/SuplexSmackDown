using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterController characterController;
    PlayerMovementManager playerMovementManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        playerMovementManager = GetComponent<PlayerMovementManager>();
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
