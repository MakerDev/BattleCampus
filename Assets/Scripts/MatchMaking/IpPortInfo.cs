namespace BattleCampusMatchServer.Models
{
    public class IpPortInfo
    {
        public string IpAddress { get; set; } = "localhost";
        public int DesktopPort { get; set; } = 7777;
        public int WebsocketPort { get; set; } = 7778;
    }
}
