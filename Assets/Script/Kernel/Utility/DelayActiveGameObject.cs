using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayActiveGameObject : MonoBehaviour
{
    [System.Serializable]
    public class TargetObjectState : System.IComparable<TargetObjectState>
    {
        public float Time;
        public bool Active;
        public GameObject[] GameObjects;

        public int CompareTo(TargetObjectState other)
        {
            if (Time < other.Time)
            {
                return 1;
            }
            else if (Time > other.Time)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
    public bool RealTime;
    public List<TargetObjectState> Targets;

	// Use this for initialization
	void Start () {
        if (Targets == null || Targets.Count == 0) return;

        // 排序
        Targets.Sort();

        StartCoroutine(DelayActive());
	}

	IEnumerator DelayActive()
    {
        float time = 0;
        for (int i = 0; i < Targets.Count; i++)
        {
            var tos = Targets[i];
            if (i == 0)
            {
                time = tos.Time;
            }
            else
            {
                time = tos.Time - Targets[i - 1].Time;
            }
            
            if (RealTime)
            {
                yield return new WaitForSecondsRealtime(time);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            for (int j = 0; j < tos.GameObjects.Length; j++)
            {
                tos.GameObjects[j].SetActive(tos.Active);
            }
        }
    }
}
