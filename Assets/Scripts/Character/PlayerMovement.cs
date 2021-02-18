using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    private bool isRunning;
    public float jumpPower = 5f;
    private float speed;
    private Vector3 InputXYZ;
    public CrouchModifiers crouchSettings = new CrouchModifiers();

    public float currentSpeed = 0;
    public bool IsGrounded = true;

    [Header("Input")]
    public InputSettings inputSettings = new InputSettings();

    [Header("Animator")]
    public Animator anim;

    [Header("Camera Settings")]
    public Camera cam;
    public float fovChange;

    private Rigidbody rigid;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        float inputForward = Input.GetAxis("Vertical");
        float inputSideway = Input.GetAxis("Horizontal");

        speed = walkSpeed;
        InputXYZ = Vector3.zero;
        Vector3 forward = inputForward * transform.forward;
        Vector3 sideway = inputSideway * transform.right;

        Vector3 combinedInput = (forward + sideway).normalized;
        InputXYZ = new Vector3(combinedInput.x, 0f, combinedInput.z);

        float inputMagnitude = Mathf.Abs(inputForward) +
            Mathf.Abs(inputSideway);
        currentSpeed = Mathf.Clamp01(inputMagnitude);
    

        if(Input.GetKey(inputSettings.sprintKey))
        {
            speed = runSpeed;
            isRunning = true;
        }
        else 
        {
            speed = walkSpeed;
            isRunning = false;
        }
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
