using System.Net;
using System.Text;
using System.Text.Json;
namespace Server
{
    public class Server
    {
        private static HttpListener server = new HttpListener();

        public static async Task StartServer()
        {
            server.Prefixes.Add("http://localhost:3000/");
            server.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                try
                {
                    var context = await server.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;
                    RequestHandler requestHandler = new RequestHandler();
                    _ = Task.Run(() => requestHandler.HandleRequest(request,response));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

    }
}
