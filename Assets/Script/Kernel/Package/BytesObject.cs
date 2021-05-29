using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BytesObject : ScriptableObject
{
    public string Name;
    public byte[] Content;

    public BinaryReader BinaryReader
    {
        get
        {
            MemoryStream ms = new MemoryStream(Content);
            BinaryReader br = new BinaryReader(ms);
            return br;
        }
    }
}
