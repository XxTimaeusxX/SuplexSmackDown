using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerDetector : MonoBehaviour
{
    public GameObject keyboardControls;
    public GameObject controllerControls;

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        CheckConnectedDevices();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Reconnected || change == InputDeviceChange.Disconnected)
        {
            CheckConnectedDevices();
        }
    }

    private void CheckConnectedDevices()
    {
        bool gamepadConnected = Gamepad.current != null;
        if (gamepadConnected)
        {
            controllerControls.SetActive(true);
            keyboardControls.SetActive(false);
        }
        else
        {
            controllerControls.SetActive(false);
            keyboardControls.SetActive(true);
        }
    }
}