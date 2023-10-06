using QFramework;
using UnityEngine;

namespace Tabletop.Local
{
    public class GobangReferee: IReferee
    {
        public bool CheckWin(GoChessColor color, LocalGridData grid, EasyGrid<LocalGridData> grids)
        {
            var centerPos = new Vector2Int(grid.X, grid.Z);

            bool res =
            CheckSingleLine(centerPos, new Vector2Int(1, -1), color, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(0, -1), color, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(-1, -1), color, grids) ||
            CheckSingleLine(centerPos, new Vector2Int(1, 0), color, grids);

            return res;
        }


        /// <summary>
        /// 
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
