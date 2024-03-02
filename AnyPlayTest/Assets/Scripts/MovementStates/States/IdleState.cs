using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
    }

    public override void UpdateState(MovementStateManager movement)
    {
        Vector2 movementInput = movement.PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        if (movementInput.magnitude > 0.1f)
        {
            if (movement.PlayerInput.PlayerInput.Sprint.triggered) movement.SwitchState(movement.RunState);
            else movement.SwitchState(movement.WalkState);
        }
        if (movement.PlayerInput.PlayerInput.Crouch.triggered) movement.SwitchState(movement.CrouchState);
        if (movement.PlayerInput.PlayerInput.Prone.triggered) movement.SwitchState(movement.ProneState);

    }
}
