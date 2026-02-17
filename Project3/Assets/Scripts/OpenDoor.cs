using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public float moveSpeed;
    public DoorManager doorManager;
    public float openTime;
    public float closeTime;
    public bool left;
    public bool right;

    void Update()
    {
        if (left)
        {
            if (doorManager.open)
            {
                openTime -= Time.deltaTime;
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            if (doorManager.close && doorManager.ableToClose)
            {
                closeTime -= Time.deltaTime;
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
        }
        if (right)
        {
            if (doorManager.open)
            {
                openTime -= Time.deltaTime;
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
            if (doorManager.close && doorManager.ableToClose)
            {
                closeTime -= Time.deltaTime;
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
        }
        if (openTime <= 0)
        {
            doorManager.open = false;
        }
        if (closeTime <= 0)
        {
            doorManager.close = false;
            doorManager.ableToClose = false;
        }
    }
}
