using UnityEngine;
using System.Collections;

public class ResourceController : MonoBehaviour 
{
    public delegate void ReleaseCallbackHandler();
    public event ReleaseCallbackHandler Release;

    protected void ReleaseResource()
    {
        if (Release != null)
        {
            Release();
        }
    }
	
}
