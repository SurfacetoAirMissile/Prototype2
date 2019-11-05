﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using UnityEngine.SceneManagement;
using GamepadInput;

public class PlayerScript : MonoBehaviour
{
    // Serialised
    [SerializeField] GameObject animatorChild;
    [SerializeField] Animator animator;
    [SerializeField] PlayerAnimatorScript animationScript;
    [SerializeField] GameObject arrow;
    [SerializeField] float xSpeed = 0.0f;
    [SerializeField] float zSpeed = 0.0f;
    [SerializeField] float drag = 2.0f;

    // Sounds
    [SerializeField] AudioSource munchSfx;

    // Public
    public static bool killedByRat = false;
    public Vector3 velocity = new Vector3(0, 0, 0);
    public bool cheeseHeld = false;

    public enum playerStates
    {
        walk,
        idle,
        run
    }
    public playerStates currentState;

    // Private Variables
    /// <summary>
    /// The bounds of the player's y
    /// </summary>
    float yBounds = 0.0f;
    // Movement speeds
    /// <summary>
    /// Current Speed
    /// </summary>
    float speed = 0.0f;
    /// <summary>
    /// Walk Speed
    /// </summary>
    const float wSpeed = 18.5f;
    /// <summary>
    /// Sprint Speed
    /// </summary>
    const float sSpeed = 25.0f;
    // Rotation speeds
    /// <summary>
    /// Rotation Speed
    /// </summary>
    float rSpeed = 5.0f;
    // Jump Heights
    float jumpForce = 50.0f;

    // Buttons
    GamePad.Button jumpButton = GamePad.Button.A;
    /// <summary>
    /// True if sprint button is held, false if not
    /// </summary>
    bool shiftKey = false;
    GamePad.Button dropButton = GamePad.Button.B;
    GamePad.Button senseButton = GamePad.Button.Y;

     void Awake()
    {
        animator = animatorChild.GetComponent<Animator>();
        animationScript = animatorChild.GetComponent<PlayerAnimatorScript>();
    }

     void FixedUpdate()
    {
        shiftKey = (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One) < 0.5f) ? false : true;
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

        UpdateAnimations();

