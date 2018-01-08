using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanHandController : MonoBehaviour {

    public Transform RightHandObj; //wird von HandManager geändert
    protected Animator HumanAnimator;


    private void Start()
    {
        HumanAnimator = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        if (HumanAnimator && (RightHandObj != null))
        {
            // Set the right hand target position and rotation, if one has been assigned
            if (RightHandObj != null)
            {
                HumanAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                HumanAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                HumanAnimator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                HumanAnimator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                HumanAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                HumanAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
