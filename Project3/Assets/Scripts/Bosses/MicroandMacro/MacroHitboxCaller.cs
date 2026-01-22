using UnityEngine;

public class MacroHitboxCaller : MonoBehaviour
{
    public MacroBoss macroBoss;
    public MicroBoss microBoss;
    private void OnTriggerEnter(Collider other)
    {
        // prefer a tag on Micro; fallback to name check
        if (other.gameObject.name == "Micro")
        {
            // Micro was hit during suplex
            Debug.Log("Micro is hit");
            if (macroBoss.damageHitbox != null) macroBoss.damageHitbox.enabled = false;
            microBoss.enemyHealth.value -= 1;
            // handle any damage/UI here
        }
    }
}
