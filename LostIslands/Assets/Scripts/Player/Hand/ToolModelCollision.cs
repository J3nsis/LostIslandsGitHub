using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ToolModelCollision : MonoBehaviour {


    ToolSwinging toolSwinnging;

    private void Start()
    {
        if (transform.parent.gameObject.GetComponent<ToolSwinging>() != null)
        {
            toolSwinnging = transform.parent.gameObject.GetComponent<ToolSwinging>();
        }
        else
        {
            print("cant find parent script!");
        }
    }

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject == Net_Manager.instance.GetLocalPlayer()) return;    
        if (toolSwinnging != null) toolSwinnging.OnCollisionEnterOnModel(collision);
       
    }

    void OnCollisionStay(UnityEngine.Collision collision)
    {
        if (collision.gameObject == Net_Manager.instance.GetLocalPlayer()) return;
        if (toolSwinnging != null) toolSwinnging.OnCollisionStayOnModel(collision);
    }

    void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.gameObject == Net_Manager.instance.GetLocalPlayer()) return;
        if (toolSwinnging != null) toolSwinnging.OnCollisionExitOnModel(collision);
    }

}
