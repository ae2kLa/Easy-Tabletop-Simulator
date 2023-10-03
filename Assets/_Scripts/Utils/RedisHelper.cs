using StackExchange.Redis;
namespace Tabletop
{
    class RedisHelper
    {
        //120.76.196.221:6379
        private static readonly ConfigurationOptions ConfigurationOptions =
            ConfigurationOptions.Parse("localhost:6379,password=8Mk_2pPowhjbiwj82nbMa_jwbuBfKiszq");
        private static readonly object Locker = new object();
        private static ConnectionMultiplexer _redisConn;

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer RedisConn
        {
            get
            {
                if (_redisConn == null)
                {
                    // 锁定某一代码块，让同一时间只有一个线程访问该代码块
                    lock (Locker)
                    {
                        if (_redisConn == null || !_redisConn.IsConnected)
                        {
                            _redisConn = ConnectionMultiplexer.Connect(ConfigurationOptions);
                        }
                    }
                }
                return _redisConn;
            }
        }

    }
}