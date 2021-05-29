using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public string PrefabName;
        public string GroupName;
        public int poolSize = 5;

        private bool inited = false;
        private GameObject mLastGo;
        private List<GameObject> pool = new List<GameObject>();
        public virtual GameObject GetObject()
        {
            if (!inited)
            {
                ResourceRecycle.Instance.InitPool(PrefabName, poolSize, SG.PoolInflationType.DOUBLE, GroupName);
                inited = true;
            }
            return ResourceRecycle.Instance.GetObjectFromPool(PrefabName);
        }

        public virtual void ReturnObject(Transform go)
        {
            go.GetComponent<ILoopScrollCellBase>().ScrollCellReturn();
            ResourceRecycle.Instance.ReturnObjectToPool(go.gameObject);
        }

    }
}
