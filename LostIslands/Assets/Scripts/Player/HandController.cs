using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    public Transform cRightHandObj;
    protected Animator animator;
    public bool ikActive = true;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if ((animator) && (cRightHandObj != null))
        {
            if (ikActive)
            {
                // Set the right hand target position and rotation, if one has been assigned
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, cRightHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, cRightHandObj.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            }
        }
    }

}
