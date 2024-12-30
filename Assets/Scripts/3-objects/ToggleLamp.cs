using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class LampSwitchManager : MonoBehaviour
{
    public Light switchLamp; // The lamp that acts as the switch
    public List<Light> otherLamps = new List<Light>(); // List of lamps controlled by the switch
    public float activationRange = 5f; // Range within which the switch can be toggled
    public Transform player; // Reference to the player

    private InputAction toggleLightAction;

    private void OnEnable()
    {
        // Create a new Input Action for the "E" key
        toggleLightAction = new InputAction("ToggleSwitch", binding: "<Keyboard>/e");
        toggleLightAction.Enable();

        // Bind the action to the ToggleSwitchLamp method
        toggleLightAction.performed += TryToggleSwitchLamp;
    }

    private void OnDisable()
    {
        toggleLightAction.Disable();
    }

    private void TryToggleSwitchLamp(InputAction.CallbackContext context)
    {
        // Check if the player is within range of the switch lamp
        if (switchLamp != null && player != null)
        {
            float distance = Vector3.Distance(player.position, switchLamp.transform.position);
            if (distance <= activationRange)
            {
                ToggleSwitchLamp();
            }
        }
    }

    private void ToggleSwitchLamp()
    {
        // Toggle the switch lamp's state
        switchLamp.enabled = !switchLamp.enabled;

        // Set all other lamps to match the state of the switch lamp
        foreach (Light lamp in otherLamps)
        {
            if (lamp != null)
            {
                lamp.enabled = switchLamp.enabled;
            }
        }
    }
}
