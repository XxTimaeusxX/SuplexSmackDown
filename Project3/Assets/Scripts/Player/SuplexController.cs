using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Last Edited: 2/11/2026 by Istvan W.

/// <summary>
/// Handles all logic for grabbing, holding, and suplexing enemies.
/// </summary>
public class SuplexController : MonoBehaviour
{
    [Header("References")]
    public EnemyGrabHandler grabHandler;
    private PlayerThrow playerThrow;
    private MovementController movementController;
    private MovementConfig movementConfig;
    public SuplexConfig suplexConfig;
    private PlayerDash playerDash;

    [Header("Suplex Configurations")]
    private SuplexAbilities currentSuplex = SuplexAbilities.None; // Which suplex is being performed
    private SuplexData activeSuplex;

    [Header("Enemy/Object Carrying")]
    public GameObject carriedObject = null;        // The GameObject of the enemy/object being carried

    private EnemyBase carriedEnemyBase = null;      // The EnemyBase script of the carried enemy
    private Transform carriedEnemy;          // The transform of the enemy that's currently being carried
    private ObjectTest objectTest;
    private Transform objectTestTransform;
 
    public bool isSuplexing = false;         // True if a suplex is in progress
    public bool suplexInputLocked = false;


    private Vector3 suplexHorizontalVelocity;

    private Coroutine suplexRoutine;

    //[Header("Testing")]


    private void Awake()
    {
        playerDash = GetComponent<PlayerDash>();
        movementController = GetComponent<MovementController>();
        movementConfig = GetComponent<MovementConfig>();
        suplexConfig = GetComponent<SuplexConfig>();
        playerThrow = GetComponent<PlayerThrow>();
    }

    public void StartSuplex(MonoBehaviour target)   // Allows me to change the suplexable object type by using a simple check
    {
        if (target is EnemyBase enemy)
        {
            carriedEnemyBase = enemy;
            isSuplexing = true;

            carriedEnemy = enemy.transform;
            carriedEnemy.SetParent(suplexConfig.carryPoint);
            carriedEnemy.localPosition = Vector3.zero;

            carriedObject = carriedEnemy.gameObject;

            //Debug.Log("StartSuplex called. Setting carriedEnemyBase to: " + enemy.name);
        }
        //if (target is ObjectTest objectTest)    // MARK: Template for object suplexing
        //{

        //}

        if (carriedObject != null)
        {
            playerThrow.carryingObject = true;
            playerThrow.readyToThrow = true;
        }

        StartCoroutine(WaitForSuplexInput());        // Wait for player to choose which suplex to perform

    }

    public void ReleaseEnemy()
    {
        if (carriedObject != null)
        {
            StopAllCoroutines();

            carriedEnemy.SetParent(null);

            carriedEnemyBase.ExitCarriedState(Vector3.zero);

            carriedEnemy = null;
            carriedEnemyBase = null;
            carriedObject = null;
            playerThrow.carryingObject = false;
            playerThrow.readyToThrow = false;
            isSuplexing = false;
        }
    }

