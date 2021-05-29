using UnityEngine;
using System.Collections;

public class Callback
{
    
    public delegate System.Object CallbackHandler(System.Object data);
    public event CallbackHandler CallbackFunc;
    public System.Object Touch(System.Object data)
    {
        try
        {
            if (CallbackFunc != null)
            {
                return CallbackFunc(data);
            }
        }
        catch (System.Exception)
        {
            return null;
        }
        return null;
    }
}
