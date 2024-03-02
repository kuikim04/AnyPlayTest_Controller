using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.Animator.SetBool("Walking", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        Vector2 movementInput = movement.PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        if (movement.PlayerInput.PlayerInput.Sprint.triggered) ExitState(movement, movement.RunState);
        else if (movement.PlayerInput.PlayerInput.Crouch.triggered) ExitState(movement, movement.CrouchState);
        else if (movement.PlayerInput.PlayerInput.Prone.triggered) ExitState(movement, movement.ProneState);
        else if (movementInput.magnitude < 0.1f) ExitState(movement, movement.IdleState);

        if (movementInput.y < 0) movement.CurrentMoveSpeed = movement.WalkBackSpeed;
        else movement.CurrentMoveSpeed = movement.WalkSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.Animator.SetBool("Walking", false);
        movement.SwitchState(state);
    }
}
