using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPush : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction pushAction;
    public GameObject push;
    public float pushForce;
    public float pushDuration;
    public float pushCoolDown;
    public float targetPushCoolDown;
    public float pushTime;
    public float dashDuration;
    public bool isPushing = false;

    void Start()
    {
        pushAction = playerInput.actions.FindAction("Push");
    }

    void Update()
    {
        Shockwave();
        if (pushCoolDown > 0f && !isPushing)
        {
            pushCoolDown -= 0.1f;
        }
        if (pushCoolDown < 0f)
        {
            pushCoolDown = 0f;
        }
    }

    public void Shockwave()
    {
        if (pushAction.WasPressedThisFrame())
        {
            pushCoolDown = targetPushCoolDown;
            isPushing = true;
            pushTime = pushDuration;
            push.SetActive(true);
        }
        if (isPushing)
        {
            if (pushTime > 0)
            {
                pushTime -= Time.deltaTime;
            }
            else
            {
                isPushing = false;
                push.SetActive(false);
            }
        }
    }
}
