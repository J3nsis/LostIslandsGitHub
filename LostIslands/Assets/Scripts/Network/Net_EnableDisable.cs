using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Net_EnableDisable : MonoBehaviour {


    void OnEnable()
    {
        GetComponent<PhotonView>().RPC("EnableDisable", PhotonTargets.Others, true);
    }

    void OnDisable()
    {
        GetComponent<PhotonView>().RPC("EnableDisable", PhotonTargets.Others, false);
    }



    [PunRPC]
    void EnableDisable(bool enable)
    {
        this.gameObject.SetActive(enable);
    }

}
