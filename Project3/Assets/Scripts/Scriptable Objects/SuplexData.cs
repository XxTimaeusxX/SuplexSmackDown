using UnityEngine;

[CreateAssetMenu(menuName = "Suplex/SuplexData")]
public class SuplexData : ScriptableObject
{
    public SuplexAbilities ability;

    public AnimationCurve verticalCurve;   // curve0
    public AnimationCurve forwardCurve;    // curve1

    public float duration = 1f;
}
