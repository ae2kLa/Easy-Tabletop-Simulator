using UnityEngine;

namespace Tabletop.Local
{
    public class LocalPracticeController : MonoBehaviour
    {
        public GameObject BlackBasket;
        public GameObject WhiteBasket;
        public GameObject Map;

        public static LocalPracticeController Instance;
        public GoChessColor PlayerColor;
        private IRobot m_robot;

        public void Awake()
        {
            Instance = this;
            Init();
        }

        private LocalGoChessBasket m_blackBasket;
        private LocalGoChessBasket m_whiteBasket;
        private LocalMapObj m_map;
        protected void Init()
        {
            m_blackBasket = Instantiate(BlackBasket).GetComponent<LocalGoChessBasket>();
            m_whiteBasket = Instantiate(WhiteBasket).GetComponent<LocalGoChessBasket>();
            m_map = Instantiate(Map).GetComponent<LocalMapObj>();
        }

        private void Start()
        {
            if (PlayerColor == GoChessColor.White)
            {
                m_robot = new LocalGobangRobot(GoChessColor.Black, m_blackBasket, m_map);
            }
            else if (PlayerColor == GoChessColor.Black)
            {
                m_robot = new LocalGobangRobot(GoChessColor.White, m_whiteBasket, m_map);
            }
            else
            {
                print("Íæ¼ÒÎ´Ñ¡ÔñÑÕÉ«");
            }
        }

    }

}