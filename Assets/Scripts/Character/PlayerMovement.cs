using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpPower = 5f;
    public CrouchModifiers crouchSettings = new CrouchModifiers();

    public float currentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    [Header("Input")]
    public InputSettings inputSettings = new InputSettings();

    [Header("Animator")]
    public Animator anim;

    [Header("Camera Settings")]
    public Camera cam;
    public float fovChange;

    void Start()
    {
        
    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
        
    }

    private void OnCollisionStay(Collision CollisionData)
    {
        if (!IsGrounded)
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit(Collision CollisionData)
    {
        IsGrounded = false;
    }

    [System.Serializable]
    public class InputSettings
    {
        public KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode crouchKey = KeyCode.LeftControl;
    }

    [System.Serializable]
    public class CrouchModifiers
    {
        public bool useCrouch = true;
        public bool toggleCrouch = false;
        public float crouchWalkSpeedMultiplier = 0.5f;
        public float crouchJumpPowerMultiplier = 0f;
    }
}
