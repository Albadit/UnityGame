using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class PlayerCamera : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityY = 1f;
    public float sensitivityX = 1f;
    public float minimumY = -360f;
    public float maximumY = 360f;
    public float minimumX = -70f;
    public float maximumX = 90f;
    float rotationX = 0f;
    float rotationY = 0f;
    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0f;
    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0f;
    public float frameCounter = 20;
    Quaternion originalRotation;

    public Transform player;
    public Vector3 offset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        transform.position = player.position + offset;
        transform.LookAt(player.position);
        Look();
    }

    public void Look()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            //Resets the average rotation
            rotAverageY = 0f;
            rotAverageX = 0f;

            //Gets rotational input from the mouse
            rotationY += Input.GetAxis("Mouse Y") * sensitivityX;
            rotationX += Input.GetAxis("Mouse X") * sensitivityY;

            //Adds the rotation values to their relative array
            rotArrayY.Add(rotationY);
            rotArrayX.Add(rotationX);

            //If the arrays length is bigger or equal to the value of frameCounter remove the first value in the array
            if (rotArrayY.Count >= frameCounter)
                rotArrayY.RemoveAt(0);
            if (rotArrayX.Count >= frameCounter)
                rotArrayX.RemoveAt(0);

            //Adding up all the rotational input values from each array
            for (int j = 0; j < rotArrayY.Count; j++)
                rotAverageY += rotArrayY[j];
            for (int i = 0; i < rotArrayX.Count; i++)
                rotAverageX += rotArrayX[i];

            //Standard maths to find the average
            rotAverageY /= rotArrayY.Count;
            rotAverageX /= rotArrayX.Count;

            //Clamp the rotation average to be within a specific value range
            rotAverageY = ClampAngle(rotAverageY, minimumX, maximumX);
            rotAverageX = ClampAngle(rotAverageX, minimumY, maximumY);

            //Get the rotation you will be at next as a Quaternion
            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

            //Rotate
            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotAverageX = 0f;
            rotationX += Input.GetAxis("Mouse X") * sensitivityY;
            rotArrayX.Add(rotationX);
            if (rotArrayX.Count >= frameCounter)
                rotArrayX.RemoveAt(0);
            for (int i = 0; i < rotArrayX.Count; i++)
                rotAverageX += rotArrayX[i];
            rotAverageX /= rotArrayX.Count;
            rotAverageX = ClampAngle(rotAverageX, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotAverageY = 0f;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityX;
            rotArrayY.Add(rotationY);
            if (rotArrayY.Count >= frameCounter)
                rotArrayY.RemoveAt(0);
            for (int j = 0; j < rotArrayY.Count; j++)
                rotAverageY += rotArrayY[j];
            rotAverageY /= rotArrayY.Count;
            rotAverageY = ClampAngle(rotAverageY, minimumX, maximumX);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }
    
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360f) && (angle <= 360f))
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}