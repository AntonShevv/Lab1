using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
    public class RequestBuilder
    {
        private static HttpClient httpClient = new HttpClient();

        public async Task HandleRequest(string operation, string data, string serverUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(serverUrl))
                {
                    Console.WriteLine("Ошибка: адрес сервера не указан");
                    return;
                }

                var requestData = new { Operation = operation, Data = data };
                string jsonRequest = JsonSerializer.Serialize(requestData);

                using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsync(serverUrl, content);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
                    return;
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Ошибка: таймаут подключения к серверу");
                    Console.WriteLine($"Детали: {ex.Message}");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка: сервер вернул статус {response.StatusCode}");
                    return;
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                ResponseData responseData;
                try
                {
                    responseData = JsonSerializer.Deserialize<ResponseData>(responseBody);
                }
                catch (JsonException)
                {
                    Console.WriteLine("Ошибка: некорректный формат ответа от сервера");
                    Console.WriteLine($"Ответ: {responseBody}");
                    return;
                }

                if (responseData != null && responseData.Success)
                {
                    Console.WriteLine($"Статус: успешно");
                    string formattedResult = FormatJson(responseData.Result);
                    Console.WriteLine($"Результат:\n{formattedResult}");
                }
                else
                {
                    Console.WriteLine($"Статус: ошибка");
                    Console.WriteLine($"Сообщение: {responseData?.Error ?? "Неизвестная ошибка"}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка подключения к серверу: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка обработки ответа: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Ошибка: превышено время ожидания ответа от сервера");
                Console.WriteLine($"Детали: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
                Console.WriteLine($"Тип ошибки: {ex.GetType().Name}");
            }
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