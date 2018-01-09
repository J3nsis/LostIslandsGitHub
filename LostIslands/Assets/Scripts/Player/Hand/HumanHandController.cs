using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class HumanHandController : MonoBehaviour {

    public Transform m_RightHandObj;//wird von HandManager geändert (Anker von Tool für Hand des Spielers)
    protected Animator animator;
    public bool ikActive = true;
    public bool headTracking = true;
    public float headTrackingWeight = 1.0f;

    private FirstPersonPlayerMovement m_fppm;
    private Transform m_HeadBone;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        m_fppm = GetComponentInParent<FirstPersonPlayerMovement>();
        m_HeadBone = m_fppm.HeadBone;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if ((animator) && (m_RightHandObj != null))
        {
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                if (headTracking)
                {
                    // Set our Look Weights
                    // m_LookWeight, m_BodyWeight, m_HeadWeight, m_EyesWeight, m_ClampWeight
                    Vector3 targetForward = m_HeadBone.position + m_fppm.m_CurrentCameraLookDirection * 1.0f;
                    Debug.DrawLine(m_HeadBone.position, targetForward, Color.red, 0.1f);

                    animator.SetLookAtWeight(1.0f, 0.5f, headTrackingWeight, 1.0f, 1.0f);
                    animator.SetLookAtPosition(targetForward);
                }

                // Set the right hand target position and rotation, if one has been assigned
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandObj.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
