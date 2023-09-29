using Mirror;
using UnityEngine;

namespace Tabletop
{
    internal class Spawner
    {
        [ServerCallback]
        internal static void InitialSpawn()
        {
            SpawnReward();
        }

        [ServerCallback]
        internal static void SpawnReward()
        {
            //NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity));
        }
    }
}
