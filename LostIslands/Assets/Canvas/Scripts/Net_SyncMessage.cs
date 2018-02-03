using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_SyncMessage : MonoBehaviour {

    public string MessageText;
    public Transform MessageParent;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(MessageText);
            stream.SendNext(MessageParent);
        }
        else
        {
            MessageText = (string)stream.ReceiveNext();
            MessageParent = (Transform)stream.ReceiveNext();

        }

    }
}
