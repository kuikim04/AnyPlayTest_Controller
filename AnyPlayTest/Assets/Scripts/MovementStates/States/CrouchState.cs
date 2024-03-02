using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.Animator.SetBool("Crouching", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        Vector2 movementInput = movement.PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        if (movement.PlayerInput.PlayerInput.Sprint.triggered) ExitState(movement, movement.RunState);
        if (movement.PlayerInput.PlayerInput.Prone.triggered) ExitState(movement, movement.ProneState);
        if (movement.PlayerInput.PlayerInput.Crouch.triggered)
        {
            if (movementInput.magnitude < 0.1f) ExitState(movement, movement.IdleState);
            else ExitState(movement, movement.WalkState);
        }

        if (movementInput.y < 0) movement.CurrentMoveSpeed = movement.CrouchBackSpeed;
        else movement.CurrentMoveSpeed = movement.CrouchSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.Animator.SetBool("Crouching", false);
        movement.SwitchState(state);
    }
}
