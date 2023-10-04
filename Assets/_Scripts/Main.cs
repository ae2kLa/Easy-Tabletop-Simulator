using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tabletop
{
    public class Main : MonoBehaviour
    {
        private void OnGUI()
        {

            GUILayout.BeginArea(new Rect(0, 100, Screen.width, Screen.height));
            if (GUI.Button(new Rect(20, 40, 100, 20), "新手教程"))
            {
                SceneManager.LoadScene("Practice");
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, 200, Screen.width, Screen.height));
            if (GUI.Button(new Rect(20, 40, 100, 20), "在线对弈"))
            {
                SceneManager.LoadScene("Offline");
            }
            GUILayout.EndArea();

        }
    }
}