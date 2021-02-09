using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    Animator CameraObject;

    void Start()
    {
        CameraObject = transform.GetComponent<Animator>();
    }

    public void Settings() => CameraObject.SetBool("Settings", true);

    public void Return()
    {
        CameraObject.SetBool("Settings", false);
        CameraObject.SetBool("WaitingRoom", false);
    }

    public void WaitingRoom() => CameraObject.SetBool("WaitingRoom", true);
}
