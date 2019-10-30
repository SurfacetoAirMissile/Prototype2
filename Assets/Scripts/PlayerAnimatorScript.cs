using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour
{
    [SerializeField] GameObject animatorParent;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] Animator animator;

    void Awake()
    {
        playerScript = animatorParent.GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Idle animation is to be activated
    /// </summary>
    public void IdleActivateEndFunc()
    {
        animator.SetTrigger("TriggerIdle");
        playerScript.currentState = PlayerScript.playerStates.idle;
        //playerScript.isAnimationTriggered = false;
    }

    /// <summary>
    /// Idle animation is to be deactivated
    /// </summary>
    public void IdleDeactivateEndFunc()
    {
        playerScript.TransOut();
    }

    /// <summary>
    /// Run animation is to be activated
    /// </summary>
    public void RunActivateEndFunc()
    {
        animator.SetTrigger("TriggerRun");
        playerScript.currentState = PlayerScript.playerStates.run;
        //playerScript.isAnimationTriggered = false;
    }

    /// <summary>
    /// Run animation is to be deactivated
    /// </summary>
    public void RunDeactivateEndFunc()
    {
        playerScript.TransOut();
    }
}
