using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    #region Instance
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerController found!");
            return;
        }
        instance = this;
    }
    #endregion



    public bool Pause;
    public bool onlyBlockMoving;

    
    void Start()
    {
        /*if (!this.GetComponent<PhotonView>().isMine)
        {
            Destroy(this);
        }*/
    }

    
    void FixedUpdate()
    {

        

    }
    
}
