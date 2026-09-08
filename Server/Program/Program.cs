using Server;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Server.StartServer();
        }
    }
}
