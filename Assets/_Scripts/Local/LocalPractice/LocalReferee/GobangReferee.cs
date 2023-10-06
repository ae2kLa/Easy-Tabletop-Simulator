using QFramework;
using UnityEngine;

namespace Tabletop.Local
{
    public class GobangReferee: IReferee
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="grids"></param>
        /// <param name="map"></param>
        public void OnPieceDrop(LocalGridData grid, EasyGrid<LocalGridData> grids, LocalMapObj map)
        {
            var piece = grid.DragObject as LocalGoChessPiece;
            var pieceColor = piece.VirtualColor;
            var centerPos = new Vector2Int(grid.X, grid.Z);

            bool res =
            CheckSingleLine(centerPos, new Vector2Int(1, -1), pieceColor, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(0, -1), pieceColor, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(-1, -1), pieceColor, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(1, 0), pieceColor, grids);

            if (res)
            {
                Debug.Log("检测到五子连成一线");

                //通知某方胜利
                LocalPracticeController.Instance.WinEvent.Trigger(piece.VirtualColor);
                map.CurrentColor.Value = GoChessColor.Unknown;
                return;
            }

            //回合转换
            if (pieceColor == GoChessColor.Black)
            {
                map.CurrentColor.Value = GoChessColor.White;
            }
            else if (pieceColor == GoChessColor.White)
            {
                map.CurrentColor.Value = GoChessColor.Black;
            }
        }


        /// <summary>
        /// 裁判长连算法
        /// </summary>
        /// <param name="centerPos"></param>
        /// <param name="direction"></param>
        /// <param name=""></param>
        private bool CheckSingleLine(Vector2Int centerPos, Vector2Int direction, GoChessColor color, EasyGrid<LocalGridData> grids)
        {
            var startPos = centerPos - direction * 4;
            var endPos = centerPos + direction;

            //起点坐标不得越界
            while (startPos.x < 0 || startPos.x >= grids.Width)
            {
                startPos += direction;
            }
            while (startPos.y < 0 || startPos.y >= grids.Height)
            {
                startPos += direction;
            }

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
                    if (x < 0 || x >= grids.Width || y < 0 || y >= grids.Height ||
                       !grids[x, y].Occupied)
                    {
                        lineDone = false;
                        break;
                    }

                    //判断颜色
                    if (grids[x, y].DragObject is LocalGoChessPiece piece)
                    {
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
                        grids[x, y].DragObject.FreezeHighlight(Color.green);
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
