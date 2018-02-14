using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Tool))]
public class ToolSwinging : MonoBehaviour {

    [HideInInspector]
    public FirstPersonPlayerMovement fppm;
    public float impactForce = 1f; // Defines dropback force
    public float impactForceFallOff = 10.0f; // Defines dropback falloff speed
    public float m_StickToGroundForce = 10.0f; // Defines dropback falloff speed

    private CharacterController m_Character;
    private Vector3 impact = Vector3.zero;
    protected Animator m_Animator;
    protected Animator m_PlayerAnimator;
    [SerializeField]
    private bool m_isSlaying;
    private float m_SlayingTime;

    public bool isSlaying;

    Tool tool;
    bool AtBasePosition;

    bool canCollideAgain = true;

    // Use this for initialization
    void Start() {
        fppm = Net_Manager.instance.GetLocalPlayer().GetComponent<FirstPersonPlayerMovement>();
        m_Animator = GetComponent<Animator>();
        m_PlayerAnimator = fppm.gameObject.GetComponentInChildren<Animator>(); // Assume 1st animator is the player's animator 
        m_Character = fppm.gameObject.GetComponent<CharacterController>();

        tool = GetComponent<Tool>();
    }

    void Update()
    {
        if (AnimatorIsPlaying("Base Layer.axe zero position"))
        {
            AtBasePosition = true;
        }
        else
        {
            AtBasePosition = false;
        }


        if (Input.GetMouseButtonDown(0) && PlayerStats.instance.ps.Ausdauer >= 10 && !fppm.Pause && AtBasePosition)
        {
            isSlaying = true;
            m_isSlaying = true;
            if (m_Animator) m_Animator.SetTrigger("slay");
            m_PlayerAnimator.SetTrigger("slay");
            m_SlayingTime = Time.time;
            //current Durability wird nur bei Treffer eins abgezogen
            PlayerStats.instance.OnSwing();//darin wird dann ausdauer abgezogen
            return;
        }

        if (isSlaying && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("axe"))
        {
            isSlaying = false;
        }

        if (HandManager.instance.currentToolObj == null)
        {
            fppm.lockMoving = false;
        }

    }

    void FixedUpdate () {

        // apply the impact force:
        if (impact.magnitude > 0.1)
        {
            m_Character.Move(new Vector3(impact.x, -m_StickToGroundForce, impact.z) * Time.deltaTime);
            if (HandManager.instance.currentToolObj != null) fppm.lockMoving = true;
            //nur wenn Tool in Hand auch Moving lock
        }
        else
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

    public void OnCollisionEnterOnModel(UnityEngine.Collision collision)//werden von Model des Tools (Child) aufgerufen, weil es den richtigen Collider hat
    {
        if (collision.gameObject.GetComponent<DamageController>() && canCollideAgain)
        {
            canCollideAgain = false;
            float currentSlayingTime = Time.time - m_SlayingTime;
            if (currentSlayingTime > 0.4f) m_isSlaying = false;
            if (isSlaying)
            {
                fppm.gameObject.GetComponentInChildren<PlayerSounds>().PlaySoundOnce("AxeChopping");
                collision.gameObject.GetComponent<DamageController>().DamageMe(tool.item.Damage);
                tool.data.currentdurability -= 1;
            }
        }
        m_Animator.SetTrigger("hitSomething");
        m_PlayerAnimator.SetTrigger("hitSomething");
        if (HandManager.instance.currentToolObj != null && fppm) fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);

        // Check if the collision target has a Rigidbody
        Rigidbody collisionRB = collision.gameObject.GetComponent<Rigidbody>();
        if (collisionRB) collisionRB.AddForceAtPosition(-contact.normal * 15f, contact.point, ForceMode.Impulse);
    }

    public void OnCollisionStayOnModel(UnityEngine.Collision collision)
    {
        if (HandManager.instance.currentToolObj != null && fppm) fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);
    }

    public void OnCollisionExitOnModel(UnityEngine.Collision collision)
    {
        canCollideAgain = true;
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

    void OnDisable()
    {
        if (fppm) fppm.lockMoving = false;
    }

}
