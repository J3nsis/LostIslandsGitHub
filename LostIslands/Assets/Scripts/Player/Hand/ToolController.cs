using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Tool))]
public class ToolController : MonoBehaviour {

    [HideInInspector]
    public FirstPersonPlayerMovement fppm;
    public float impactForce = 1f; // Defines dropback force
    public float impactForceFallOff = 10.0f; // Defines dropback falloff speed
    public float m_StickToGroundForce = 10.0f; // Defines dropback falloff speed
    //public bool ikActive = true;

    private CharacterController m_Character;
    private Rigidbody m_Rigidbody;
    private Vector3 impact = Vector3.zero;
    protected Animator m_Animator;
    protected Animator m_PlayerAnimator;
    private AudioSource m_AudioSource;
    private bool m_isSlaying;
    private float m_SlayingTime;

    [SerializeField]
    bool curretlySlaying;

    Tool tool;

    // Use this for initialization
    void Start() {
        fppm = Net_Manager.instance.GetLocalPlayer().GetComponent<FirstPersonPlayerMovement>();
        m_Animator = GetComponent<Animator>();
        m_PlayerAnimator = fppm.gameObject.GetComponentInChildren<Animator>(); // Assume 1st animator is the player's animator 
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Character = fppm.gameObject.GetComponent<CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();

        tool = GetComponent<Tool>();
    }

    void Update()
    {
        if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.axe") &&
                !m_PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.standing melee attack downward")) // Check if the player is in idle state
        {
            curretlySlaying = false;
            tool.Swinging = false;
        }
        else
        {
            curretlySlaying = true;
            tool.Swinging = true;
        }


        if (Input.GetMouseButtonDown(0) && PlayerStats.instance.ps.Ausdauer >= 10 && PlayerController.instance.Pause == false && curretlySlaying == false)
        {
            if (m_Animator) m_Animator.SetTrigger("slay");
            m_PlayerAnimator.SetTrigger("slay");
            m_isSlaying = true;
            m_SlayingTime = Time.time;

            PlayerStats.instance.ps.Ausdauer -= 10;
            tool.data.currentdurability -= 1;
            curretlySlaying = true;
        }
    }

    void FixedUpdate () {
        // apply the impact force:
        if (impact.magnitude > 0.1)
        {
            m_Character.Move(new Vector3(impact.x, -m_StickToGroundForce, impact.z) * Time.deltaTime);
            fppm.lockMoving = true;
        } else
        {
            fppm.lockMoving = false;
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, impactForceFallOff * Time.deltaTime);
    }

    // call this function to add an impact force:
    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact = dir.normalized * force;
    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (m_AudioSource && collision.gameObject.GetComponent<DamageController>())
        {
            float currentSlayingTime = Time.time - m_SlayingTime;
            if (currentSlayingTime > 0.4f) m_isSlaying = false;
            if (m_isSlaying) m_AudioSource.Play();
        }
        if (m_Animator) m_Animator.SetTrigger("hitSomething");
        m_PlayerAnimator.SetTrigger("hitSomething");
        fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);

        // Check if the collision target has a Rigidbody
        Rigidbody collisionRB = collision.gameObject.GetComponent<Rigidbody>();
        if (collisionRB) collisionRB.AddForceAtPosition(-contact.normal * 15f, contact.point, ForceMode.Impulse);
    }

    void OnCollisionStay(UnityEngine.Collision collision)
    {
        fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);
    }

    bool AnimatorIsPlaying()
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).length >
               m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        //return AnimatorIsPlaying() && m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

}
