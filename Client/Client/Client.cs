using System.Text;
using System.Text.Json;

namespace Client
{
    public class Client
    {
        public static async Task Run()
        {
            Console.Write("Введите адрес сервера или нажмите enter для адреса по умолчанию(http://localhost:3000/)\n");
            string serverUrl = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(serverUrl))
            {
                serverUrl = "http://localhost:3000/";
            }

            Console.WriteLine("Клиент запущен. Подключение к серверу...\n");

            while (true)
            {
                Console.WriteLine("1 - Создать заказ");
                Console.WriteLine("2 - Кастомизировать букет");
                Console.WriteLine("3 - Назначить флориста");
                Console.WriteLine("4 - Организовать доставку");
                Console.WriteLine("5 - Просмотр букетов");
                Console.WriteLine("6 - Просмотр заказов");
                Console.WriteLine("7 - Просмотр клиентов");
                Console.WriteLine("8 - Просмотр флористов");
                Console.WriteLine("9 - Выход");
                Console.Write("Выберите операцию (1-9): ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "9")
                {
                    Console.WriteLine("exit");
                    break;
                }

                RequestBuilder requestBuilder = new RequestBuilder();

                switch (choice)
                {
                    case "1":
                        await CreateOrder(requestBuilder, serverUrl);
                        break;
                    case "2":
                        await CustomizeBouquet(requestBuilder, serverUrl);
                        break;
                    case "3":
                        await AssignFlorist(requestBuilder, serverUrl);
                        break;
                    case "4":
                        await ArrangeDelivery(requestBuilder, serverUrl);
                        break;
                    case "5":
                        await requestBuilder.HandleRequest("GetBouquets", "", serverUrl);
                        break;
                    case "6":
                        await requestBuilder.HandleRequest("GetOrders", "", serverUrl);
                        break;
                    case "7":
                        await requestBuilder.HandleRequest("GetCustomers", "", serverUrl);
                        break;
                    case "8":
                        await requestBuilder.HandleRequest("GetFlorists", "", serverUrl);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        private static async Task CreateOrder(RequestBuilder requestBuilder, string serverUrl)
        {
            Console.WriteLine("ID клиента: ");
            string customerIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(customerIdStr, out int customerId))
            {
                Console.WriteLine("Ошибка: ID клиента должен быть числом");
                return;
            }

            Console.WriteLine("ID букета: ");
            string bouquetIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(bouquetIdStr, out int bouquetId))
            {
                Console.WriteLine("Ошибка: ID букета должен быть числом");
                return;
            }

            Console.Write("Особые пожелания: ");
            string requirements = Console.ReadLine() ?? "";

            var data = new
            {
                CustomerId = customerId,
                BouquetId = bouquetId,
                SpecialRequirements = requirements
            };

            string jsonData = JsonSerializer.Serialize(data);
            await requestBuilder.HandleRequest("CreateOrder", jsonData, serverUrl);
        }

        private static async Task CustomizeBouquet(RequestBuilder requestBuilder, string serverUrl)
        {
            Console.WriteLine("ID заказа: ");
            string orderIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(orderIdStr, out int orderId))
            {
                Console.WriteLine("Ошибка: ID заказа должен быть числом");
                return;
            }

            Console.WriteLine("Что добавить ещё в букет?(например розы)");
            string customization = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(customization))
            {
                Console.WriteLine("Ошибка: Укажите что добавить в букет");
                return;
            }

            Console.WriteLine("Дополнительная стоимость: ");
            string priceStr = Console.ReadLine() ?? "";
            if (!decimal.TryParse(priceStr, out decimal price) || price < 0)
            {
                Console.WriteLine("Ошибка: Дополнительная стоимость должна быть положительным числом");
                return;
            }

            var data = new
            {
                OrderId = orderId,
                Customization = customization,
                AdditionalPrice = price
            };

            string jsonData = JsonSerializer.Serialize(data);
            await requestBuilder.HandleRequest("CustomizeBouquet", jsonData, serverUrl);
        }

        private static async Task AssignFlorist(RequestBuilder requestBuilder, string serverUrl)
        {
            Console.WriteLine("ID заказа: ");
            string orderIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(orderIdStr, out int orderId))
            {
                Console.WriteLine("Ошибка: ID заказа должен быть числом");
                return;
            }

            Console.WriteLine("ID флориста: ");
            string floristIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(floristIdStr, out int floristId))
            {
                Console.WriteLine("Ошибка: ID флориста должен быть числом");
                return;
            }

            var data = new
            {
                OrderId = orderId,
                FloristId = floristId
            };

            string jsonData = JsonSerializer.Serialize(data);
            await requestBuilder.HandleRequest("AssignFlorist", jsonData, serverUrl);
        }

        private static async Task ArrangeDelivery(RequestBuilder requestBuilder, string serverUrl)
        {
            Console.WriteLine("ID заказа: ");
            string orderIdStr = Console.ReadLine() ?? "";
            if (!int.TryParse(orderIdStr, out int orderId))
            {
                Console.WriteLine("Ошибка: ID заказа должен быть числом");
                return;
            }

            Console.WriteLine("Адрес доставки: ");
            string address = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(address))
            {
                Console.WriteLine("Ошибка: Адрес доставки не может быть пустым");
                return;
            }

            Console.WriteLine("Дата доставки (ГГГГ-ММ-ДД ЧЧ:ММ): ");
            string dateStr = Console.ReadLine() ?? "";
            if (!DateTime.TryParse(dateStr, out DateTime deliveryDate))
            {
                Console.WriteLine("Ошибка: Неверный формат даты. Используйте ГГГГ-ММ-ДД ЧЧ:ММ");
                return;
            }

            Console.WriteLine("Имя курьера: ");
            string courierName = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(courierName))
            {
                Console.WriteLine("Ошибка: Имя курьера не может быть пустым");
                return;
            }

            Console.WriteLine("Телефон курьера: ");
            string courierPhone = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(courierPhone))
            {
                Console.WriteLine("Ошибка: Телефон курьера не может быть пустым");
                return;
            }

            Console.WriteLine("Примечания: ");
            string notes = Console.ReadLine() ?? "";

            var data = new
            {
                OrderId = orderId,
                Address = address,
                DeliveryDate = deliveryDate,
                CourierName = courierName,
                CourierPhone = courierPhone,
                Notes = notes
            };

            string jsonData = JsonSerializer.Serialize(data);
            await requestBuilder.HandleRequest("ArrangeDelivery", jsonData, serverUrl);
        }
    }
}