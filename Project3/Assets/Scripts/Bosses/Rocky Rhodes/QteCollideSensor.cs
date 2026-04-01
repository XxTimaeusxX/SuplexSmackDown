using UnityEngine;

public class QteCollideSensor : MonoBehaviour
{
    private string _playerTag = "Player";
    private int _playerOverlapCount = 0;

    // Drag the main RockyRhodes script (or QTESystem) into this slot in the inspector
    public QTESystem qteSystemScript;
    public Collider QTETriggerCollider;
    public RhockyAbilities rockyAbilitiesScript;
    void Start()
    {
        QTETriggerCollider.enabled = false;
       
    }
    public void ResetOverlap()
    {
        _playerOverlapCount = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            PlayerDash playerDash = other.GetComponent<PlayerDash>();
            if(playerDash.isDashing)
            {
                _playerOverlapCount++;
                if (_playerOverlapCount == 1)
                {
                   // Debug.Log("Player entered Boulder Eruption area (Child Trigger)!");
                    
                    qteSystemScript.EnableQuickTimeEvent = true;
                    qteSystemScript.StartQTE();
                    rockyAbilitiesScript.CheckState(RockyRhodesStates.QTEMode);
                }
            }
           
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            PlayerDash playerDash = other.GetComponent<PlayerDash>();

            if (playerDash != null && playerDash.isDashing && _playerOverlapCount == 0)
            {
                _playerOverlapCount++;
             //   Debug.Log("Player activated dash while inside trigger! Triggering QTE.");
                rockyAbilitiesScript.CheckState(RockyRhodesStates.QTEMode);
                qteSystemScript.EnableQuickTimeEvent = true;
                qteSystemScript.StartQTE();
            }
        }
    }
}
