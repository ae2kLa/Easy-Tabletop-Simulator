using QFramework;
using UnityEngine;

namespace Tabletop.Local
{
    public abstract class LocalMapAttachArea : LocalOutLineObj, LocalIAttachable
    {
        /// <summary>
        /// 属于哪个Grids
        /// </summary>
        [HideInInspector] public EasyGrid<LocalGridData> Grids;

        /// <summary>
        /// 该格对应哪个GridData
        /// </summary>
        [HideInInspector] public LocalGridData Grid;
        [HideInInspector] public LocalMapObj Map;


        public abstract void Attach(LocalDragObj dragObject);
    }

}
