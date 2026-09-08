using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Нажмите Enter для запуска тестов...");
            Console.ReadLine();

            var testClass = new FlowerShopTests();

            try
            {
                await testClass.Test1_GetBouquets_Success();
                Console.WriteLine("Тест 1 (Успешный сценарий) пройден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест 1 провален: {ex.Message}");
            }

            try
            {
                await testClass.Test2_UnknownOperation_ReturnsError();
                Console.WriteLine("Тест 2 (Неизвестная операция) пройден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест 2 провален: {ex.Message}");
            }

            try
            {
                await testClass.Test3_InvalidJson_ReturnsError();
                Console.WriteLine("Тест 3 (Некорректные данные) пройден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест 3 провален: {ex.Message}");
            }

            try
            {
                await testClass.Test4_ServerUnavailable_ThrowsException();
                Console.WriteLine("Тест 4 (Недоступный сервер) пройден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Тест 4 провален: {ex.Message}");
            }

            testClass.Dispose();

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }
    }
}