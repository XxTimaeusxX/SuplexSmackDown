using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossShockwave : MonoBehaviour
{
    public float growDuration = 2f;
    public float activeDuration = 2f;
    private Vector3 targetScale;
    private Vector3 initialScale;
    public float growSizeX;
    public float growSizeY;
    public float growSizeZ;

    void Start()
    {
        targetScale = new Vector3(growSizeX, growSizeY, growSizeZ);
        initialScale = transform.localScale;
        StartCoroutine(ScaleOverTime(growDuration, targetScale));
    }

    void Update()
    {
        activeDuration -= Time.deltaTime;
        if (activeDuration <= 0)
        {
            Destroy(gameObject);
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

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            collision.gameObject.GetComponent<PlayerHealth>().iFrames = true;
        }
    }
}
