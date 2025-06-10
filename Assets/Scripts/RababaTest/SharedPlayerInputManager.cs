using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Extended PlayerInputManager that allows sharing a keyboard between players.
/// </summary>
public class SharedPlayerInputManager : PlayerInputManager
{
    private int _playerIndex;
    
    /// <summary>
    /// Names of the control schemes that will be used for players sharing the keyboard.
    /// </summary>
    private readonly string[] _controlSchemes = new[]
    {
        "WASD", "Arrows"
    };

    /// <summary>
    /// Set player's control scheme based on their index
    /// </summary>
    /// <param name="p">Player Input</param>
    private void RebindPlayer(PlayerInput p)
    {
        p.SwitchCurrentControlScheme(_controlSchemes[_playerIndex], Keyboard.current);
        _playerIndex++;
    }
    
    /// <summary>
    /// Replacement for <see cref="PlayerInputManager.JoinPlayerFromActionIfNotAlreadyJoined"/>
    /// Allowing <see cref="SharedPlayerInputManager"/> to share keyboard, which splits keys into seperate control schemes/>
    /// </summary>
    /// param name="context">Input action's callback context data</param>
    protected override void JoinPlayerFromActionIfNotAlreadyJoined(InputAction.CallbackContext context)
    {
        if (!CheckIfPlayerCanJoin())
        {
            Debug.Log("Player cannot join");
            return;
        }

        var device = context.control.device;
        
        // We want to sharing keyboard device. Hence dont need to share it
        if (device is not Keyboard)
        {
            if (PlayerInput.FindFirstPairedToDevice(device) != null)
            {
                return;
            }
        }

        var p = JoinPlayer(pairWithDevice: device);
        Debug.Log("Joined player " + p.playerIndex + " with device " + device.name + "");
        // We also want players sharing the the keyboard have a unique control scheme
        if (device is Keyboard)
        {
            RebindPlayer(p);
        }
    }
}