        // Only be able to move if sense is not being held
        if (!GamePad.GetButton(senseButton, GamePad.Index.One))
        {
            UpdateArrow(false);

            if (!shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F))
            {
                WalkUpdate();
            }
            else if (shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F))
            {
                RunUpdate();
            }
            else if (leftStick.x == 0.0F && leftStick.y == 0.0F)
            {
                WalkUpdate();
            }
        }
        // Sense is being held
        else
        {
            UpdateArrow(true);
            // Set animation
        }

    }

    /// <summary>
    /// Moves the player
    /// </summary>
     void WalkUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        yBounds = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (GamePad.GetButton(jumpButton, GamePad.Index.One))
        {
            Jump();
        }

        speed = wSpeed;

        // Horiztonal movement
        MovementV3();
    }

    /// <summary>
    /// Moves the player when sprinting
    /// </summary>
     void RunUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        yBounds = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (GamePad.GetButton(jumpButton, GamePad.Index.One))
        {
            Jump();
        }

        speed = sSpeed;

        // Horiztonal movement
        MovementV3();
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
     void Jump()
    {
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, yBounds))
        {
            // Normal Jump
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    // 'classic' tank controls
    /// <summary>
    /// Checks for input for movement
    /// </summary>
     void MovementV2()
    {
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
        Vector2 rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One);
        xSpeed = leftStick.x;
        zSpeed = leftStick.y;

        this.transform.Rotate(new Vector3(0.0f, xSpeed * rSpeed, 0.0f));

        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * zSpeed * speed);
    }
    // WASD controls (W is back wall)
    void MovementV1()
    {
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

        Vector3 movement = new Vector3(leftStick.x, 0.0f, leftStick.y);

        if (leftStick.x != 0.0f || leftStick.y != 0.0f)
        {
            this.GetComponent<Rigidbody>().AddForce(movement * speed);
        }
    }
    // Racing controls
    void MovementV3()
    {
        //GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
        Vector2 rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One);

        //zSpeed = GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One);
        //zSpeed -= GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.One);
        //xSpeed = leftStick.x;

        /*
        if (GamePad.GetButton(GamePad.Button.RightStick, GamePad.Index.One))
        {
            Camera.main.GetComponent<CinemachineFreeLook>().m_YAxis = ;
        }
        */

        if (leftStick.x != 0.0f || leftStick.y != 0.0f)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0.0f;
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0.0f;
            cameraRight.Normalize();
            
            Vector3 motionDirection = leftStick.x * cameraRight + leftStick.y * cameraForward;
            this.transform.forward = motionDirection;
            //this.transform.Rotate(new Vector3(0.0f, xSpeed * rSpeed, 0.0f));
            this.GetComponent<Rigidbody>().AddForce(this.transform.forward * speed);

        }


    }

    // When collided with trigger object
    void OnTriggerEnter(Collider other)
    {
        // Collided with cheese
        if (other.tag == "Cheese" && !cheeseHeld)
        {
            // Destroy collectable
            Destroy(other.gameObject);
            cheeseHeld = true;
        }

        if (other.tag == "CameraZone")
        {
            //GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            //camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), true);
        }

        // When player reaches home
        if (other.tag == "Home")
        {
            // Check score for win
            CheckWin();
        }
    }

    // When collided with collider
     void OnCollisionEnter(Collision collision)
    {
        // If colliding with cat
        if (collision.collider.tag == "cat")
        {
            Dead();
        }

        // If colliding with cat
        if (collision.collider.tag == "Rat")
        {
            if (collision.gameObject.GetComponent<ratScript>().currentState != ratScript.ratStates.dead)
            {
                killedByRat = true;
                Dead();
            }
        }
    }

     void OnTriggerExit(Collider other)
    {
        if (other.tag == "CameraZone")
        {
            //GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            //camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), false);
        }
    }

    void UpdateArrow(bool IsActive)
    {
        // Being set to active but is not currently active
        if (IsActive && !arrow.GetComponent<MeshRenderer>().enabled)
        {
            arrow.GetComponent<MeshRenderer>().enabled = true;
        }
        // Being turned off and is currently on
        else if (!IsActive && arrow.GetComponent<arrowScript>().enabled)
        {
            arrow.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Checks if the player has won
    /// </summary>
     void CheckWin()
    {
        // If had more than a certain amount of food
        if (cheeseHeld)
        {
            SceneManager.LoadScene("win");
        }
        else
        {
            Dead();
        }
    }

    /// <summary>
    /// Kills the player
    /// </summary>
    public void Dead()
    {
        SceneManager.LoadScene("lose");
    }

    void UpdateAnimations()
    {
        shiftKey = (GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One) < 0.5f) ? false : true;
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

        if (GamePad.GetButton(senseButton, GamePad.Index.One))
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
        else if (!shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        else if (shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
        }
        else if (leftStick.x == 0.0F && leftStick.y == 0.0F)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }

    /// <summary>
    /// Transitions out of animation
    /// </summary>
    public void TransOut()
    {
        //bool shiftKey = Input.GetKey(KeyCode.LeftShift);
        //float verticalAxis = Input.GetAxisRaw("Vertical");
        //float horizontalAxis = Input.GetAxisRaw("Horizontal");
        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
        float triggers = GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One);
        triggers -= GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.One);

        if (shiftKey && (leftStick.x != 0.0F || triggers != 0.0F))
        {
            // running
            //currentState = playerStates.transInRun;
            animator.SetTrigger("TriggerStartRun");
            //isAnimationTriggered = true;
            return;
        }
        if (leftStick.x != 0.0F || triggers != 0.0F)
        {
            // walking
            currentState = playerStates.walk;
            animator.SetTrigger("TriggerWalk");
            //isAnimationTriggered = true;
            return;
        }
        if (leftStick.x == 0.0F && triggers == 0.0F)
        {
            //currentState = playerStates.transInIdle;
            animator.SetTrigger("TriggerStartIdle");
            //isAnimationTriggered = true;
            return;
        }
    }
}
