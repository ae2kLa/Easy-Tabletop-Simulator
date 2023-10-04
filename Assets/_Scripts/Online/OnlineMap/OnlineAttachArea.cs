using Mirror;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Online
{
    public class OnlineAttachArea : OnlineOutLineObj, OnlineIAttachable
    {
        /// <summary>
        /// 属于哪个Grids
        /// </summary>
        [HideInInspector] public EasyGrid<OnlineGridData> Grids;

        /// <summary>
        /// 该格对应哪个GridData
        /// </summary>
        [HideInInspector] public OnlineGridData Grid;

        [HideInInspector] public OnlineMapObj Map;

        public override void OnStartServer()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
        }


        public void Attach(uint playerNid, OnlineDragObj dragObject)
        {
            //TODO:这个方法届时当下沉到子类
            var piece = dragObject as OnlineGoChessPiece;
            if (piece is null)
            {
                OnlinePlayerManager.Instance.SendMsg(playerNid, $"所拖拽物体并非围棋棋子");
                return;
            }
            else if (piece.VirtualColor != Map.CurrentColor.Value)
            {
                if (Map.CurrentColor.Value == GoChessColor.Black)
                    OnlinePlayerManager.Instance.SendMsg(playerNid, $"当前是黑子回合，白子落子无效");
                else
                    OnlinePlayerManager.Instance.SendMsg(playerNid, $"当前是白子回合，黑子落子无效");

                //落子无效时自动将棋子移回棋篓
                StartCoroutine(piece.RecycleDragObject(playerNid));
                return;
            }

            if (Grid.Occupied) return;
            Grid.Occupied = true;
            Grid.DragObject = dragObject;

            //棋子还没移动到目标点时不准在棋盘上落子
            var currentColor = Map.CurrentColor.Value;
            Map.CurrentColor.Value = GoChessColor.Unknown;
            StartCoroutine(piece.ApplyAttachTransform(transform, () =>
            {
                var rb = piece.transform.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.freezeRotation = true;

                //TODO:这个方法届时当下沉到子类
                if (CheckWin(piece.VirtualColor))
                {
                    OnlinePlayerManager.Instance.SendAllMsg("检测到五子连成一线");

                    //TODO:清空棋盘，重新开始

                    Map.CurrentColor.Value = GoChessColor.Unknown;
                    return;
                }

                //回合转换
                if (currentColor == GoChessColor.Black)
                {
                    Map.CurrentColor.Value = GoChessColor.White;
                }
                else if (currentColor == GoChessColor.White)
                {
                    Map.CurrentColor.Value = GoChessColor.Black;
                }
            }));
        }

        /// <summary>
        /// 每次仅按“放射状”检测“以本格为中心的局部9x9”即可
        /// </summary>
        private bool CheckWin(GoChessColor color)
        {
            var centerPos = new Vector2Int(Grid.X, Grid.Z);

            bool res =
            CheckSingleLine(centerPos, new Vector2Int(1, -1), color) ||
            CheckSingleLine(centerPos, new Vector2Int(0, -1), color) ||
            CheckSingleLine(centerPos, new Vector2Int(-1, -1), color) ||
            CheckSingleLine(centerPos, new Vector2Int(1, 0), color);

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centerPos"></param>
        /// <param name="direction"></param>
        /// <param name=""></param>
        private bool CheckSingleLine(Vector2Int centerPos, Vector2Int direction, GoChessColor color)
        {
            var startPos = centerPos - direction * 4;
            var endPos = centerPos + direction;

            //起点坐标不得越界
            while (startPos.x < 0 || startPos.x >= Grids.Width)
            {
                startPos += direction;
            }
            while (startPos.y < 0 || startPos.y >= Grids.Height)
            {
                startPos += direction;
            }

            //print("startPos:" + startPos);
            //print("endPos:" + endPos);

            while (startPos.x != endPos.x || startPos.y != endPos.y)
            {
                var x = startPos.x;
                var y = startPos.y;
                var xOffset = direction.x;
                var yOffset = direction.y;

                bool lineDone = true;
                //print("*************************************************************");
                for (int i = 0; i < 5; i++, x += xOffset, y += yOffset)
                {
                    //print("x, y:" + x + " " + y);
                    //不得越界
                    if (x < 0 || x >= Grids.Width || y < 0 || y >= Grids.Height ||
                       !Grids[x, y].Occupied)
                    {
                        lineDone = false;
                        break;
                    }

                    //判断颜色
                    if (Grids[x, y].DragObject is OnlineGoChessPiece)
                    {
                        var piece = Grids[x, y].DragObject as OnlineGoChessPiece;
                        if (piece.VirtualColor != color)
                        {
                            lineDone = false;
                            break;
                        }
                    }
                }

                //自动高亮并返回True。(TODO)更好的做法是out一个List供外部使用，而不是在算法里写业务逻辑
                if (lineDone)
                {
                    x = startPos.x;
                    y = startPos.y;
                    for (int i = 0; i < 5; i++, x += xOffset, y += yOffset)
                    {
                        Grids[x, y].DragObject.RpcFreezeHighlight(Color.green);
                    }
                    return true;
                }
                //print("*************************************************************");

                startPos += direction;
            }

            return false;
        }



    }

}
