using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public GameObject objectiveMarker;

    public GameObject animatorChild;
    public Animator animator;
    public PlayerAnimatorScript animationScript;

    public static bool killedByRat = false;

    public enum playerStates
    {
        walk,
        idle,
        run
    }

    //public bool isAnimationTriggered = false;

    public playerStates currentState;

    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float xSpeed = 0.0f;
    public float zSpeed = 0.0f;
    public float drag = 2.0f;
    public uint foodMeter = 0;

    // Sounds
    public AudioSource munchSfx;

    // Private Variables
    private float misnomer = 0.0f;
    private int winCondition = 18;
    // Movement speeds
    private float speed = 0.0f; // Current speed
    private const float wSpeed = 18.5f; // Walk speed
    private const float sSpeed = 25.0f; // Sprint speed
    // Jump Heights
    private float jumpForce = 150.0f;
    private float highJumpForce = 200.0f;
    // Rotation speeds
    private float rSpeed = 5.0f;

    private void Awake()
    {
        animator = animatorChild.GetComponent<Animator>();
        animationScript = animatorChild.GetComponent<PlayerAnimatorScript>();
    }

    private void FixedUpdate()
    {
        bool shiftKey = Input.GetKey(KeyCode.LeftShift);
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        if (!shiftKey && (verticalAxis != 0.0F || horizontalAxis != 0.0F))
        {
            WalkUpdate();
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        else if (shiftKey && (verticalAxis != 0.0F || horizontalAxis != 0.0F))
        {
            RunUpdate();
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
        }
        else if (verticalAxis == 0.0F && horizontalAxis == 0.0F)
        {
            WalkUpdate();
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
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

    private void StandardUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        misnomer = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Sprinting
            speed = sSpeed;
        }
        else
        {
            // Walking
            speed = wSpeed;
        }

        // Horiztonal movement
        MovementV2();
    }

    private void WalkUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        misnomer = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        speed = wSpeed;

        // Horiztonal movement
        MovementV2();
    }

    private void RunUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        misnomer = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        speed = sSpeed;

        // Horiztonal movement
        MovementV2();
    }

    private void Jump()
    {
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, misnomer + 0.1f))
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

    private void MovementV2()
    {
        xSpeed = Input.GetAxisRaw("Horizontal") * rSpeed;
        zSpeed = Input.GetAxisRaw("Vertical");

        this.transform.Rotate(new Vector3(0.0f, xSpeed, 0.0f));

        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * zSpeed * speed);
    }

    // When collided with trigger object
    private void OnTriggerEnter(Collider other)
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
    private void OnCollisionEnter(Collision collision)
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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CameraZone")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), false);
        }
    }

    private void CheckWin()
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

    public void Dead()
    {
        SceneManager.LoadScene("lose");
    }

    public void TransOut()
    {
        bool shiftKey = Input.GetKey(KeyCode.LeftShift);
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        if (shiftKey && (verticalAxis != 0.0F || horizontalAxis != 0.0F))
        {
            // running
            //currentState = playerStates.transInRun;
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
            //currentState = playerStates.transInIdle;
            animator.SetTrigger("TriggerStartIdle");
            //isAnimationTriggered = true;
            return;
        }
    }
}
