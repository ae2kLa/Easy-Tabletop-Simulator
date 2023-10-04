using Mirror;
using QFramework;
using UnityEngine;


namespace Tabletop.Online
{
    /// <summary>
    /// ��¼��ǰ�Ծֵ���һ���غ�
    /// </summary>
    public class OnlineMapObject : NetworkBehaviour
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
        /// ����ԭ���ƫ��
        /// </summary>
        [SerializeField]
        protected Vector3 offset;

        protected EasyGrid<OnlineGridData> m_grids;
        public EasyGrid<OnlineGridData> Grids => m_grids;

        protected bool ServerStarted = false;

        protected Transform AttachParent;

        //TODO:����ֶν�ʱ���³�������
        public BindableProperty<GoChessColor> CurrentColor;

        public override void OnStartServer()
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
                //TODO:�׻غϻ�û����ʾ
                OnlinePlayerManager.Instance.ForEach((player) =>
                {
                    if (player.CurrentColor == color)
                        OnlinePlayerManager.Instance.SendMsg(player.netId, $"���ڵ���Ļغ���");
                });
            });
            ServerStarted = true;
        }

        [Server]
        public void MapInit()
        {
            m_grids = new EasyGrid<OnlineGridData>(width, height);

            m_grids.ForEach((x, z, _) =>
            {
                m_grids[x, z] = new OnlineGridData(x, z, transform, offset, gridSize);
                var attachAreaGO = GameObject.Instantiate(AttachPrefab, m_grids[x, z].WorldPos, Quaternion.identity, AttachParent);

                //�ͻ�����������
                //NetworkServer.Spawn(attachArea, connectionToClient);

                var attachArea = attachAreaGO.GetComponent<OnlineAttachArea>();
                attachArea.Grids = this.Grids;
                attachArea.Grid = m_grids[x, z];
                attachArea.Map = this;
            });
        }


        public void RestartGame(uint playerNid)
        {
            m_grids.ForEach((x, z, grid) =>
            {
                grid.Occupied = false;
                if (grid.DragObject != null)
                {
                    grid.DragObject.Restart(playerNid);
                    grid.DragObject = null;
                }
            });
        }


        private void Update()
        {
            if (!ServerStarted) return;

            Draw();
        }

        protected void Draw()
        {
            if (m_grids == null) return;

            m_grids.ForEach((x, z, gird) =>
            {
                if (x >= m_grids.Width - 1 || z >= m_grids.Height - 1)//����������̸�
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