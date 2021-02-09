
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Rigidbody))]

public class CharacterMovement : MonoBehaviour
{
    #region Variables

    public bool controllerPauseState = false;

    public bool enableCameraMovement = true;
    public enum InvertMouseInput { None, X, Y, Both }
    public InvertMouseInput mouseInputInversion = InvertMouseInput.None;
    public enum CameraInputMethod { Traditional, TraditionalWithConstraints, Retro }
    public CameraInputMethod cameraInputMethod = CameraInputMethod.Traditional;

    public float verticalRotationRange = 170;
    public float mouseSensitivity = 10;
    public float fOVToMouseSensitivity = 1;
    public float cameraSmoothing = 5f;
    public bool lockAndHideCursor = false;
    public Camera playerCamera;

    public bool enableCameraShake = false;
    internal Vector3 cameraStartingPosition;
    float baseCamFOV;

    public Sprite Crosshair;
    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;

    public bool walkByDefault = true;
    public float walkSpeed = 4f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 8f;
    public float jumpPower = 5f;
    public bool canJump = true;
    public bool canHoldJump;
    bool jumpInput;
    bool didJump;
    public float speed;
    internal float walkSpeedInternal;
    internal float sprintSpeedInternal;
    internal float jumpPowerInternal;

    [System.Serializable]
    public class CrouchModifiers
    {
        public bool useCrouch = true;
        public bool toggleCrouch = false;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public float crouchWalkSpeedMultiplier = 0.5f;
        public float crouchJumpPowerMultiplier = 0f;
        public bool crouchOverride;
        internal float colliderHeight;

    }
    public CrouchModifiers crouchSettings = new CrouchModifiers();

    [System.Serializable]
    public class AdvancedSettings
    {
        public float gravityMultiplier = 1.0f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
        public float maxSlopeAngle = 55;
        internal bool isTouchingWalkable;
        internal bool isTouchingUpright;
        internal bool isTouchingFlat;
        public float maxWallShear = 89;
        public float maxStepHeight = 0.2f;
        internal bool stairMiniHop = false;
        public RaycastHit surfaceAngleCheck;
        public Vector3 curntGroundNormal;
        public Vector2 moveDirRef;
        public float lastKnownSlopeAngle;
        public float FOVKickAmount = 2.5f;
        public float changeTime = 0.75f;
        public float fovRef;
    }

    public AdvancedSettings advanced = new AdvancedSettings();
    private CapsuleCollider capsule;
    private Vector2 inputXY;
    float yVelocity;
    private bool IsGrounded { get; set; }
    private bool isCrouching;
    private bool isSprinting = false;

    public Rigidbody fps_Rigidbody;

    #endregion

    private void Awake()
    {
        originalRotation = transform.localRotation.eulerAngles;

        walkSpeedInternal = walkSpeed;
        sprintSpeedInternal = sprintSpeed;
        jumpPowerInternal = jumpPower;
        capsule = GetComponent<CapsuleCollider>();
        IsGrounded = true;
        isCrouching = false;
        fps_Rigidbody = GetComponent<Rigidbody>();
        fps_Rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        fps_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        crouchSettings.colliderHeight = capsule.height;

    }

