using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tabletop.Online
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        public override void Start()
        {
            base.Start();

            //更新Redis
            if (NetworkManager.singleton is NetworkRoomManagerExt room && room.roomSlots.Count == room.minPlayers)
            {
                room.SetRedisValue(OnlineRoomState.Full);
                print($"我在执行更新Redis: RoomState.Full, port: {(room.transport as KcpTransport).port}");
            }
        }

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