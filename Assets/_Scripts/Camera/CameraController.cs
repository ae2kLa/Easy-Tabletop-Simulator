using UnityEngine;
using QFramework;

namespace Tabletop
{
	public partial class CameraController : ViewController, ISingleton
	{
        private static CameraController m_instance;
        public static CameraController Instance => m_instance;

		private void Awake()
		{
            if (m_instance == null)
            {
                m_instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void OnSingletonInit()
        {

        }

        void Start()
		{

		}
	}
}
