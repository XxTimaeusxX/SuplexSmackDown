using System;
using UnityEngine;
[Serializable]
public struct FaceExpression
{
    public PlayerState state;
    public Material eyesMaterial;
    public Material mouthMaterial;
}

public class PlayerFaceAnimations : MonoBehaviour
{
  
    [Header("References")]
    public PlayerMovement playerMovement;
    public MeshRenderer eyesRenderer;
    public MeshRenderer mouthRenderer;

    [Header("Expressions Setup")]
    public FaceExpression[] expressions;

    private PlayerState _currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentState = playerMovement.CurrentState;
        UpdateFaceExpression(_currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement != null && playerMovement.CurrentState != _currentState)
        {
            _currentState = playerMovement.CurrentState;
            UpdateFaceExpression(_currentState);
        }
    }
    private void UpdateFaceExpression(PlayerState newState)
    {
        foreach (var expression in expressions)
        {
            if (expression.state == newState)
            {
                    eyesRenderer.material = expression.eyesMaterial;
                    mouthRenderer.material = expression.mouthMaterial;
                break; // Stop searching once we found the matching state
            }
        }
    }

}
