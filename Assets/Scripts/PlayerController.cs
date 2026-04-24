using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Unity.Cinemachine;

public class PlayerController : NetworkBehaviour
{
    public float MovementSpeed = 8f;
    public float JumpForce = 3f;
    public float DistanceToGround = 0.5f;
    public float RotationSmoothing = 20f;

    public GameObject CameraAnchor;

    public CinemachineCamera PlayerCamera;

    private Rigidbody _Rigidbody;
    private PlayerInputActions _PlayerInputActions;

    private PlayerAnimationController _PlayerAnimationController;
    private Vector2 InputVector;

    private float pitch, yaw;

    private void Start()
    {
        if (isClient && isLocalPlayer) PlayerCamera.Priority = 100;
    }

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();


        _PlayerInputActions = new PlayerInputActions();
        _PlayerInputActions.Enable();
        _PlayerInputActions.Player.Jump.performed += Jump;

        _PlayerAnimationController = GetComponent<PlayerAnimationController>();
    }



    public void Jump(InputAction.CallbackContext context)
    {
        if (IsGround()) _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, DistanceToGround);
    }


    [Command]
    public void Move(float RightLeft, float UpDown)
    {
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
        if (!(isClient && isLocalPlayer)) return;

        float upDown = _PlayerInputActions.Player.UpDown.ReadValue<float>();
        float rightLeft = _PlayerInputActions.Player.LeftRight.ReadValue<float>();
        Vector2 look = _PlayerInputActions.Player.Aim.ReadValue<Vector2>();

        yaw += look.x;
        pitch = Mathf.Clamp(pitch - look.y, -60f, 90f);

        SetRotationCamera(pitch);
        Move(rightLeft, upDown);
        SetRotation(yaw);
    }


    [Command]
    public void SetRotation(float yaw)
    {
        Quaternion target = Quaternion.Euler(0f, yaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, RotationSmoothing * Time.fixedDeltaTime);
    }

    public void SetRotationCamera(float pitch)
    {
        Quaternion target = Quaternion.Euler(pitch, 0f, 0f);
        CameraAnchor.transform.localRotation = Quaternion.Slerp(
            CameraAnchor.transform.localRotation, target, RotationSmoothing * Time.fixedDeltaTime);
    }

}
