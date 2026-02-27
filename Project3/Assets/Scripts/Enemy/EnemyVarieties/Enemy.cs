
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : EnemyBase
{
    private bool _shoalWasInChaseRange = false;
    
    public override void Update()
    {
        base.Update();  // Let base class handle everything
        
        // No additional logic needed - base handles it all
        // If you need enemy-specific behavior, add it here
    }
    
    public override void ChasePlayer()
    {
        base.ChasePlayer();
        
        // Enemy-specific: Audio edge trigger
        if (Target != null)
        {
            bool inChaseRange = m_Distance <= chaseRange;
            
            if (inChaseRange && !_shoalWasInChaseRange)
            {
                AudioManager.PlayShoalIdle();
            }
            
            _shoalWasInChaseRange = inChaseRange;
        }
        else
        {
            _shoalWasInChaseRange = false;
        }
    }
}
