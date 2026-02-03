using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// ~ Ovi. 



// TODO: Develop the principle mechnics

// TODO: jump slam attack, dash knock up, 

public enum RockyRhodesStates
{
    Regular,
    BoulderEruption,
    BullRock,
    Dead,
}

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RockyRhodes : EnemyBase
{
    [Header("Rocky Rhodes Settings")]
    public RockyRhodesStates CurrentRockyState;
    private List<RockyRhodesStates> _RandomSelection= new List<RockyRhodesStates>
    {
        RockyRhodesStates.BoulderEruption,
        RockyRhodesStates.BullRock,
    };

    public float AbilityCooldown = 5f;
    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    private void OnValidate()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  new  void Start()
    {
        CurrentRockyState = RockyRhodesStates.Regular;
    }

    // Update is called once per frame
    public override void  Update()
    {
        base.Update();
        CheckState();
    }

    public void CheckState()
    {
        switch(CurrentRockyState)
        {
            case RockyRhodesStates.Regular:
                Regular();
                break;
            case RockyRhodesStates.BoulderEruption:
                BoulderEruption();
                break;
            case RockyRhodesStates.BullRock:
                BullRock();
                break;
        }
     }

    public void Regular()
    {
       
        _abilityTimer += Time.deltaTime;
        Debug.Log("Regular State Active");
        
        if (_abilityTimer >= AbilityCooldown)
        {
            ShuffleAbilities();
           
        }
       
       
    }
    public void BoulderEruption()
    {

        Debug.Log("Boulder Eruption Activated");
        _abilityTimer += Time.deltaTime;
        if (_abilityTimer >= AbilityCooldown)
        {
            CurrentRockyState = RockyRhodesStates.Regular;
            _abilityTimer = 0f;
        }
    }

    public void BullRock()
    {
        Debug.Log("Bull Rock Activated");
        _abilityTimer += Time.deltaTime;
        if (_abilityTimer >= AbilityCooldown)
        {            
            CurrentRockyState = RockyRhodesStates.Regular;
            _abilityTimer = 0f;
        }

    }
    public void ShuffleAbilities()
    { 
        int randomIndex = Random.Range(0, _RandomSelection.Count);
        CurrentRockyState = _RandomSelection[randomIndex];
        _abilityTimer = 0f;

    }
    public void Dead()
    {
               Debug.Log("Rocky Rhodes is Dead");
    }
}
