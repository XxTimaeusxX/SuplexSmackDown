using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerHealth>().iFrames == false)
        {
            //Debug.Log("Enemy attack hitbox triggered on Player");
            // Here you would typically call a method on the Player to apply damage
             other.GetComponent<PlayerHealth>().TakeDamage();
            other.GetComponent<PlayerHealth>().iFrames = true;
        }
    }
}
