using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonPlayerMovement : MonoBehaviour
{
    [SerializeField]
    private bool m_IsWalking;
    [SerializeField]
    private float m_WalkSpeed;
    [SerializeField]
    private float m_RunSpeed;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_RunstepLenghten;
    [SerializeField]
    private float m_JumpSpeed;
    [SerializeField]
    private float m_StickToGroundForce;
    [SerializeField]
    private float m_GravityMultiplier;
    public Transform HeadBone;
    public Vector3 HeadBoneOffset;
    [SerializeField]
    private MouseLook m_MouseLook;
    [SerializeField]
    private bool m_UseFovKick;
    [SerializeField]
    private FOVKick m_FovKick = new FOVKick();
    [SerializeField]
    private bool m_UseHeadBob;
    [SerializeField]
    private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField]
    private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField]
    private float m_StepInterval;
    [SerializeField]
    private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField]
    private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField]
    private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;

    private AudioSource m_AudioSource;
    [HideInInspector]
    public Vector3 m_CurrentCameraLookDirection;

    [SerializeField]
    Animator anim;

    public bool StateRun;
    public bool StateWalk;
    public bool StateStand;
    public bool StateJump;
    public bool lockMoving = false;//nur f�r ToolController!
    public bool lockRotation = false;

    public bool Pause;

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Net_Manager.instance.GetLocalPlayer().GetComponentInChildren<Camera>();
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_AudioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform, m_Camera.transform);
    }

    private void Update()
    {
        if (Pause)
        {
            StateStand = false;
            StateWalk = false;
            StateRun = false;
            StateJump = false;
            PlayerStats.instance.isRunning = false;
            anim.SetBool("stateJump", false);
            anim.SetBool("stateRun", false);
            anim.SetBool("stateWalk", false);
            return; 
        }

        if (lockRotation == false)
        {
            RotateView();
        }
        // the jump state needs to read here to make sure it is not missed
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }


    private void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }

    private void FixedUpdate()
    {
        if (Pause)
        {
            return;
        }

        if (!m_IsWalking)
        {
            PlayerStats.instance.isRunning = true;
            StateStand = false;
            StateWalk = false;
            StateRun = true;
        }

        if (CrossPlatformInputManager.GetAxis("Vertical") > 0 && m_IsWalking)
        {
            StateStand = false;
            StateWalk = true;
            StateRun = false;
            PlayerStats.instance.isRunning = false;
        }


        if (CrossPlatformInputManager.GetAxis("Vertical") == 0)
        {
            StateStand = true;
            StateWalk = false;
            StateRun = false;
            PlayerStats.instance.isRunning = false;
        }




        //################################################
        float speed = 0;

        GetInput(out speed);


        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                            m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;


        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;

                StateJump = true;
            }
            else
            {
                StateJump = false;
            }
        }
        else
        {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        }

        if (!lockMoving) m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        m_MouseLook.UpdateCursorLock();
        //#############################################

        if (StateRun)
        {
            PlayerStats.instance.isRunning = true;
            anim.SetBool("stateRun", true);
        }
        else
        {
            PlayerStats.instance.isRunning = false;
            anim.SetBool("stateRun", false);
        }

        if (StateWalk)
        {
            anim.SetBool("stateWalk", true);
        }
        else
        {
            anim.SetBool("stateWalk", false);
        }

        if (StateJump)
        {
            anim.SetBool("stateJump", true);
            anim.SetBool("stateRun", false);
            anim.SetBool("stateWalk", false);
        }
        else
        {
            anim.SetBool("stateJump", false);
        }
    }


    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }


    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }


    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;

        if (HeadBone)
        {
            Vector3 targetPosition = HeadBone.transform.position + m_Camera.transform.forward * HeadBoneOffset.z +
            m_Camera.transform.right * HeadBoneOffset.x + m_Camera.transform.up * HeadBoneOffset.y;

            //Vector3 lerpedPosition = Vector3.Lerp(m_Camera.transform.position, targetPosition, Time.deltaTime * 40f); // Camera Damping
            //m_Camera.transform.position = lerpedPosition;
            m_Camera.transform.position = targetPosition;

        }

        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                    (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;

    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_IsWalking;


        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running

        if ((PlayerStats.instance.ps.Ausdauer >= 1))//only allowed to run when enough stana ?
        {
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
        }
        else
        {
            m_IsWalking = Input.GetKey(KeyCode.LeftShift);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayerStats.instance.isRunning = true;
        }
        else
        {
            PlayerStats.instance.isRunning = false;
        }

        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);

        m_CurrentCameraLookDirection = m_Camera.transform.forward;

        if (HeadBone)
        {
            //Vector3 relativePos = HeadBone.transform.position + HeadBone.transform.up;
            Quaternion rotation = Quaternion.LookRotation(HeadBone.transform.right);

            m_Camera.transform.rotation = Quaternion.Lerp(m_Camera.transform.rotation, rotation, 0.6f); // Lerps between current rotation and headbone

        }
    }


    bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length >
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        //return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

}
