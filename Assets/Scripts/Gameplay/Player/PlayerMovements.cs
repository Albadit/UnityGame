using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[AddComponentMenu("Tamer/FPSC/FPSC Movement")]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public Transform cam;
    public float GravityStrength;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float speed;

    [Header("Jump")]
    public float jumpForce = 3f;
    public bool isGround;

    [Header("Animation")]
    public Animator anim;
    public float jumpForceForward = 3f;
    public float runJumpForceForward = 5.5f;
    private bool jumpAnim = false;
    private bool runAnim = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Animation();
        Move();
        StartCoroutine(Jump());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGround = true;
        } 
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGround = false;
        }
    }

    void Animation()
    {
        anim.SetFloat("Blend", speed);
        anim.SetBool("Jump", jumpAnim);
        anim.SetBool("Running", runAnim);
    }

    void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        speed = new Vector2(inputX, inputZ).sqrMagnitude;
        Vector3 direction = new Vector3(inputX, 0f, inputZ).normalized;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
        {
            runAnim = true;
            speed = 3.75f;
        }

        else
        {
            runAnim = false;

            if (speed > 1)
            {
                speed = 1f;
            }
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    IEnumerator Jump()
    {
        if (isGround == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGround = false;
                jumpAnim = true;
                yield return new WaitForSeconds(0.8f);
                //controller.addfore = new Vector3(0, jumpForce, jumpForceForward);
                yield return new WaitForSeconds(1.33f);
                jumpAnim = false;
            }

           /* if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpAnim = true;
                rb.velocity = new Vector3(0, jumpForce, runJumpForceForward);
                yield return new WaitForSeconds(0.75f);
                jumpAnim = false;
            }*/
        }
    }
}