using UnityEngine;
using System.Collections;

public class RotationObject : MonoBehaviour 
{
    public GameObject mRotationObject;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnDrag (Vector2 delta)
    {
        if (delta.x != 0)
        {
            //Quaternion quat = new Quaternion();
            //quat.SetAxisAngle(Vector3.forward, -0.01f * delta.x);
            Quaternion quat = Quaternion.AngleAxis(-0.1f * delta.x, Vector3.forward);
            mRotationObject.transform.localRotation *= quat;
        }
    }
}
