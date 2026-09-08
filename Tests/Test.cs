using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class FlowerShopTests : IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _serverUrl = "http://localhost:3000/";
        private bool _disposed;

        public FlowerShopTests()
        {
            _client = new HttpClient();
        }

        [Fact]
        public async Task Test1_GetBouquets_Success()
        {
            if (!await IsServerRunning())
            {
                throw new Exception("Сервер не запущен. Запустите сервер перед выполнением тестов.");
            }

            var requestData = new { Operation = "GetBouquets", Data = "" };
            string jsonRequest = JsonSerializer.Serialize(requestData);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_serverUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseData>(responseBody);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Result);

            var bouquets = JsonSerializer.Deserialize<List<Bouquet>>(result.Result);
            Assert.NotNull(bouquets);
            Assert.NotEmpty(bouquets);

            foreach (var b in bouquets)
            {
                Assert.True(b.Id > 0);
                Assert.NotNull(b.Name);
                Assert.True(b.Price >= 0);
            }
        }

        [Fact]
        public async Task Test2_UnknownOperation_ReturnsError()
        {
            if (!await IsServerRunning())
            {
                throw new Exception("Сервер не запущен. Запустите сервер перед выполнением тестов.");
            }

            var requestData = new { Operation = "DeleteAllOrders", Data = "" };
            string jsonRequest = JsonSerializer.Serialize(requestData);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_serverUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseData>(responseBody);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("Неизвестная операция", result.Error);
        }

        [Fact]
        public async Task Test3_InvalidJson_ReturnsError()
        {
            if (!await IsServerRunning())
            {
                throw new Exception("Сервер не запущен. Запустите сервер перед выполнением тестов.");
            }

            string invalidJson = "{qwe :23}";
            using var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_serverUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseData>(responseBody);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("Некорректный формат JSON", result.Error);
        }

        [Fact]
        public async Task Test4_ServerUnavailable_ThrowsException()
        {
            string wrongUrl = "http://localhost:3001/";
            var requestData = new { Operation = "GetBouquets", Data = "" };
            string jsonRequest = JsonSerializer.Serialize(requestData);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            using var client = new HttpClient();

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                client.PostAsync(wrongUrl, content)
            );

            Assert.Contains("Подключение не установлено, т.к. конечный", exception.Message);
        }

        private async Task<bool> IsServerRunning()
        {
            try
            {
                var requestData = new { Operation = "GetBouquets", Data = "" };
                string jsonRequest = JsonSerializer.Serialize(requestData);
                using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_serverUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _client?.Dispose();
            }
            _disposed = true;
        }

        private class ResponseData
        {
            public bool Success { get; set; }
            public string Result { get; set; }
            public string Error { get; set; }
        }

        private class Bouquet
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
}