    private void Start()
    {
        Canvas canvas = new GameObject("AutoCrosshair").AddComponent<Canvas>();
        canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        canvas.transform.SetParent(playerCamera.transform);
        canvas.transform.position = Vector3.zero;

        Image crossHair = new GameObject("Crosshair").AddComponent<Image>();
        crossHair.sprite = Crosshair;
        crossHair.rectTransform.sizeDelta = new Vector2(25, 25);
        crossHair.transform.SetParent(canvas.transform);
        crossHair.transform.position = Vector3.zero;

        cameraStartingPosition = playerCamera.transform.localPosition;
        if (lockAndHideCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }

        baseCamFOV = playerCamera.fieldOfView;
        capsule.radius = capsule.height / 4;
        advanced.zeroFrictionMaterial = new PhysicMaterial("Zero_Friction");
        advanced.zeroFrictionMaterial.dynamicFriction = 0;
        advanced.zeroFrictionMaterial.staticFriction = 0;
        advanced.zeroFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        advanced.zeroFrictionMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        advanced.highFrictionMaterial = new PhysicMaterial("Max_Friction");
        advanced.highFrictionMaterial.dynamicFriction = 1;
        advanced.highFrictionMaterial.staticFriction = 1;
        advanced.highFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        advanced.highFrictionMaterial.bounceCombine = PhysicMaterialCombine.Average;

        if (GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
    }

    private void Update()
    {
        if (enableCameraMovement && !controllerPauseState)
        {
            float mouseYInput = 0;
            float mouseXInput = 0;
            float camFOV = playerCamera.fieldOfView;

            // the Part where you invert or not 
            if (cameraInputMethod == CameraInputMethod.Traditional || cameraInputMethod == CameraInputMethod.TraditionalWithConstraints)
            {
                mouseYInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.X ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
                mouseXInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.Y ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X");
            }
            else
            {
                mouseXInput = Input.GetAxis("Horizontal") * (mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.Y ? 1 : -1);
            }

            if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
            if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

            targetAngles.y += mouseXInput * (mouseSensitivity - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f);

            if (cameraInputMethod == CameraInputMethod.Traditional) { targetAngles.x += mouseYInput * (mouseSensitivity - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f); }
            else { targetAngles.x = 0f; }

            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * verticalRotationRange, 0.5f * verticalRotationRange);
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, (cameraSmoothing) / 100);

            playerCamera.transform.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
            transform.localRotation = Quaternion.Euler(0, followAngles.y + originalRotation.y, 0);
        }

        if (canHoldJump ? (canJump && Input.GetButton("Jump")) : (Input.GetButtonDown("Jump") && canJump))
        {
            jumpInput = true;
        }
        else if (Input.GetButtonUp("Jump")) { jumpInput = false; }


        if (crouchSettings.useCrouch)
        {
            if (!crouchSettings.toggleCrouch) { isCrouching = crouchSettings.crouchOverride || Input.GetKey(crouchSettings.crouchKey); }
            else if (Input.GetKeyDown(crouchSettings.crouchKey)) { isCrouching = !isCrouching || crouchSettings.crouchOverride; }
        }

