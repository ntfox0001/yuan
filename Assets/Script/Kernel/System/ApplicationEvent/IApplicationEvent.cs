using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IApplicationEvent
{
    void ApplicationPause();
    void ApplicationResume();
}
