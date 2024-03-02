using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.Animator.SetBool("Running", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        Vector2 movementInput = movement.PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        if (movement.PlayerInput.PlayerInput.Sprint.triggered) ExitState(movement, movement.WalkState);
        if (movementInput.magnitude < 0.1f) ExitState(movement, movement.IdleState);

        if (movementInput.y < 0) movement.CurrentMoveSpeed = movement.RunBackSpeed;
        else movement.CurrentMoveSpeed = movement.RunSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.Animator.SetBool("Running", false);
        movement.SwitchState(state);
    }
}
