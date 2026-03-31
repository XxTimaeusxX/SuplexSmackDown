using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class RockyRhodesAttacks : MonoBehaviour
{
    RockyRhodesManager manager;
    Transform chosenPoint;
    public bool collided;

    private void Awake()
    {
        manager = GetComponent<RockyRhodesManager>();
    }

    #region Rope Rush
    public void StartRopeRush()
    {
        if (manager.ropeRush && manager.canPerformAction)
        {
            ReadyRopeRush();
        }
        ReactivateRopeRush();
    }

    private void ReadyRopeRush()
    {
        manager.canPerformAction = false;
        ChooseRandomRopeRushPoint();
        Vector3 direction = (chosenPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + direction * (Vector3.Distance(transform.position, chosenPoint.position) * manager.moveSpeed);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(chosenPoint.position.x, transform.position.y, chosenPoint.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(MoveToRopeRushPoint());
    }

    private IEnumerator MoveToRopeRushPoint()
    {
        while (Vector3.Distance(transform.position, chosenPoint.position) > manager.interactionDistance)
        {
            float step = manager.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, chosenPoint.position, step);
            yield return null;
        }
        RopeRush();
    }

    private void RopeRush()
    {
        collided = false;
        Vector3 directionToPlayer = (manager.player.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, manager.player.transform.position) * manager.ropeRushForce);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(manager.player.transform.position.x, transform.position.y, manager.player.transform.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(RopeRushCoroutine(targetPosition));
    }

    private IEnumerator RopeRushCoroutine(Vector3 target)
    {
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        while (!collided)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, manager.ropeRushForce * Time.deltaTime);
            yield return null;
        }
    }

    private void ChooseRandomRopeRushPoint()
    {
        manager.ropeRush = false;
        if (manager.ropeRushStartPoints == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.ropeRushStartPoints.Length);
        chosenPoint = manager.ropeRushStartPoints[randomPoint];
    }

    private void ReactivateRopeRush()
    {
        if (collided && manager.arena1)
        {
            manager.numberOfRopeRusheCharges -= 1;
            manager.canPerformAction = true;
        }
        if (manager.numberOfRopeRusheCharges > 0 && manager.arena1 && collided)
        {
            manager.ropeRush = true;
        }
    }
    #endregion

    #region Cannonball
    private void Cannball()
    {

    }
    #endregion

    #region Enhanced Rope Rush
    public void StartEnhancedRopeRush()
    {
        if (manager.enhancedRopeRush && manager.canPerformAction)
        {
            ReadyEnhancedRopeRush();
        }
        ReactivateEnhancedRopeRush();
    }

    private void ReadyEnhancedRopeRush()
    {
        manager.canPerformAction = false;
        ChooseRandomEnhancedRopeRushPoint();
        Vector3 direction = (chosenPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + direction * (Vector3.Distance(transform.position, chosenPoint.position) * manager.moveSpeed);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(chosenPoint.position.x, transform.position.y, chosenPoint.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(MoveToEnhancedRopeRushPoint());
    }

    private IEnumerator MoveToEnhancedRopeRushPoint()
    {
        while (Vector3.Distance(transform.position, chosenPoint.position) > manager.interactionDistance)
        {
            float step = manager.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, chosenPoint.position, step);
            yield return null;
        }
        EnhancedRopeRush();
    }

    private void EnhancedRopeRush()
    {
        collided = false;
        Vector3 directionToPlayer = (manager.player.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, manager.player.transform.position) * manager.enhancedRopeRushForce);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(manager.player.transform.position.x, transform.position.y, manager.player.transform.position.z);
        transform.LookAt(targetLookAt);
        StartCoroutine(EnhancedRopeRushCoroutine(targetPosition));
    }

    private IEnumerator EnhancedRopeRushCoroutine(Vector3 target)
    {
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        while (!collided)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, manager.enhancedRopeRushForce * Time.deltaTime);
            yield return null;
        }
    }

    private void ChooseRandomEnhancedRopeRushPoint()
    {
        manager.enhancedRopeRush = false;
        if (manager.enhancedRopeRushStartPoints == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.enhancedRopeRushStartPoints.Length);
        chosenPoint = manager.enhancedRopeRushStartPoints[randomPoint];
    }

    private void ReactivateEnhancedRopeRush()
    {
        if (collided && manager.arena3)
        {
            manager.canPerformAction = true;
        }
        if (manager.numberOfEnhancedRopeRusheCharges > 0 && manager.arena3 && collided)
        {
            manager.enhancedRopeRush = true;
        }
    }
    #endregion

    private IEnumerator ReEnableCanPerformAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        manager.canPerformAction = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rope")) {
            collided = true;
            if (manager.arena3 && manager.numberOfEnhancedRopeRusheCharges > 0)
            {
                manager.numberOfEnhancedRopeRusheCharges -= 1;
            }
        }
    }
}