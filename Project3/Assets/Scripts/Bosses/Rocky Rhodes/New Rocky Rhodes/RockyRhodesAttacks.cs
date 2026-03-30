using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class RockyRhodesAttacks : MonoBehaviour
{
    RockyRhodesManager manager;
    Transform chosenPoint;

    private void Awake()
    {
        manager = GetComponent<RockyRhodesManager>();
    }

    #region Rope Rush
    public void StartRopeRush()
    {
        if (manager.ropeRush)
        {
            ReadyRopeRush();
        }
    }

    private void ReadyRopeRush()
    {
        ChooseRandomPoint();
        Vector3 direction = (chosenPoint.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + direction * (Vector3.Distance(transform.position, chosenPoint.position) * manager.moveSpeed);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(chosenPoint.position.x, transform.position.y, chosenPoint.position.z);
        transform.LookAt(targetLookAt);
        if (!manager.canPerformAction)
        {
            return;
        }
        StartCoroutine(MoveToPoint());
    }

    private IEnumerator MoveToPoint()
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
        Vector3 directionToPlayer = (manager.player.transform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, manager.player.transform.position) * manager.rushForce);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(manager.player.transform.position.x, transform.position.y, manager.player.transform.position.z);
        transform.LookAt(targetLookAt);
        if (!manager.canPerformAction)
        {
            return;
        }
        StartCoroutine(RushCoroutine(targetPosition));
    }

    private IEnumerator RushCoroutine(Vector3 target)
    {
        manager.ropeRush = false;
        manager.canPerformAction = false;
        StartCoroutine(ReEnableCanPerformAction(manager.rushCooldown));
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        while (Time.time < startTime + manager.chargeDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, manager.rushForce * Time.deltaTime);
            yield return null;
        }
    }

    private void ChooseRandomPoint()
    {
        manager.ropeRush = false;
        if (manager.ropeRushStartPoints == null)
        {
            return;
        }

        int randomPoint = Random.Range(0, manager.ropeRushStartPoints.Length);
        chosenPoint = manager.ropeRushStartPoints[randomPoint];
    }
    #endregion

    private void Cannball()
    {

    }

    private void RopeRushEnhanced()
    {

    }

    private IEnumerator ReEnableCanPerformAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        manager.canPerformAction = true;
    }
}