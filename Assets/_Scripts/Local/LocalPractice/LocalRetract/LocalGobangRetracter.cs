using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGobangRetracter : LocalIRetract
    {
        private Stack<LocalMapAttachArea> attachAreas = new Stack<LocalMapAttachArea>();

        public void RecordStep(LocalMapAttachArea attachArea)
        {
            attachAreas.Push(attachArea);
        }

        /// <summary>
        /// 跟电脑对战五子棋时，悔棋一次=撤去两枚棋子；
        /// </summary>
        public void RetractLastStep()
        {
            if (attachAreas.Count == 0) return;

            int i = 0;
            while(attachAreas.Count != 0 && i < 2)
            {
                var attachArea = attachAreas.Pop();
                var dragObj = attachArea.Grid.DragObject;
                dragObj.RecycleFromContainer();
                attachArea.Grid.ClearOccupied();

                i++;
            }
        }

        public void RetractAll()
        {
            if (attachAreas.Count == 0) return;

            while (attachAreas.Count != 0)
            {
                var attachArea = attachAreas.Pop();
                var dragObj = attachArea.Grid.DragObject;
                dragObj.RecycleFromContainer();
                attachArea.Grid.ClearOccupied();
            }
        }

        //private IEnumerator RestractWithDelay()
        //{
        //    yield return new WaitForSeconds(0.8f);
        //}
    }
}