using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public enum InvertMouseInput { Normal, InvertX, InvertY, InvertBoth }

    [Header("Input Type")]
    public InvertMouseInput mouseInputInversion = InvertMouseInput.Normal;

    [Header("Mouse Movement")]
    [Range(0.1f, 10)]
    public float mouseSensitivity = 1;
    public float cameraSmoothing = 1;
    public float verticalRangeUp = 150;
    public float verticalRangeDown = 150;

    private float FOVToMouseSensitivity = 1;

    private float baseCamFOV;
    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;

    public Camera cam;

    void Awake() => Cursor.lockState = CursorLockMode.Locked;

    void Start()
    {
        baseCamFOV = cam.fieldOfView;
    }

    void Update()
    {
        float mouseYInput = 0;
        float mouseXInput = 0;
        float camFOV = cam.fieldOfView;

        mouseYInput = mouseInputInversion == InvertMouseInput.Normal || mouseInputInversion == InvertMouseInput.InvertX ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
        mouseXInput = mouseInputInversion == InvertMouseInput.Normal || mouseInputInversion == InvertMouseInput.InvertY ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X");

        if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
        if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

        targetAngles.y += mouseXInput * (mouseSensitivity - ((baseCamFOV - camFOV) * FOVToMouseSensitivity) / 6f);

        targetAngles.x += mouseYInput * (mouseSensitivity - ((baseCamFOV - camFOV) * FOVToMouseSensitivity) / 6f);

        targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * verticalRangeDown, 0.5f * verticalRangeUp);
        followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, (cameraSmoothing) / 100);

        cam.transform.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
        transform.localRotation = Quaternion.Euler(0, followAngles.y + originalRotation.y, 0);
    }
}
