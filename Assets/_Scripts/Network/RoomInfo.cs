namespace Tabletop
{
    public struct RoomInfo
    {
        public string RoomName;
        public ushort Port;
        public RoomState RoomState;

        public RoomInfo(string roomName, ushort port, RoomState roomState)
        {
            RoomName = roomName;
            Port = port;
            RoomState = roomState;
        }
    }
}