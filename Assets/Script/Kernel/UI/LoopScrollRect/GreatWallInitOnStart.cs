using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.GreatWallVerticalScrollRect))]
    [DisallowMultipleComponent]
    public class GreatWallInitOnStart : MonoBehaviour
    {
        public int totalCount = -1;
        void Start()
        {
            var ls = GetComponent<GreatWallVerticalScrollRect>();
            ls.totalCount = totalCount;
            ls.RefillCells();
        }
    }
}