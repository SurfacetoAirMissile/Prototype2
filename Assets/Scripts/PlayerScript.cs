using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    // Serialised
    [SerializeField] GameObject objectiveMarker;
    [SerializeField] GameObject animatorChild;
    [SerializeField] Animator animator;
    [SerializeField] PlayerAnimatorScript animationScript;

    // Public
    public static bool killedByRat = false;

    public enum playerStates
    {
        walk,
        idle,
        transInIdle,
        transOutIdle,
        run,
        transInRun,
        transOutRun,

    }
    public playerStates currentState;

    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    [SerializeField] float xSpeed = 0.0f;
    [SerializeField] float zSpeed = 0.0f;
    [SerializeField] float drag = 2.0f;
    [SerializeField] uint foodMeter = 0;

    // Sounds
    [SerializeField] AudioSource munchSfx;

    // Private Variables
    /// <summary>
    /// The bounds of the player's y
    /// </summary>
    float yBounds = 0.0f;
    int winCondition = 18;
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
    float jumpForce = 150.0f;
    float highJumpForce = 200.0f;

     void Awake()
    {
        animator = animatorChild.GetComponent<Animator>();
        animationScript = animatorChild.GetComponent<PlayerAnimatorScript>();
    }

     void FixedUpdate()
    {
        bool shiftKey = Input.GetKey(KeyCode.LeftShift);
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        switch (currentState)
        {
            case playerStates.idle:
                WalkUpdate();
                if (verticalAxis != 0.0F || horizontalAxis != 0.0F)
                {
                    animator.SetTrigger("TriggerExitIdle");
                }
                break;
            case playerStates.walk:
                WalkUpdate();
                if (verticalAxis == 0.0F && horizontalAxis == 0.0F)
                {
                    animator.SetTrigger("TriggerStartIdle");
                }
                if (shiftKey)
                {
                    animator.SetTrigger("TriggerStartRun");
                }
                break;
            case playerStates.run:
                RunUpdate();
                if ((verticalAxis == 0.0F && horizontalAxis == 0.0F) || !shiftKey)
                {
                    animator.SetTrigger("TriggerExitRun");
                }
                break;
            default:
                break;
        }

        if (foodMeter >= 18)
        {
            objectiveMarker.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            objectiveMarker.GetComponent<MeshRenderer>().enabled = false;
        }

    }

    // Not called?
    // void StandardUpdate()
    //{
    //    // Manual Drag (X and Z axis)
    //    Vector3 vel = this.GetComponent<Rigidbody>().velocity;
    //    vel.x *= (0.98f / drag);
    //    vel.z *= (0.98f / drag);
    //    this.GetComponent<Rigidbody>().velocity = vel;

    //    // Update distance to ground
    //    yBounds = this.GetComponent<CapsuleCollider>().bounds.extents.y;

    //    // Jump
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Jump();
    //    }

    //    // Sprint
    //    if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        // Sprinting
    //        speed = sSpeed;
    //    }
    //    else
    //    {
    //        // Walking
    //        speed = wSpeed;
    //    }

    //    // Horiztonal movement
    //    MovementV2();
    //}

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        speed = wSpeed;

        // Horiztonal movement
        MovementV2();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        speed = sSpeed;

        // Horiztonal movement
        MovementV2();
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
     void Jump()
    {
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, yBounds + 0.1f))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // High Jump
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * highJumpForce);
            }
            else
            {
                // Normal Jump
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
            }
        }
    }

    /// <summary>
    /// Checks for input for movement
    /// </summary>
     void MovementV2()
    {
        xSpeed = Input.GetAxisRaw("Horizontal") * rSpeed;
        zSpeed = Input.GetAxisRaw("Vertical");

        this.transform.Rotate(new Vector3(0.0f, xSpeed, 0.0f));

        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * zSpeed * speed);
    }

    // When collided with trigger object
     void OnTriggerEnter(Collider other)
    {
        // Collided with cheese
        if (other.tag == "Cheese")
        {
            // Destroy collectable
            Destroy(other.gameObject);

            foodMeter += 4;

            munchSfx.Play();

            GameObject.Find("Image").GetComponent<UIscript>().UpdateSprites(foodMeter);
        }

        // Collided with bread
        if (other.tag == "Bread")
        {
            // Destroy the bread
            Destroy(other.gameObject);

            foodMeter += 1;

            munchSfx.Play();

            GameObject.Find("Image").GetComponent<UIscript>().UpdateSprites(foodMeter);
        }

        if (other.tag == "CameraZone")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), true);
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
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), false);
        }
    }

    /// <summary>
    /// Checks if the player has won
    /// </summary>
     void CheckWin()
    {
        // If had more than a certain amount of food
        if (foodMeter >= winCondition)
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

    /// <summary>
    /// Transitions out of animation
    /// </summary>
    public void TransOut()
    {
        bool shiftKey = Input.GetKey(KeyCode.LeftShift);
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (shiftKey && (verticalAxis != 0.0F || horizontalAxis != 0.0F))
        {
            // running
            currentState = playerStates.transInRun;
            animator.SetTrigger("TriggerStartRun");
            //isAnimationTriggered = true;
            return;
        }
        if (verticalAxis != 0.0F || horizontalAxis != 0.0F)
        {
            // walking
            currentState = playerStates.walk;
            animator.SetTrigger("TriggerWalk");
            //isAnimationTriggered = true;
            return;
        }
        if (verticalAxis == 0.0F && horizontalAxis == 0.0F)
        {
            currentState = playerStates.transInIdle;
            animator.SetTrigger("TriggerStartIdle");
            //isAnimationTriggered = true;
            return;
        }
    }
}
