using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Enemy : OGEnemyBase
{
    // Shoal-specific audio edge trigger state
    private bool _shoalWasInChaseRange = false;
    public override void Update()
    {
        base.Update();
        bool grounded = IsEnemyGrounded();
        if (Target != null)
        {
            m_Distance = Vector3.Distance(Target.transform.position, transform.position);
            bool inChaseRange = m_Distance <= chaseRange;
            if (inChaseRange && !_shoalWasInChaseRange)
            {
                // Entered chase range: play Shoal "detected" sound
                AudioManager.PlayShoalIdle();
            }

            _shoalWasInChaseRange = inChaseRange;
        }
        else _shoalWasInChaseRange = false;
    }
}
