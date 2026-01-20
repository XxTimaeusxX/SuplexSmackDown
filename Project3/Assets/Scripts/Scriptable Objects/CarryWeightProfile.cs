using UnityEngine;

/// <summary>
/// Scriptable Objects should be stored inside the "Resources/ScriptableObjects" folder for easy access
/// </summary>

// ~Istvan W

// Last Edited: 12/15/2025 by Istvan W

[CreateAssetMenu(fileName = "CarryWeightProfile", menuName = "Movement/Carry Weight Profile")]
public class CarryWeightProfile : ScriptableObject
{
    [Header("Profile Settings")]
    public string displayName;       // e.g. "Light Object", "Heavy Crate"
    public float speedMultiplier;    // e.g. 1.0f (no penalty), 0.8f (20% slower), 0.5f (50% slower)
    public float rotationMultiplier; // Higher values mean slower rotation

    // Optional: expand later

}
