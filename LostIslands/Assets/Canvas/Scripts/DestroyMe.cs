using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class DestroyMe : MonoBehaviour {


    public void Destroy(float time, bool online)
    {
        if (online)
        {
            GetComponent<PhotonView>().RPC("DestroyRPC", PhotonTargets.Others, time);
        }
        else
        {
            Destroy(this.gameObject, time);
        }
        
    }

    [PunRPC]
    public void DestroyRPC(float time)
    {
        Destroy(this.gameObject, time);
    }
}
