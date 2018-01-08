using UnityEngine;

public class DestroyMe : MonoBehaviour {


    public void Destroy(float time)
    {
        Destroy(this.gameObject, time);
    }
}
