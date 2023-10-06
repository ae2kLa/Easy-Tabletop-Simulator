using QFramework;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalMapObj : MonoBehaviour
    {
        public GameObject AttachPrefab;

        [SerializeField]
        [Range(1, 20)]
        protected int width = 5;

        [Range(1, 20)]
        [SerializeField]
        protected int height = 5;

        [Range(0.1f, 3f)]
        [SerializeField]
        protected float gridSize = 1f;

        /// <summary>
        /// 棋盘原点的偏移
        /// </summary>
        [SerializeField]
        protected Vector3 offset;

        protected EasyGrid<LocalGridData> m_grids;
        public EasyGrid<LocalGridData> Grids => m_grids;

        protected Transform AttachParent;

        public BindableProperty<GoChessColor> CurrentColor;
        public BindableProperty<LocalOutLineObj> LastOutlineObj;

        /// <summary>
        /// TODO:选择模式后注入
        /// </summary>
        private IReferee m_gameReferee;
        private LocalIRetract m_gameRetarter;

        public void Awake()
        {
            if (AttachParent == null)
            {
                var GO = new GameObject("AttachParent");
                AttachParent = GO.transform;
            }

            MapInit();

            CurrentColor = new BindableProperty<GoChessColor>(GoChessColor.Black);
            CurrentColor.RegisterWithInitValue((color) =>
            {
                //print($"回合轮转");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            LastOutlineObj = new BindableProperty<LocalOutLineObj>();
            LastOutlineObj.RegisterWithInitValue((outlineObj) =>
            {
                if(outlineObj != null)
                {
                    outlineObj.FreezeHighlight(Color.red);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            m_gameReferee = new GobangReferee();
            m_gameRetarter = new LocalGobangRetracter();
        }

        public void MapInit()
        {
            m_grids = new EasyGrid<LocalGridData>(width, height);

            m_grids.ForEach((x, z, _) =>
            {
                m_grids[x, z] = new LocalGridData(x, z, transform, offset, gridSize);
                var attachAreaGO = GameObject.Instantiate(AttachPrefab, m_grids[x, z].WorldPos, Quaternion.identity, AttachParent);
                var attachArea = attachAreaGO.GetComponent<LocalMapAttachArea>();
                m_grids[x, z].AttachArea = attachArea;
                attachArea.Grids = Grids;
                attachArea.Grid = m_grids[x, z];
                attachArea.Map = this;
            });
        }

        public void OnPieceDrop(LocalGridData grid, EasyGrid<LocalGridData> grids)
        {
            m_gameReferee.OnPieceDrop(grid, grids, this);
        }

        public void OnRecordStep(LocalMapAttachArea attachArea)
        {
            m_gameRetarter.RecordStep(attachArea);
        }

        public void RetractLastStep()
        {
            m_gameRetarter.RetractLastStep();
        }

        public void RetractAll()
        {
            m_gameRetarter.RetractAll();
        }

        public void RestartGame()
        {
            m_grids.ForEach((x, z, grid) =>
            {
                grid.Occupied = false;
                if (grid.DragObject != null)
                {
                    grid.DragObject.RecycleFromContainer();
                    grid.DragObject = null;
                }
            });
        }


        private void Update()
        {
            Draw();
        }

        protected void Draw()
        {
            if (m_grids == null) return;

            m_grids.ForEach((x, z, gird) =>
            {
                if (x >= m_grids.Width - 1 || z >= m_grids.Height - 1)//交点才算棋盘格
                {

                }
                else
                {
                    var tileWorldPos = gird.WorldPos;
                    var leftBottomPos = tileWorldPos;
                    var leftTopPos = tileWorldPos + new Vector3(0, 0, gridSize);
                    var rightBottomPos = tileWorldPos + new Vector3(gridSize, 0, 0);
                    var rightTopPos = tileWorldPos + new Vector3(gridSize, 0, gridSize);

                    Debug.DrawLine(leftBottomPos, leftTopPos, Color.red);
                    Debug.DrawLine(leftBottomPos, rightBottomPos, Color.red);
                    Debug.DrawLine(rightTopPos, leftTopPos, Color.red);
                    Debug.DrawLine(rightTopPos, rightBottomPos, Color.red);
                }
            });
        }

    }

}
