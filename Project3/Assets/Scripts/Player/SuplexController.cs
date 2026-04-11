using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Istvan W.

// TODO: Add collision checks to prevent suplexing into/through walls/obstacles

/// <summary>
/// Handles all logic for grabbing, holding, and suplexing enemies.
/// </summary>
public class SuplexController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public RageMeter rageMeter;
    private PlayerThrow playerThrow;
    private MovementController movementController;
    private MovementConfig movementConfig;
    public SuplexConfig suplexConfig;
    private PlayerDash playerDash;
    
    [Header("Suplex Configurations")]
    private SuplexAbilities currentSuplex = SuplexAbilities.None; // Which suplex is being performed
    private SuplexData activeSuplex;

    [Header("Enemy/Object Carrying")]
    public ICarriable carriedObject;
    //public GameObject carriedObject = null;        // The GameObject of the carriable/object being carried
    //public MonoBehaviour carriedObjectScript = null;  // The script of the carriable/object being carried (if needed for specific interactions)

    private EnemyBase carriedEnemyBase = null;      // The OGEnemyBase script of the carried carriable
    //private Transform carriedEnemy;          // The transform of the carriable that's currently being carried
    private MonoBehaviour carriableMono;              // Testin for suplexable objects
    public GameObject suplexedObject; 

    public bool isSuplexing = false;         // True if a suplex is in progress
    public bool suplexInputLocked = false;


    private Vector3 suplexHorizontalVelocity;

    private Coroutine suplexRoutine;

    //[Header("In Testing")]


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
        movementController = GetComponent<MovementController>();
        movementConfig = GetComponent<MovementConfig>();
        suplexConfig = GetComponent<SuplexConfig>();
        playerThrow = GetComponent<PlayerThrow>();
        rageMeter = GetComponent<RageMeter>();
    }


    public void StartSuplex(MonoBehaviour target)   // Allows me to change the suplexable object type by using a simple check
    {
        if (target is ICarriable carriable)
        {
            isSuplexing = true;
            carriedObject = carriable;

            if (target is EnemyBase enemy)
            {
                //carriedEnemy = enemy.transform;
                carriedEnemyBase = enemy;

                //carriedEnemy.SetParent(suplexConfig.carryPoint);
                //carriedEnemy.localPosition = Vector3.zero;
            }
            else
            {
                carriableMono = target;
            }

            //Debug.Log("StartSuplex called. Setting carriedEnemyBase to: " + carriable.name);
        }

        if (carriedObject != null)
        {
            playerThrow.carryingObject = true;
            playerThrow.readyToThrow = true;
        }

        StartCoroutine(WaitForSuplexInput());        // Wait for player to choose which suplex to perform

    }

    public void ReleaseEnemy(Vector3 throwForce)
    {
        if (carriedObject != null)
        {
            StopAllCoroutines();


            carriedObject.ExitCarriedState(throwForce);

            //carriedEnemy = null;
            carriedEnemyBase = null;
            carriableMono = null;
            carriedObject = null;

            movementController.overrideVerticalMotion = false;
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
                        movementController.rightBumper.IsPressed() && 
                        !movementController.aimInputLock
                ),

            new SuplexInput(
                SuplexAbilities.Long,
                () =>   playerDash.isDashing &&
                        Time.time - playerDash.dashStartTime < suplexConfig.longSuplexBuffer &&
                        movementController.jumpAction.IsPressed() &&
                        !movementController.aimInputLock
                ),           
            new SuplexInput(
                SuplexAbilities.Rainbow,
                () =>   movementController.jumpAction.WasPressedThisFrame() &&
                        !movementController.aimInputLock
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
        //Debug.Log("Chosen Suplex: " + currentSuplex);
        PerformSuplex(currentSuplex);
    }

    void PerformSuplex(SuplexAbilities type)
    {
        activeSuplex = suplexConfig.suplexes.Find(s => s.ability == type);
        SuplexConditionHandler(type);

        GameObject carriedGO = ((MonoBehaviour)carriedObject).gameObject;
        suplexedObject = carriedGO;
        Debug.Log("Releasing object: " + carriedGO.name);

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

        CameraShakeManager.Instance.SuplexCameraShake(suplexConfig.impulseSource);
        if (suplexConfig.shockwave != null && suplexConfig.rageBar != null)
        {
            if (suplexConfig.rageBar.value >= 1)
            {
                Instantiate(suplexConfig.rageShockwave, controller.transform.position, controller.transform.rotation, controller.transform);
                rageMeter.rageIncrease = false;
                suplexConfig.rageBar.value = 0;
                suplexConfig.suplexBar.sprite = suplexConfig.suplexBarFull;
            }
            else
            {
                Instantiate(suplexConfig.shockwave, controller.transform.position, controller.transform.rotation, controller.transform);
            }
        }
        AudioManager.PlaySuplexSlam();
        ReleaseEnemy(Vector3.zero);
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
        if (carriedObject == null)
            return;

        CancelSuplexEarly();

        movementController.controller.Move(suplexHorizontalVelocity * Time.deltaTime);

        float objHeight;
        Vector3 dropPos;
        if (carriedObject is EnemyBase)
        {            
            objHeight = EnemyBase.GetHeight(carriedEnemyBase.mainCollider);
            dropPos = transform.position + Vector3.down * objHeight;
        }
        else
        {
            objHeight = ((MonoBehaviour)carriedObject).GetComponent<Collider>().bounds.size.y;
            dropPos = transform.position + Vector3.down * objHeight;
        }

            if (carriedEnemyBase != null)
                carriedEnemyBase.transform.position = dropPos;
            if (carriableMono != null)
                carriableMono.transform.position = dropPos;



            // carriedEnemyBase.ApplyDownwardForce(64f);    // Apply downward force to ensure they hit the ground

            ReleaseEnemy(Vector3.down * 64f);
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