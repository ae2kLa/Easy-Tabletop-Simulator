using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGobangRobotWithMiniMax : IRobot
    {
        private GoChessColor m_robotColor;
        private LocalGoChessBasket m_chessBasket;
        private LocalMapObj m_map;

        public LocalGobangRobotWithMiniMax(GoChessColor color, LocalGoChessBasket chessBasket, LocalMapObj map)
        {
            m_robotColor = color;
            m_chessBasket = chessBasket;
            m_map = map;

            m_map.CurrentColor.Register((color) =>
            {
                if (color == m_robotColor)
                {
                    map.StartCoroutine(OnTurnToRobot());
                }
            }).UnRegisterWhenGameObjectDestroyed(map);

            m_grids = new List<LocalGridData>();
            map.Grids.ForEach((x, z, grid) =>
            {
                m_grids.Add(grid);
            });
        }

        private List<LocalGridData> m_grids;
        private List<LocalGridData> m_allDrops;
        private List<LocalGridData> m_robotDrops;
        private List<LocalGridData> m_playerDrops;
        public int Depth = 3;
        public int AtkRatio = 1;//AI的进攻性，默认为1
        private LocalGridData m_targetGrid;

        private int m_searchCnt;
        private int m_cutCnt;

        public IEnumerator OnTurnToRobot()
        {
            m_searchCnt = 0;
            m_cutCnt = 0;

            m_allDrops = m_grids.Where((grid) => grid.Occupied).ToList();
            m_robotDrops = m_allDrops.Where((grid) =>
                (grid.DragObject as LocalGoChessPiece).VirtualColor == m_robotColor).ToList();
            m_playerDrops = m_allDrops.Where((grid) =>
                (grid.DragObject as LocalGoChessPiece).VirtualColor != m_robotColor).ToList();

            yield return null;

            var piece = m_chessBasket.Get(m_robotColor);
            //前两手下棋盘中心附近
            if (m_allDrops.Count <= 1)
            {
                var x = m_map.Grids.Width / 2;
                var y = m_map.Grids.Height / 2;
                if(!m_map.Grids[x, y].Occupied)
                    m_map.Grids[x, y].AttachArea.Attach(piece);
                else if(Random.Range(0, 100) % 2 == 1)
                    m_map.Grids[x + 1, y].AttachArea.Attach(piece);
                else
                    m_map.Grids[x - 1, y].AttachArea.Attach(piece);
            }
            else
            {
                Minimax(m_robotColor, Depth, float.MinValue, float.MaxValue);
                Debug.Log($"搜索次数:{m_searchCnt}, 剪枝次数:{m_cutCnt}");
                m_targetGrid.AttachArea.Attach(piece);
                m_targetGrid = null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="currentColor"></param>
        /// <param name="depth"></param>
        /// <param name="alpha">Alpha 是可能解的最大下限，也就是至少能够获得的评分，我方的步骤中会不断试图提高这个值。</param>
        /// <param name="beta">Beta 是可能解的最小上限，也就是最好能够获得的评分，对方的步骤中会不断试图降低这个值。</param>
        /// <returns></returns>
        private float Minimax(GoChessColor currentColor, int depth, float alpha, float beta)
        {
            if (depth == 0)
            {
                Evaluation(currentColor, out int score);
                return score;
            }
            else if (Evaluation(currentColor, out int score))//有任意一方长连时提前返回
            {
                Debug.Log($"长连: {(2 + depth) * score}");
                return (2 + depth) * score;
            }

            //相邻无棋子则跳过
            var blankList = m_grids.Where(grid => !m_allDrops.Contains(grid) && HasNeightnor(grid)).ToList();
            LocalGridData bestGrid = null;
            if (currentColor == m_robotColor)
            {
                float best = float.MinValue;
                for (int i = 0; i < blankList.Count; i++)
                {
                    m_searchCnt++;
                    LocalGridData grid = blankList[i];

                    if (currentColor == m_robotColor)
                        m_robotDrops.Add(grid);
                    else
                        m_playerDrops.Add(grid);
                    m_allDrops.Add(grid);

                    float value;
                    if (currentColor == GoChessColor.White)
                        value = Minimax(GoChessColor.Black, depth - 1, alpha, beta);
                    else
                        value = Minimax(GoChessColor.White, depth - 1, alpha, beta);

                    if (currentColor == m_robotColor)
                        m_robotDrops.Remove(grid);
                    else
                        m_playerDrops.Remove(grid);
                    m_allDrops.Remove(grid);

                    if (value > best)//使最大下限提高的点位作为目标落子点
                    {
                        best = value;
                        bestGrid = grid;
                    }

                    alpha = Mathf.Max(alpha, best);
                    //alpha-beta剪枝点
                    if (beta <= alpha)
                    {
                        Debug.Log($"剪枝时alpha: {alpha}, beta: {beta}");
                        m_cutCnt++;
                        break;
                    }
                }

                if (depth == this.Depth)
                {
                    m_targetGrid = bestGrid;
                }
                return best;
            }
            else
            {
                float best = float.MaxValue;
                for (int i = 0; i < blankList.Count; i++)
                {
                    m_searchCnt++;
                    LocalGridData grid = blankList[i];

                    if (currentColor == m_robotColor)
                        m_robotDrops.Add(grid);
                    else
                        m_playerDrops.Add(grid);
                    m_allDrops.Add(grid);

                    float value;
                    if (currentColor == GoChessColor.White)
                        value = Minimax(GoChessColor.Black, depth - 1, alpha, beta);
                    else
                        value = Minimax(GoChessColor.White, depth - 1, alpha, beta);

                    if (currentColor == m_robotColor)
                        m_robotDrops.Remove(grid);
                    else
                        m_playerDrops.Remove(grid);
                    m_allDrops.Remove(grid);

                    best = Mathf.Min(best, value);
                    beta = Mathf.Min(beta, best);
                    //alpha-beta剪枝点
                    if (beta <= alpha)
                    {
                        Debug.Log($"剪枝时alpha: {alpha}, beta: {beta}");
                        m_cutCnt++;
                        break;
                    }
                }
                return best;
            }
        }

        //估价函数
        private bool Evaluation(GoChessColor targetColor, out int score)
        {
            List<LocalGridData> myList;
            List<LocalGridData> enemyList;
            if (targetColor == m_robotColor)
            {
                myList = m_robotDrops;
                enemyList = m_playerDrops;
            }
            else
            {
                myList = m_playerDrops;
                enemyList = m_robotDrops;
            }

            score = 0;
            //19x19x4 = 1600
            for(int i = 0; i < m_map.Grids.Width; i++)
            {
                for (int j = 0; j < m_map.Grids.Height; j++)
                {
                    for (int k = 0; k < m_directions.Length; k++)
                    {
                        if (CalculateScore(m_map.Grids[i, j], m_directions[k], myList, enemyList, out int myScore))
                        {
                            score += myScore;
                            return true;
                        }
                        score += myScore;
                    }
                }
            }

            if (targetColor != m_robotColor)
            {
                score *= -1;
            }
            return false;
        }

        private bool CalculateScore(LocalGridData grid, Vector2Int direction,
            List<LocalGridData> myList, List<LocalGridData> enemyList, out int score)
        {
            score = 0;

            //单向检测形状并统计得分
            string shape5 = "";
            string shape6 = "";
            var x = 0;
            var y = 0;
            for (int i = 0; i <= 4; i++)
            {
                x = grid.X + i * direction.x;
                y = grid.Z + i * direction.y;
                if (0 <= x && x < m_map.Grids.Width && 0 <= y && y < m_map.Grids.Height)
                {
                    //对方棋子阻隔直接return
                    if (enemyList.Contains(m_map.Grids[x, y]))
                    {
                        return false;
                    }
                    else if (myList.Contains(m_map.Grids[x, y]))
                    {
                        shape5 += '1';
                        shape6 += '1';
                    }
                    else
                    {
                        shape5 += '0';
                        shape6 += '0';
                    }
                }
                //越界直接return
                else
                {
                    return false;
                }
            }

            if (m_shapeWeightMap.ContainsKey(shape5))
            {
                score += m_shapeWeightMap[shape5];

                //长连直接return
                if (shape5.Equals("11111"))
                {
                    Debug.Log($"长连, {grid.X}, {grid.Z}");
                    return true;
                }
            }

            x = grid.X + 5 * direction.x;
            y = grid.Z + 5 * direction.y;
            if (0 <= x && x < m_map.Grids.Width && 0 <= y && y < m_map.Grids.Height)
            {
                if (enemyList.Contains(m_map.Grids[x, y]))
                    return false;
                else if (myList.Contains(m_map.Grids[x, y]))
                    shape6 += '1';
                else
                    shape6 += '0';

                if (m_shapeWeightMap.ContainsKey(shape6))
                {
                    score += m_shapeWeightMap[shape6];
                }
            }

            return false;
        }

        private Vector2Int[] m_directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1)
        };

        private Dictionary<string, int> m_shapeWeightMap = new Dictionary<string, int>()
        {
            {"01100", 5},
            {"00110", 5},

            {"11010", 20},
            {"01011", 20},
            {"00111", 50},
            {"11100", 50},

            {"01110", 100},
            {"010110", 300},
            {"011010", 300},
            {"11101", 300},
            {"10111", 300},
            {"11011", 300},
            {"01111", 500},
            {"11110", 500},
            {"011110", 5000},

            {"11111", 500000}
        };

        private bool HasNeightnor(LocalGridData grid)
        {
            int x = grid.X;
            int z = grid.Z;
            int width = m_map.Grids.Width;
            int height = m_map.Grids.Height;

            for(int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    if (0 <= x + i && x + i < width && 0 <= z + j && z + j < height &&
                        m_allDrops.Contains(m_map.Grids[x + i, z + j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}