using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public class Net_ObjectsSyncHandler : MonoBehaviour {

    [SerializeField]
    bool SyncEnableDisable, SyncDestroy;


    private void OnDestroy()
    {
        if (SyncDestroy)
        {
            PhotonNetwork.Destroy(GetComponent<PhotonView>());
        }        
    }

    void OnEnable()
    {
        if (SyncEnableDisable)
        {
            GetComponent<PhotonView>().RPC("EnableDisable", PhotonTargets.Others, true);
        }
    }

    void OnDisable()
    {
        if (SyncEnableDisable)
        {
            GetComponent<PhotonView>().RPC("EnableDisable", PhotonTargets.Others, false);
        }
    }


    [PunRPC]
    void EnableDisable(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
