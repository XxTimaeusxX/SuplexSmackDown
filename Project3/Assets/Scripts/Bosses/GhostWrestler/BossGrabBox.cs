using UnityEngine;

public class BossGrabBox : MonoBehaviour
{
    public BossGrabHandler grab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            grab.grabbed = true;
            gameObject.SetActive(false);
        }
    }
}
