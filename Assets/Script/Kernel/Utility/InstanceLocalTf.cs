using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceLocalTf : MonoBehaviour
{
    public Transform Src;
    public Transform[] Dest;


    void Update()
    {
        for (int i = 0; i < Dest.Length; i++)
        {

            Dest[i].localRotation = Src.localRotation;
        }
    }
    [ContextMenu("Refresh")]
    void RefreshDestArray()
    {
        List<Transform> tl = new List<Transform>();

        GetAllChildren(transform, Src.name, tl, Src);

        Dest = tl.ToArray();
    }
    void GetAllChildren(Transform node, string name, List<Transform> nodes, Transform exclude)
    {
        for (int i = 0; i < node.childCount; i++)
        {
            Transform child = node.GetChild(i);
            if (child.name == name)
            {
                if (child != exclude)
                    nodes.Add(child);
            }
            GetAllChildren(child, name, nodes, exclude);
        }
    }
    
}
