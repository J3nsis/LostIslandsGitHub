using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteCameraController : MonoBehaviour {

    public GameObject player;       //Public variable to store a reference to the player game object
    public bool rotateWithPlayer  = false;

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    private Vector3 offset2;        //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

        offset2 = transform.position - (player.transform.position + player.transform.forward * offset.z);
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if (rotateWithPlayer)
        {
            transform.position = player.transform.position + player.transform.forward * offset.z + offset2;
            transform.LookAt(player.transform);
        }
        else
        {
            transform.position = player.transform.position + offset;
            transform.LookAt(player.transform);
        }
    }
}
