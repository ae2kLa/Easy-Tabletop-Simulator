using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Local
{
    public class LocalGobangRobot : IRobot
    {
        public LocalGobangRobot(GoChessColor color, LocalGoChessBasket chessBasket, LocalMapObj map)
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
        }

        private GoChessColor m_robotColor;
        private LocalGoChessBasket m_chessBasket;
        private LocalMapObj m_map;

        public IEnumerator OnTurnToRobot()
        {
            var xMax = m_map.Grids.Width;
            var zMax = m_map.Grids.Height;
            var x = Random.Range(0, xMax);
            var z = Random.Range(0, zMax);
            while (m_map.Grids[x, z].Occupied)
            {
                yield return null;
                x = Random.Range(0, xMax);
                z = Random.Range(0, zMax);
            }
            var piece = m_chessBasket.Get(m_robotColor);
            m_map.Grids[x, z].AttachArea.Attach(piece);
        }

    }
}
