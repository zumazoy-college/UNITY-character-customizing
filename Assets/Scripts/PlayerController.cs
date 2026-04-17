using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 8f;

    public float JumpForce = 3f;

    public float DistanceToGround = 0.1f;

    private Rigidbody _Rigidbody;
    private PlayerInputActions _PlayerInputActions;

    private PlayerAnimationController _PlayerAnimationController;
    private Vector2 InputVector;

    public void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();

        _PlayerInputActions = new PlayerInputActions();
        _PlayerInputActions.Enable();
        _PlayerInputActions.Player.Jump.performed += Jump;

        _PlayerAnimationController = GetComponent<PlayerAnimationController>();
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, DistanceToGround);


    }

    public void Jump(InputAction.CallbackContext Context)
    {
        if (IsGround()) _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    public void Move()
    {
        float UpDown = _PlayerInputActions.Player.UpDown.ReadValue<float>();
        float RightLeft = _PlayerInputActions.Player.LeftRight.ReadValue<float>();

        InputVector = new Vector2(RightLeft, UpDown);

        _Rigidbody.AddForce(new Vector3(RightLeft, 0, UpDown) * MovementSpeed, ForceMode.Force);

        _PlayerAnimationController.SetAnimationMove(InputVector);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistanceToGround));
    }

    private void FixedUpdate()
    {
        Move();
    }

}
