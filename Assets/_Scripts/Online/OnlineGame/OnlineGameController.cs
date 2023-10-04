using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop.Online
{
    public class OnlineGameController : NetworkBehaviour
    {
        public GameObject BlackBasket;
        public GameObject WhiteBasket;
        public GameObject Map;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Init();
        }

        [Server]
        protected void Init()
        {
            var bb = Instantiate(BlackBasket);
            var bw = Instantiate(WhiteBasket);
            var map = Instantiate(Map);

            NetworkServer.Spawn(bb, connectionToClient);
            NetworkServer.Spawn(bw, connectionToClient);
            NetworkServer.Spawn(map, connectionToClient);
        }
    }

}