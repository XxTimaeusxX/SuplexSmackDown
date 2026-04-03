using System;
using UnityEngine;
[Serializable]
public struct MacroFaceExpression
{
    public MacroState state;
    public Material eyesMaterial;
    public Material mouthMaterial;
}

public class MacroFaceAnimations : MonoBehaviour
{
    [Header("References")]
    public MacroBoss macroBossScript;
    public SkinnedMeshRenderer eyesRenderer;
    public SkinnedMeshRenderer mouthRenderer;

    [Header("Expressions Setup")]
    public MacroFaceExpression[] expressions;
    private MacroState _currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentState = macroBossScript.CurrentMacroState;
        UpdateFaceExpression(_currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(macroBossScript != null && macroBossScript.CurrentMacroState != _currentState)
        {
            _currentState = macroBossScript.CurrentMacroState;
            UpdateFaceExpression(_currentState);
        }
    }
    private void UpdateFaceExpression(MacroState newState)
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
