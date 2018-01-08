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
    protected Animator animator;

    Tool tool;

    // Use this for initialization
    void Start () {
        fppm = Net_Manager.instance.GetLocalPlayer().GetComponent<FirstPersonPlayerMovement>();
        animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Character = fppm.gameObject.GetComponent<CharacterController>();

        tool = GetComponent<Tool>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerStats.instance.ps.Ausdauer >= 10 && PlayerController.instance.Pause == false && tool.Swinging == false)
        {
            animator.SetTrigger("slay");
            PlayerStats.instance.ps.Ausdauer -= 10;
            tool.data.currentdurability -= 1;
        }

        if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("tool animation")))//wenn nicht am Swingen
        {
            tool.Swinging = false;
        }
        else
        {
            tool.Swinging = true;
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
        animator.SetTrigger("hitSomething");
        fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);
    }


    void OnCollisionStay(UnityEngine.Collision collision)
    {
        fppm.lockMoving = true;
        ContactPoint contact = collision.contacts[0];
        AddImpact(contact.normal, impactForce);
    }

}
