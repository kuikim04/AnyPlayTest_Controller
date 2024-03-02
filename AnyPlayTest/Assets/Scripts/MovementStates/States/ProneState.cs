using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProneState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.Animator.SetBool("Prone", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        Vector2 movementInput = movement.PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        if (movement.PlayerInput.PlayerInput.Sprint.triggered) ExitState(movement, movement.RunState);
        if (movement.PlayerInput.PlayerInput.Crouch.triggered) ExitState(movement, movement.CrouchState);
        if (movement.PlayerInput.PlayerInput.Prone.triggered)
        {
            if (movementInput.magnitude < 0.1f) ExitState(movement, movement.IdleState);
            else ExitState(movement, movement.WalkState);
        }

        if (movementInput.y < 0) movement.CurrentMoveSpeed = movement.ProneBackSpeed;
        else movement.CurrentMoveSpeed = movement.ProneSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.Animator.SetBool("Prone", false);
        movement.SwitchState(state);
    }

}
