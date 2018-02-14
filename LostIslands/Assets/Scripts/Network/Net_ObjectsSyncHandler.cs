using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public class Net_ObjectsSyncHandler : MonoBehaviour {

    [SerializeField]
    bool SyncDisable, SyncDestroy;//, SyncEnable;

    private void OnDestroy()
    {
        if (SyncDestroy)
        {
            //PhotonNetwork.Destroy(GetComponent<PhotonView>()); besser mit RPC weil sonst muss man Owner sein um zerstören zu können
            GetComponent<PhotonView>().RPC("RPC_Destroy", PhotonTargets.Others, 0.0f, false);
        }        
    }

    void OnDisable()//geht nicht 100%
    {
        if (SyncDisable)
        {
            GetComponent<PhotonView>().RPC("RPC_Disable", PhotonTargets.Others);
        }
    }

    /*void OnEnable()
    {
        if (SyncEnable)
        {
            GetComponent<PhotonView>().RPC("RPC_Enable", PhotonTargets.Others);
        }
    }*/

    public void Net_DestroyMe(float time = 0f, bool fadeOut = false)
    {
        GetComponent<PhotonView>().RPC("RPC_Destroy", PhotonTargets.All, time, fadeOut);   
    }

    public void DestroyMe(float time = 0, bool fadeOut = false)
    {
        if (fadeOut)
        {
            if (GetComponent<FadeObjectInOut>() == null)
            {
                gameObject.AddComponent<FadeObjectInOut>();
            }
            if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            GetComponent<FadeObjectInOut>().FadeOut();
            Destroy(this.gameObject, time + GetComponent<FadeObjectInOut>().fadeTime); //fadeTime = 0.5
        }
        else
        {
            Destroy(this.gameObject, time);
        }
    }

    [PunRPC]
    void RPC_Disable()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    void RPC_Enable()
    {
        gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_Destroy(float time, bool fadeOut = false)
    {
        if (fadeOut)
        {
            if (GetComponent<FadeObjectInOut>() == null)
            {
                gameObject.AddComponent<FadeObjectInOut>();
            }
            GetComponent<FadeObjectInOut>().FadeOut();
            Destroy(this.gameObject, time + GetComponent<FadeObjectInOut>().fadeTime); //fadeTime = 0.5
        }
        else
        {
            Destroy(this.gameObject, time);
        } 
    }

}