    /// <summary>
    /// Waits for the player to press a suplex input, then starts the chosen suplex.
    /// </summary>
    public IEnumerator WaitForSuplexInput()
    {
        currentSuplex = SuplexAbilities.None;
        suplexInputLocked = false;

        //TODO: Redo the dictionary to match new Input System
        var suplexInputs = new[] 
        {
            new SuplexInput(
                SuplexAbilities.Super,
                () =>   movementController.leftBumper.IsPressed() && 
                        movementController.rightBumper.IsPressed()
                ),

            new SuplexInput(
                SuplexAbilities.Long,
                () =>   playerDash.isDashing &&
                        Time.time - playerDash.dashStartTime < suplexConfig.longSuplexBuffer &&
                        movementController.jumpAction.IsPressed()

                ),           
            new SuplexInput(
                SuplexAbilities.Rainbow,
                () =>   movementController.jumpAction.WasPressedThisFrame()
                )

        };

        while (currentSuplex == SuplexAbilities.None)
        {
            foreach (var input in suplexInputs)
            {
                if (input.onPress())
                {
                    //if (input.ability == SuplexAbilities.Super && !SuplexConditionHandler())
                    //{
                    //    Debug.Log("Not enough power for Super Suplex!");
                    //    continue; // Ignore the attempt
                    //}
                    currentSuplex = input.ability;
                    break;
                }
            }

            suplexInputLocked = true;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Chosen Suplex: " + currentSuplex);
        PerformSuplex(currentSuplex);
    }

    void PerformSuplex(SuplexAbilities type)
    {
        activeSuplex = suplexConfig.suplexes.Find(s => s.ability == type);
        SuplexConditionHandler(type);
        suplexRoutine = StartCoroutine(SuplexRoutine(activeSuplex));
    }
    IEnumerator SuplexRoutine(SuplexData data)
    {
        movementController.move = Vector3.zero;
        movementController.overrideVerticalMotion = true;
        suplexInputLocked = false;

        Vector3 lockedForward = movementController.transform.forward;
        lockedForward.y = 0;
        lockedForward.Normalize();

        float t = 0f;

        float verticalEndTime = data.verticalCurve.keys[^1].time;
        float forwardEndTime = data.forwardCurve.keys[^1].time;

        while (t < data.duration)
        {
            float normalized = t / data.duration;

            float curveTimeV = normalized * verticalEndTime;
            float curveTimeF = normalized * forwardEndTime;

            float velocityY = data.verticalCurve.Evaluate(normalized);
            float forwardSpeed = data.forwardCurve.Evaluate(normalized);

            //Vector3 forward = movementController.transform.forward;
            //forward.y = 0f;
            //forward.Normalize();

            //suplexHorizontalVelocity = forward * forwardSpeed;

            Vector3 delta =
                lockedForward * forwardSpeed * Time.deltaTime +
                Vector3.up * velocityY * Time.deltaTime;

            movementController.controller.Move(delta);

            t += Time.deltaTime;
            yield return null;
        }

        float finalVerticalVel = data.verticalCurve.Evaluate(verticalEndTime);
        float finalForwardVel = data.forwardCurve.Evaluate(forwardEndTime);

        Vector3 finalVelocity =
            lockedForward * finalForwardVel +
            Vector3.up * finalVerticalVel;

        Vector3 slamImpulse =
            lockedForward * data.slamForwardForce +
            Vector3.down * data.slamDownwardForce;

        movementController.overrideVerticalMotion = false;

        movementController.SetVelocity(finalVelocity);
        movementController.AddImpulse(slamImpulse);

        yield return new WaitUntil(() => movementController.isGrounded == true);

        ReleaseEnemy();

    }


    // On-Press Struct
    public struct SuplexInput
    {
        public SuplexAbilities ability;
        public Func<bool> onPress;

        public SuplexInput(SuplexAbilities ability, Func<bool> onPress)
        {
            this.ability = ability;
            this.onPress = onPress;
        }
    }

    public void JumpOff()
    {
        if (carriedEnemyBase == null)
            return;

        CancelSuplexEarly();

        movementController.controller.Move(suplexHorizontalVelocity * Time.deltaTime);

        float enemyHeight = EnemyBase.GetHeight(carriedEnemyBase.mainCollider);
        Vector3 dropPos = transform.position + Vector3.down * enemyHeight;
        carriedEnemyBase.transform.position = dropPos;

        carriedEnemyBase.ApplyDownwardForce(64f);    // Apply downward force to ensure they hit the ground

        ReleaseEnemy();
    }

    public void CancelSuplexEarly()
    {
        if (suplexRoutine != null)
            StopCoroutine(suplexRoutine);

        movementController.overrideVerticalMotion = false;
    }

    private void SuplexConditionHandler(SuplexAbilities type)
    {
        if (type == SuplexAbilities.Long)
        {
            playerDash.CancelDash();
        }
        if (type == SuplexAbilities.Super)
        {
            // If requirements aren't met, cancel suplex

            //Debug.Log("Not enough power for Super Suplex!");
            //CancelSuplexEarly();
        }
    }
}