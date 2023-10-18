using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGoChessPiece : LocalDragObj
    {
        //黑白两方
        [HideInInspector] protected GoChessColor m_virtualColor = GoChessColor.Unknown;
        public GoChessColor VirtualColor
        {
            set
            {
                if (m_virtualColor == value) return;

                m_virtualColor = value;
                ColorChange(m_virtualColor);
            }
            get { return m_virtualColor; }
        }

        public Material WhiteMaterial;
        public Material BlackMaterial;

        protected override void Init()
        {
            base.Init();
        }

        private void ColorChange(GoChessColor virtualColor)
        {
            //修改为对应材质
            if (virtualColor == GoChessColor.White)
            {
                transform.Find("model").GetComponent<MeshRenderer>().material = WhiteMaterial;
            }
            else if (virtualColor == GoChessColor.Black)
            {
                transform.Find("model").GetComponent<MeshRenderer>().material = BlackMaterial;
            }
            else
            {
                print("棋子颜色未知");
            }
        }

        protected override bool CheckHandleAddition(GoChessColor color)
        {
            return color == VirtualColor;
        }

    }

}