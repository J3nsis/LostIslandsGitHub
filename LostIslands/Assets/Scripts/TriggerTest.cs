using UnityEngine;

public class TriggerTest : MonoBehaviour {

    public bool Trigger;
    public bool Collision;

	void Start () 
	{
        Trigger = false;
        Collision = false;
	}
	


    private void OnTriggerExit()
    {
        Trigger = false;
    }

    private void OnTriggerStay()
    {
        Trigger = true;
    }

    private void OnCollisionExit()
    {
        Collision = false;
    }

    private void OnCollisionStay()
    {
        Collision = true;
    }
}
