using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shockwave : MonoBehaviour
{
    public float pushForce;

    public float growDuration = 2f;
    public float activeDuration = 2f;
    public Vector3 targetScale;
    private Vector3 initialScale;
    public float growSize;

    

    private void Start()
    {
        targetScale = new Vector3(growSize, growSize, growSize);
        initialScale = transform.localScale;
        StartCoroutine(ScaleOverTime(growDuration, targetScale));
    }

    private void Update()
    {
        activeDuration -= Time.deltaTime;
        if (activeDuration <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
        
    }

    IEnumerator ScaleOverTime(float duration, Vector3 endScale)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
    }
}
