using UnityEngine;

public class RopeRushHitbox : MonoBehaviour
{
    public RockyRhodesManager manager;
    public PlayerHealth health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == manager.player)
        {
            health.TakeDamage();
        }
    }
}
