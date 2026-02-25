using UnityEngine;

public class Level2BossManager : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;

    [Header("References")]
    public GameObject player;
    public GameObject intangibleBody;
    public GameObject solidBody;

    [Header("Strings")]
    public string flower;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(flower))
        {
            Debug.Log("Collide");
        }
    }
}
