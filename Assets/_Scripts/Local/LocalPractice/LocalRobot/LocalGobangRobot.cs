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
            InitValueMap();

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

        private void CalculateValue()
        {



        }

        private Dictionary<string, int> m_valueMap;
        private void InitValueMap()
        {
            m_valueMap = new Dictionary<string, int>();

            //¿Õ-0£»ºÚ-1£»°×-2
            m_valueMap["1"] = 20;
            m_valueMap["11"] = 400;
            m_valueMap["111"] = 500;
            m_valueMap["1111"] = 8000;
            m_valueMap["2"] = 20;
            m_valueMap["22"] = 400;
            m_valueMap["222"] = 500;
            m_valueMap["2222"] = 8000;

            m_valueMap["12"] = 8;
            m_valueMap["112"] = 80;
            m_valueMap["1112"] = 450;
            m_valueMap["11112"] = 8000;

            m_valueMap["21"] = 8;
            m_valueMap["221"] = 80;
            m_valueMap["2221"] = 450;
            m_valueMap["22221"] = 8000;
        }


    }
}
