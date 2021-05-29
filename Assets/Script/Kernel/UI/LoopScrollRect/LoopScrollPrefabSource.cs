using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class GreatWallPrefabSource
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;
        private GameObject mLastGo;
        private List<GameObject> pool= new List<GameObject>();
     
        public GameObject GetObject()
        {
            return pool[pool.Count-1];  
        }
        public bool ReturnObject(Transform ts)
        {
            if (ts.gameObject == mLastGo)
                return false;
            mLastGo = ts.gameObject;
            pool.Remove(ts.gameObject);
            pool.Add(ts.gameObject);
            return true;
  
        }
    }
}
