using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Net_Destroy : MonoBehaviour {


    private void OnDestroy()
    {
        GetComponent<PhotonView>().RPC("Destroy", PhotonTargets.Others);
    }


    [PunRPC]
    void Destroy()
    {
        Destroy(this.gameObject);
    }

}
