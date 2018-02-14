using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Net_SyncMessage : MonoBehaviour {

    public string MessageText;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(MessageText);
        }
        else
        {
            MessageText = (string)stream.ReceiveNext();
            GetComponent<Text>().text = MessageText;
        }

    }
}
