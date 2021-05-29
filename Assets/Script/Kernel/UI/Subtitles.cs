using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subtitles : MonoBehaviour
{
    [System.Serializable]
    public enum Position
    {
        Bottom,
        Center,
    }
    [System.Serializable]
    public class Line
    {
        public int TextId;
        public float Time;
        public Position Pos;
    }

    public float ShowTime = 3.0f;
    public Line[] Trace;

}
