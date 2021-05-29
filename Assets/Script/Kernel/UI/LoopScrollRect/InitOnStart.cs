using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour
    {
        public bool IsAutoRefill = true;
        public int totalCount = -1;
        public int FullScreenItems = 5;//满屏状态可显示几个（这保证尾部偏移时有足够的元素填充）
        void Start()
        {
            if (IsAutoRefill)
            {
                var ls = GetComponent<LoopScrollRect>();
                ls.totalCount = totalCount;
                ls.RefillCellsFromEnd();
            }
           
        }
        public void RefillCellsInitial(uint cellindex)
        {
            var ls = GetComponent<LoopScrollRect>();
            int index = (int)cellindex;
            ls.totalCount = totalCount;
            if (index + FullScreenItems > totalCount)
            {
                index = totalCount - FullScreenItems;
            }
            if (index < 0)
            {
                index = 0;
            }
            ls.RefillCellsFromEnd(index);
        }

        public void RefillCells(int offset = 0)
        {
            var ls = GetComponent<LoopScrollRect>();
            ls.totalCount = totalCount;
            ls.RefillCells(offset);
        }
    }
}