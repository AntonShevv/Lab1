using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public class RequestHandler
    {
        private static Actions action = new Actions();
        public static event Action<Order> OrderCreated;
        public static event Action<Order> BouquetPrepared;
        public static event Action<Order> FloristAssigned;
        public static event Action<Order> DeliveryArranged;
        public static bool isInitialized = false;
        public RequestHandler()
        {
            if (!isInitialized)
            {
                action.InitializeData();
                isInitialized = true;
            }

            OrderCreated += OnOrderCreated;
            BouquetPrepared += OnBouquetPrepared;
            FloristAssigned += OnFloristAssigned;
            DeliveryArranged += OnDeliveryArranged;
        }

        public async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                string formattedRequest = FormatJson(requestBody);
                Console.WriteLine($"Получен запрос: {request.Url}");
                Console.WriteLine($"Данные: {formattedRequest}");

                RequestData requestData;
                try
                {
                    requestData = JsonSerializer.Deserialize<RequestData>(requestBody);
                }
                catch (JsonException)
                {
                    SendError(response, "Некорректный формат JSON");
                    return;
                }

                if (requestData == null || string.IsNullOrEmpty(requestData.Operation))
                {
                    SendError(response, "Некорректный запрос. Отсутствует операция.");
                    return;
                }

                string result;
                try
                {
                    switch (requestData.Operation)
                    {
                        case "GetBouquets":
                            result = action.GetBouquets();
                            break;
                        case "GetCustomers":
                            result = action.GetCustomers();
                            break;
                        case "GetFlorists":
                            result = action.GetFlorists();
                            break;
                        case "GetOrders":
                            result = action.GetOrders();
                            break;
                        case "CreateOrder":
                            result = action.CreateOrder(requestData.Data ?? "");
                            break;
                        case "CustomizeBouquet":
                            result = action.CustomizeBouquet(requestData.Data ?? "");
                            break;
                        case "AssignFlorist":
                            result = action.AssignFlorist(requestData.Data ?? "");
                            break;
                        case "ArrangeDelivery":
                            result = action.ArrangeDelivery(requestData.Data ?? "");
                            break;
                        default:
                            SendError(response, $"Неизвестная операция: {requestData.Operation}");
                            return;
                    }
                }
                catch (Exception ex)
                {
                    SendError(response, $"Ошибка выполнения операции: {ex.Message}");
                    return;
                }

                string formattedResult = FormatJson(result);
                Console.WriteLine($"Ответ отправлен. Результат: {formattedResult}");

                var responseData = new ResponseData { Result = result, Success = true };
                SendResponse(response, responseData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                SendError(response, $"Внутренняя ошибка сервера: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }


        private void OnOrderCreated(Order order)
        {
            Console.WriteLine($"Событие OnOrderCreated");
            Console.WriteLine($"Букет: {order.Bouquet.Name}, сумма: {order.TotalPrice}");
        }

        private void OnBouquetPrepared(Order order)
        {
            Console.WriteLine($"Событие OnBouquetPrepared");
            Console.WriteLine($"Состав: {order.Bouquet.FlowersComposition}");
        }

        private void OnFloristAssigned(Order order)
        {
            Console.WriteLine($"Событие OnFloristAssigned");
            Console.WriteLine($"Флорист {order.Florist.Name} назначен на заказ {order.Id}");
        }

        private void OnDeliveryArranged(Order order)
        {
            Console.WriteLine($"Событие OnDeliveryArranged");
            Console.WriteLine($"Адрес: {order.Delivery.Address}, дата: {order.Delivery.DeliveryDate:dd.MM.yyyy HH:mm}");
        }

        private void SendResponse(HttpListenerResponse response, object data)
        {
            var json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void SendError(HttpListenerResponse response, string message)
        {
            var errorData = new ResponseData { Success = false, Error = message };
            SendResponse(response, errorData);
        }

        private string FormatJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            try
            {
                using var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                return JsonSerializer.Serialize(doc.RootElement, options);
            }
            catch
            {
                return json;
            }
        }
    }
}


