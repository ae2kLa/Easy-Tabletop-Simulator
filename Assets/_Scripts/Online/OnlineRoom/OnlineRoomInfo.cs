namespace Tabletop.Online
{
    public struct OnlineRoomInfo
    {
        public string Name;
        public OnlineRoomState State;
        public ushort Port;

        public OnlineRoomInfo(string name, ushort port, OnlineRoomState state)
        {
            Name = name;
            Port = port;
            State = state;
        }

        ////通过 json 获取到的 string 值给本实体类的枚举赋值
        //public string State
        //{
        //    set
        //    {
        //        m_state = (RoomState)System.Enum.Parse(typeof(RoomState), value);
        //    }
        //}
    }
}