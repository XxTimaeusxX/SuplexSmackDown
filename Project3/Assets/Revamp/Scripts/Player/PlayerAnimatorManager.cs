using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    PlayerManager player;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        player.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        player.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }
}
