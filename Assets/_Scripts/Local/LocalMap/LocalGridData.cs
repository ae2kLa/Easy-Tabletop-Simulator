using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGridData
    {
        public LocalGridData(int x, int z, Transform parentTrans, Vector3 offset, float size = 1f)
        {
            X = x;
            Z = z;
            m_parentTrans = parentTrans;
            m_offset = offset;
            m_localPos = new Vector3(x * size, 0, z * size);
        }

        /// <summary>
        /// 对应的吸附区域是哪个
        /// </summary>
        public LocalIAttachable AttachArea;

        /// <summary>
        /// 是否被占领/吸附
        /// </summary>
        public bool Occupied;

        /// <summary>
        /// 吸附在上面的物体
        /// </summary>
        public LocalDragObj DragObject;

        /// <summary>
        /// 当前格对应的局部坐标
        /// </summary>
        private Vector3 m_localPos;

        /// <summary>
        /// 依照哪个物体为参考系
        /// </summary>
        private Transform m_parentTrans;

        /// <summary>
        /// 偏移
        /// </summary>
        public Vector3 m_offset;

        /// <summary>
        /// 当前格对应的世界坐标
        /// </summary>
        public Vector3 WorldPos => m_localPos + m_parentTrans.position + m_offset;

        /// <summary>
        /// 在棋盘中的x坐标
        /// </summary>
        public int X;

        /// <summary>
        /// 在棋盘中的z坐标
        /// </summary>
        public int Z;
    }

}