using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tabletop
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        #region 各种无参回调
        public override void OnStartClient()
        {
            //Debug.Log($"OnStartClient {gameObject}");
        }

        public override void OnClientEnterRoom()
        {
            //Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
        }

        //触发退出的客户端上的所有Player都会回调此方法
        public override void OnClientExitRoom()
        {
            //Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
        }
        #endregion

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            //Debug.Log($"IndexChanged {newIndex}");
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            //Debug.Log($"ReadyStateChanged {newReadyState}");
        }

        public override void OnGUI()
        {
            base.OnGUI();
        }


    }
}
