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
        public int Depth = 1;
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

            Minimax(m_robotColor, Depth, float.MinValue, float.MaxValue);

            Debug.Log($"搜索次数:{m_searchCnt}, 剪枝次数:{m_cutCnt}");

            //为null就随机下
            if (m_targetGrid == null)
            {
                var xMax = m_map.Grids.Width;
                var zMax = m_map.Grids.Height;
                var x = Random.Range(0, xMax);
                var z = Random.Range(0, zMax);
                while (m_map.Grids[x, z].Occupied)
                {
                    x = Random.Range(0, xMax);
                    z = Random.Range(0, zMax);
                }
                var piece = m_chessBasket.Get(m_robotColor);
                m_map.Grids[x, z].AttachArea.Attach(piece);
            }
            else
            {
                var piece = m_chessBasket.Get(m_robotColor);
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
                Debug.Log($"长连: {2 * score}");
                return 2 * score;
            }

            var blankList = m_grids.Where(grid => !m_allDrops.Contains(grid)).ToList();

            if (currentColor == m_robotColor)
            {
                float best = float.MinValue;

                for (int i = 0; i < blankList.Count; i++)
                {
                    //此落子处周围无相邻棋子则跳过
                    if (!HasNeightnor(blankList[i])) continue;

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
                        m_targetGrid = grid;
                    }

                    alpha = Mathf.Max(alpha, best);
                    //alpha-beta剪枝点
                    if (beta <= alpha)
                    {
                        Debug.Log($"alpha: {alpha}, beta: {beta}");
                        m_cutCnt++;
                        break;
                    }
                }
                return best;
            }
            else
            {
                float best = float.MaxValue;

                for (int i = 0; i < blankList.Count; i++)
                {
                    //此落子处周围无相邻棋子则跳过
                    if (!HasNeightnor(blankList[i])) continue;
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

            //本方得分
            //int myScore = 0;
            //for (int i = 0; i < myList.Count; i++)
            //{
            //    for(int j = 0; j < m_directions.Length; j++)
            //    {
            //       if(CalculateScore(myList[i], m_directions[j], myList, enemyList, out myScore))
            //       {
            //            return true;
            //       }
            //    }
            //}

            //int enemyScore = 0;
            //for (int i = 0; i < enemyList.Count; i++)
            //{
            //    for (int j = 0; j < m_directions.Length; j++)
            //    {
            //        if (CalculateScore(enemyList[i], m_directions[j], enemyList, myList, out enemyScore))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //score = myScore - enemyScore / 10;

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

            //按某个方向检测形状
            for (int i = -5; i <= 0; i++)
            {
                string shape6 = "";
                for(int j = 0; j <= 5; j++)
                {
                    var x = grid.X + (i + j) * direction.x;
                    var y = grid.Z + (i + j) * direction.y;
                    if (0 <= x && x < m_map.Grids.Width && 0 <= y && y < m_map.Grids.Height)
                    {
                        if (enemyList.Contains(m_map.Grids[x, y]))
                        {
                            break;
                        }
                        else if (myList.Contains(m_map.Grids[x, y]))
                        {
                            shape6 += '1';
                        }
                        else
                        {
                            shape6 += '0';
                        }
                    }
                }
                if (m_shapeWeightMap.ContainsKey(shape6))
                {
                    score += m_shapeWeightMap[shape6];
                }
            }

            //按某个方向检测形状
            for (int i = -4; i <= 0; i++)
            {
                string shape5 = "";
                for (int j = 0; j <= 4; j++)
                {
                    var x = grid.X + (i + j) * direction.x;
                    var y = grid.Z + (i + j) * direction.y;
                    if (0 <= x && x < m_map.Grids.Width && 0 <= y && y < m_map.Grids.Height)
                    {
                        if (enemyList.Contains(m_map.Grids[x, y]))
                        {
                            break;
                        }
                        else if (myList.Contains(m_map.Grids[x, y]))
                        {
                            shape5 += '1';
                        }
                        else
                        {
                            shape5 += '0';
                        }
                    }
                }

                if (m_shapeWeightMap.ContainsKey(shape5))
                {
                    score += m_shapeWeightMap[shape5];
                    if (shape5.Equals("11111"))
                    {
                        Debug.Log($"长连, {grid.X}, {grid.Z}");
                        return true;
                    }
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