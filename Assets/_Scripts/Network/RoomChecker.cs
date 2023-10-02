using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tabletop
{
    public class RoomChecker : MonoBehaviour
    {
        void Start()
        {
            //using (ConnectionMultiplexer conn = RedisHelper.RedisConn)
            //{
            //    var db = conn.GetDatabase(); //在Redis中获得与数据库的交互式连接
            //    //db.StringSet("测试", "更新你的房间/端口状态");

            //    for(int i = 1004; i <= 1007; i++)
            //    {
            //        var key = "Room" + i.ToString();
            //        if (db.KeyExists(key))
            //        {
            //            string state = db.StringGet(key);
            //            //if(state.Equals("Available"))
            //            //{

            //            //}
            //            print($"{i}: {state}");
            //        }
            //    }
            //}
        }
    }
}