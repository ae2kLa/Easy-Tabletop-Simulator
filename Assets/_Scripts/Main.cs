using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tabletop
{
    public class Main : MonoBehaviour
    {
        public bool showOptions = false;

        private void OnGUI() 
        {
            GUILayout.BeginArea(new Rect(0, 200, Screen.width, Screen.height));
            //if (GUI.Button(new Rect(20, 40, 100, 20), "新手教程"))
            //{
            //    SceneManager.LoadScene("Practice");
            //}

            if (!showOptions && GUI.Button(new Rect(20, 60, 100, 20), "对弈"))
            {
                showOptions = true;
            }

            if (showOptions && GUI.Button(new Rect(20, 80, 100, 20), "本地人机对弈"))
            {
                showOptions = false;
                SceneManager.LoadScene("Practice");
            }
            if (showOptions && GUI.Button(new Rect(20, 100, 100, 20), "在线人人对弈"))
            {
                showOptions = false;
                SceneManager.LoadScene("Offline");
            }
            if (showOptions && GUI.Button(new Rect(20, 120, 100, 20), "返回"))
            {
                showOptions = false;
            }
            GUILayout.EndArea();
        }

        
    }
}