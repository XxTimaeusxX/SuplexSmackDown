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
    public Transform player;
    public float pushCooldown;
    bool pushing = false;
    

    void Start()
    {
        pushAction = playerInput.actions.FindAction("Push");
    }

    void Update()
    {
        if (pushAction.WasPressedThisFrame() && pushing == false)
        {
            Instantiate(push, player.position, player.rotation, player);
            pushing = true;
        }
        if (pushing)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown <= 0)
        {
            pushing = false;
            pushCooldown = 2;
        }
    }
}
