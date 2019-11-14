using System.Collections;
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
    [SerializeField] GameObject arrow;
    [SerializeField] float xSpeed = 0.0f;
    [SerializeField] float zSpeed = 0.0f;
    [SerializeField] float drag = 2.0f;
    [SerializeField] GameObject cheesePrefab;
    [SerializeField] GameObject heldCheeseObject;

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
    /// <summary>
    /// Speed while carrying cheese
    /// </summary>
    const float carrySpeed = 10.5f;
    // Rotation speeds
    /// <summary>
    /// Rotation Speed
    /// </summary>
    float rSpeed = 5.0f;
    // Jump Heights
    float jumpForce = 50.0f;

    MenuController UIcontrol;

    enum Animations
    {
        RUN,
        WALK,
        IDLE,
        SENSE,
        CHEESEIDLE,
        CHEESEWALK
    }

    // Buttons
    GamePad.Button jumpButton = GamePad.Button.A;
    /// <summary>
    /// True if sprint button is held, false if not
    /// </summary>
    bool shiftKey = false;
    GamePad.Button dropButton = GamePad.Button.B;
    bool senseButton = false;

    void Awake()
    {
        animator = animatorChild.GetComponent<Animator>();
        UIcontrol = GameObject.FindGameObjectWithTag("UIObject").GetComponent<MenuController>();
    }

    void FixedUpdate()
    {
        if (UIcontrol.currentMenu == MenuController.MenuMode.NONE)
        {
            shiftKey = GamePad.GetTrigger(GamePad.Trigger.RightTrigger, GamePad.Index.One) > 0.5f;
            senseButton = GamePad.GetTrigger(GamePad.Trigger.LeftTrigger, GamePad.Index.One) > 0.5f;
            if (!shiftKey) { shiftKey = Input.GetKey("left shift"); }
            Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
            float kbForward = Input.GetAxis("kbForward");
            float kbRight = Input.GetAxis("kbRight");

            // Only be able to move if sense is not being held
            if (!senseButton)
            {
                UpdateArrow(false);

                if (!shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F || kbForward != 0.0f || kbRight != 0.0f))
                {
                    WalkUpdate();
                    if (!cheeseHeld)
                    {
                        // Set animation
                        ChangeAnimation(Animations.WALK);
                    }
                    else
                    {
                        // Cheese held
                        ChangeAnimation(Animations.CHEESEWALK);
                    }
                }
                else if (shiftKey && (leftStick.x != 0.0F || leftStick.y != 0.0F || kbForward != 0.0f || kbRight != 0.0f))
                {
                    RunUpdate();
                    if (!cheeseHeld)
                    {
                        // Set animation
                        ChangeAnimation(Animations.RUN);
                    }
                    else
                    {
                        ChangeAnimation(Animations.CHEESEWALK);
                    }
                }
                else if (leftStick.x == 0.0F && leftStick.y == 0.0F && kbForward == 0.0f && kbRight == 0.0f)
                {
                    WalkUpdate();
                    if (!cheeseHeld)
                    {
                        // Set animation
                        ChangeAnimation(Animations.IDLE);
                    }
                    else
                    {
                        ChangeAnimation(Animations.CHEESEIDLE);
                    }
                }

                // Drop cheese
                if (GamePad.GetButton(dropButton, GamePad.Index.One) && cheeseHeld)
                {
                    cheeseHeld = false;
                    Vector3 spawnPos = transform.position;
                    spawnPos.y += yBounds + 0.2f;
                    GameObject newCheese = Instantiate(cheesePrefab, spawnPos, Quaternion.identity);
                    heldCheeseObject.GetComponent<MeshRenderer>().enabled = false;

                    // Throw forward
                    Vector3 force = new Vector3(0.0f, 100.0f, 0.0f);
                    force += transform.forward * 15.0f;
                    newCheese.GetComponent<Rigidbody>().AddForce(force);
                }
            }
            // Sense is being held
            else
            {
                UpdateArrow(true);
                if (!cheeseHeld)
                {
                    // Set animation
                    ChangeAnimation(Animations.SENSE);
                }
                else
                {
                    ChangeAnimation(Animations.CHEESEIDLE);
                }
            }
        }
        else
        {
            if (cheeseHeld) { ChangeAnimation(Animations.CHEESEIDLE); }
            else { ChangeAnimation(Animations.IDLE); }
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

        if (cheeseHeld)
        {
            speed = carrySpeed;
        }
        else
        {
            speed = wSpeed;
        }

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
        if (Input.GetKey("space"))
        {
            Jump();
        }

        if (cheeseHeld)
        {
            speed = carrySpeed;
        }
        else
        {
            speed = sSpeed;
        }

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
    
    // Racing controls
    void MovementV3()
    {
        //GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

        Vector2 leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

        float kbForward = Input.GetAxis("kbForward");
        float kbRight = Input.GetAxis("kbRight");

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

        if (kbRight != 0.0f || kbForward != 0.0f)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0.0f;
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0.0f;
            cameraRight.Normalize();

            Vector3 motionDirection = kbRight * cameraRight + kbForward * cameraForward;
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
            heldCheeseObject.GetComponent<MeshRenderer>().enabled = true;
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
            if (collision.gameObject.GetComponent<RatNavScript>().currentState != RatNavScript.ratStates.dead)
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
            UIcontrol.ChangeMenuMode(MenuController.MenuMode.WINSCREEN);
        }
        else
        {
            UIcontrol.ChangeMenuMode(MenuController.MenuMode.LOSESCREEN);
        }
    }

    /// <summary>
    /// Kills the player
    /// </summary>
    public void Dead()
    {
        SceneManager.LoadScene("lose");
    }

    void ChangeAnimation(Animations i)
    {
        switch(i)
        {
            case Animations.IDLE:
                {
                    animator.SetBool("Idle", true);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Sense", false);
                    animator.SetBool("CheeseWalk", false);
                    animator.SetBool("CheeseIdle", false);
                    break;
                }
            case Animations.WALK:
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", true);
                    animator.SetBool("Run", false);
                    animator.SetBool("Sense", false);
                    animator.SetBool("CheeseWalk", false);
                    animator.SetBool("CheeseIdle", false);
                    break;
                }
            case Animations.RUN:
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", true);
                    animator.SetBool("Sense", false);
                    animator.SetBool("CheeseWalk", false);
                    animator.SetBool("CheeseIdle", false);
                    break;
                }
            case Animations.SENSE:
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Sense", true);
                    animator.SetBool("CheeseWalk", false);
                    animator.SetBool("CheeseIdle", false);
                    break;
                }
            case Animations.CHEESEWALK:
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Sense", false);
                    animator.SetBool("CheeseWalk", true);
                    animator.SetBool("CheeseIdle", false);
                    break;
                }
            case Animations.CHEESEIDLE:
                {
                    animator.SetBool("Idle", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Sense", false);
                    animator.SetBool("CheeseWalk", false);
                    animator.SetBool("CheeseIdle", true);
                    break;
                }

            default:
                break;
        }
    }
}
