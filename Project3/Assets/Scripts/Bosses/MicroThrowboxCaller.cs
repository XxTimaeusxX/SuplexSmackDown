using UnityEngine;

public class MicroThrowboxCaller : MonoBehaviour
{
    public MicroBoss microBoss;
    private void OnTriggerEnter(Collider macro)
    {
        if (macro.gameObject.name.Equals("Macro"))
        {
           StartCoroutine(microBoss.ThrowMicro());
        }
    }
}
