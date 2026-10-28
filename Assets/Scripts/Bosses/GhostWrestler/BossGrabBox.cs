using UnityEngine;

public class BossGrabBox : MonoBehaviour
{
    public Level2BossManager grab;

    private void Update()
    {
        float activeTime = 1f;
        activeTime -= Time.deltaTime;
        if (activeTime <= 0)
        {
            activeTime = 1;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Grab");
            grab.grabBoxGrab = true;
            gameObject.SetActive(false);
        }
    }
}