        if (Input.GetButtonDown("Cancel")) { ControllerPause(); }
    }

    private void FixedUpdate()
    {
        isSprinting = Input.GetKey(sprintKey);

        Vector3 MoveDirection = Vector3.zero;
        speed = walkByDefault ? isCrouching ? walkSpeedInternal : (isSprinting ? sprintSpeedInternal : walkSpeedInternal) : (isSprinting ? walkSpeedInternal : sprintSpeedInternal);

        if (advanced.maxSlopeAngle > 0)
        {
            if (advanced.isTouchingUpright && advanced.isTouchingWalkable)
            {

                MoveDirection = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal);
                if (!didJump) { fps_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; }
            }
            else if (advanced.isTouchingUpright && !advanced.isTouchingWalkable)
            {
                fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
            }

            else
            {
                fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                MoveDirection = ((transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal) * (fps_Rigidbody.velocity.y > 0.01f ? SlopeCheck() : 0.8f));
            }
        }
        else
        {
            MoveDirection = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal);
        }

        RaycastHit WT;
        if (advanced.maxStepHeight > 0 && Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - 0.01f, 0), MoveDirection, out WT, capsule.radius + 0.15f, Physics.AllLayers, QueryTriggerInteraction.Ignore) && Vector3.Angle(WT.normal, Vector3.up) > 88)
        {
            RaycastHit ST;
            if (!Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - (advanced.maxStepHeight), 0), MoveDirection, out ST, capsule.radius + 0.25f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                advanced.stairMiniHop = true;
                transform.position += new Vector3(0, advanced.maxStepHeight * 1.2f, 0);
            }
        }
        Debug.DrawRay(transform.position, MoveDirection, Color.red, 0, false);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        inputXY = new Vector2(horizontalInput, verticalInput);
        if (inputXY.magnitude > 1) { inputXY.Normalize(); }

        yVelocity = fps_Rigidbody.velocity.y;

        if (IsGrounded && jumpInput && jumpPowerInternal > 0 && !didJump)
        {
            if (advanced.maxSlopeAngle > 0)
            {
                if (advanced.isTouchingFlat || advanced.isTouchingWalkable)
                {
                    didJump = true;
                    jumpInput = false;
                    yVelocity += fps_Rigidbody.velocity.y < 0.01f ? jumpPowerInternal : jumpPowerInternal / 3;
                    advanced.isTouchingWalkable = false;
                    advanced.isTouchingFlat = false;
                    advanced.isTouchingUpright = false;
                    fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                }

            }
            else
            {
                didJump = true;
                jumpInput = false;
                yVelocity += jumpPowerInternal;
            }

        }

        if (advanced.maxSlopeAngle > 0)
        {


            if (!didJump && advanced.lastKnownSlopeAngle > 5 && advanced.isTouchingWalkable)
            {
                yVelocity *= SlopeCheck() / 4;
            }
            if (advanced.isTouchingUpright && !advanced.isTouchingWalkable && !didJump)
            {
                yVelocity += Physics.gravity.y;
            }
        }


        if (!controllerPauseState)
        {
            fps_Rigidbody.velocity = MoveDirection + (Vector3.up * yVelocity);

        }
        else { fps_Rigidbody.velocity = Vector3.zero; }

        if (inputXY.magnitude > 0 || !IsGrounded)
        {
            capsule.sharedMaterial = advanced.zeroFrictionMaterial;
        }
        else { capsule.sharedMaterial = advanced.highFrictionMaterial; }

        fps_Rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));


        if (advanced.FOVKickAmount > 0)
        {
            if (isSprinting && !isCrouching && playerCamera.fieldOfView != (baseCamFOV + (advanced.FOVKickAmount * 2) - 0.01f))
            {
                if (Mathf.Abs(fps_Rigidbody.velocity.x) > 0.5f || Mathf.Abs(fps_Rigidbody.velocity.z) > 0.5f)
                {
                    playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, baseCamFOV + (advanced.FOVKickAmount * 2), ref advanced.fovRef, advanced.changeTime);
                }

            }
            else if (playerCamera.fieldOfView != baseCamFOV) { playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, baseCamFOV, ref advanced.fovRef, advanced.changeTime * 0.5f); }

        }

        if (crouchSettings.useCrouch)
        {

            if (isCrouching)
            {
                capsule.height = Mathf.MoveTowards(capsule.height, crouchSettings.colliderHeight / 1.5f, 5 * Time.deltaTime);
                walkSpeedInternal = walkSpeed * crouchSettings.crouchWalkSpeedMultiplier;
                jumpPowerInternal = jumpPower * crouchSettings.crouchJumpPowerMultiplier;

            }
            else
            {
                capsule.height = Mathf.MoveTowards(capsule.height, crouchSettings.colliderHeight, 5 * Time.deltaTime);
                walkSpeedInternal = walkSpeed;
                sprintSpeedInternal = sprintSpeed;
                jumpPowerInternal = jumpPower;
            }
        }

        IsGrounded = false;

        if (advanced.maxSlopeAngle > 0)
        {
            if (advanced.isTouchingFlat || advanced.isTouchingWalkable || advanced.isTouchingUpright) { didJump = false; }
            advanced.isTouchingWalkable = false;
            advanced.isTouchingUpright = false;
            advanced.isTouchingFlat = false;
        }
    }

    public IEnumerator CameraShake(float Duration, float Magnitude)
    {
        float elapsed = 0;
        while (elapsed < Duration && enableCameraShake)
        {
            playerCamera.transform.localPosition = Vector3.MoveTowards(playerCamera.transform.localPosition, new Vector3(cameraStartingPosition.x + Random.Range(-1, 1) * Magnitude, cameraStartingPosition.y + Random.Range(-1, 1) * Magnitude, cameraStartingPosition.z), Magnitude * 2);
            yield return new WaitForSecondsRealtime(0.001f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localPosition = cameraStartingPosition;
    }

    public void RotateCamera(Vector2 Rotation, bool Snap)
    {
        enableCameraMovement = !enableCameraMovement;
        if (Snap) { followAngles = Rotation; targetAngles = Rotation; } else { targetAngles = Rotation; }
        enableCameraMovement = !enableCameraMovement;
    }

    public void ControllerPause()
    {
        controllerPauseState = !controllerPauseState;
        if (lockAndHideCursor)
        {
            Cursor.lockState = controllerPauseState ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = controllerPauseState;
        }
    }

    float SlopeCheck()
    {

        advanced.lastKnownSlopeAngle = Mathf.MoveTowards(advanced.lastKnownSlopeAngle, Vector3.Angle(advanced.curntGroundNormal, Vector3.up), 5f);

        return new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(advanced.maxSlopeAngle + 15, 0f), new Keyframe(advanced.maxWallShear, 0.0f), new Keyframe(advanced.maxWallShear + 0.1f, 1.0f), new Keyframe(90, 1.0f)) { preWrapMode = WrapMode.Clamp, postWrapMode = WrapMode.ClampForever }.Evaluate(advanced.lastKnownSlopeAngle);

    }

    private void OnCollisionEnter(Collision CollisionData)
    {
        for (int i = 0; i < CollisionData.contactCount; i++)
        {
            float a = Vector3.Angle(CollisionData.GetContact(i).normal, Vector3.up);
            if (CollisionData.GetContact(i).point.y < transform.position.y - ((capsule.height / 2) - capsule.radius * 0.95f))
            {

                if (!IsGrounded)
                {
                    IsGrounded = true;
                    advanced.stairMiniHop = false;
                    if (didJump && a <= 70) { didJump = false; }
                }

                if (advanced.maxSlopeAngle > 0)
                {
                    if (a < 5.1f) { advanced.isTouchingFlat = true; advanced.isTouchingWalkable = true; }
                    else if (a < advanced.maxSlopeAngle + 0.1f) { advanced.isTouchingWalkable = true; /* IsGrounded = true; */}
                    else if (a < 90) { advanced.isTouchingUpright = true; }

                    advanced.curntGroundNormal = CollisionData.GetContact(i).normal;
                }

            }
        }
    }

    private void OnCollisionStay(Collision CollisionData)
    {

        for (int i = 0; i < CollisionData.contactCount; i++)
        {
            float a = Vector3.Angle(CollisionData.GetContact(i).normal, Vector3.up);
            if (CollisionData.GetContact(i).point.y < transform.position.y - ((capsule.height / 2) - capsule.radius * 0.95f))
            {

                if (!IsGrounded)
                {
                    IsGrounded = true;
                    advanced.stairMiniHop = false;
                }

                if (advanced.maxSlopeAngle > 0)
                {
                    if (a < 5.1f) { advanced.isTouchingFlat = true; advanced.isTouchingWalkable = true; }
                    else if (a < advanced.maxSlopeAngle + 0.1f) { advanced.isTouchingWalkable = true; /* IsGrounded = true; */}
                    else if (a < 90) { advanced.isTouchingUpright = true; }

                    advanced.curntGroundNormal = CollisionData.GetContact(i).normal;
                }

            }
        }
    }

    private void OnCollisionExit(Collision CollisionData)
    {
        IsGrounded = false;
        if (advanced.maxSlopeAngle > 0) { advanced.curntGroundNormal = Vector3.up; advanced.lastKnownSlopeAngle = 0; advanced.isTouchingWalkable = false; advanced.isTouchingUpright = false; }

    }


}