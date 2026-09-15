using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public class Actions
    {
        private static List<Customer> customers = new List<Customer>();
        private static List<Bouquet> bouquets = new List<Bouquet>();
        private static List<Order> orders = new List<Order>();
        private static List<Florist> florists = new List<Florist>();
        private static List<Delivery> deliveries = new List<Delivery>();
        private static int customerId = 1;
        private static int bouquetId = 1;
        private static int floristId = 1;
        private static int orderId = 1;
        private static int deliveryId = 1;

        public static event Action<Order> OrderCreated;
        public static event Action<Order> BouquetPrepared;
        public static event Action<Order> FloristAssigned;
        public static event Action<Order> DeliveryArranged;

        public Actions() { }
        public string GetBouquets() => JsonSerializer.Serialize(bouquets);
        public string GetCustomers() => JsonSerializer.Serialize(customers);
        public string GetFlorists() => JsonSerializer.Serialize(florists);
        public string GetOrders() => JsonSerializer.Serialize(orders);

        public void InitializeData()
        {
            customers.AddRange(new[] {
                new Customer { Id = customerId++, Name = "Анна Петрова", Phone = "+375336482312", Email = "anna@gmail.com", Address = "ул. Ленина, 15" },
                new Customer { Id = customerId++, Name = "Иван Сидоров", Phone = "+375336482231", Email = "ivan@gmail.com", Address = "ул. Скабин, 28" }
            });

            bouquets.AddRange(new[] {
                new Bouquet { Id = bouquetId++, Name = "Романтический", Description = "Букет из красных роз", Price = 350, FlowersComposition = "25 красных роз", Quantity = 10 },
                new Bouquet { Id = bouquetId++, Name = "Весенний", Description = "Яркий весенний букет", Price = 280, FlowersComposition = "Тюльпаны, нарциссы", Quantity = 8 },
                new Bouquet { Id = bouquetId++, Name = "Эксклюзивный", Description = "Авторский букет", Price = 550, FlowersComposition = "Орхидеи, лилии", Quantity = 5 }
            });

            florists.AddRange(new[] {
                new Florist { Id = floristId++, Name = "Дмитрий", Surname = "Иванов", Experience = 5, Specialty = "Свадебные букеты", IsAvailable = true },
                new Florist { Id = floristId++, Name = "Мария", Surname = "Рыжова", Experience = 3, Specialty = "Композиции", IsAvailable = true }
            });
        }
        public string CreateOrder(string jsonData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<CreateOrderData>(jsonData);

                if (data == null)
                    throw new Exception("Некорректные данные заказа");

                var customer = customers.FirstOrDefault(c => c.Id == data.CustomerId);
                if (customer == null)
                    throw new Exception($"Клиент с ID {data.CustomerId} не найден");

                var bouquet = bouquets.FirstOrDefault(b => b.Id == data.BouquetId);
                if (bouquet == null)
                    throw new Exception($"Букет с ID {data.BouquetId} не найден");

                if (bouquet.Quantity <= 0)
                    throw new Exception("Букет отсутствует в наличии");

                var order = new Order
                {
                    Id = orderId++,
                    Customer = customer,
                    Bouquet = bouquet,
                    SpecialRequirements = data.SpecialRequirements ?? "",
                    TotalPrice = bouquet.Price + 50,
                    Status = "New"
                };

                bouquet.Quantity--;
                orders.Add(order);

                OrderCreated?.Invoke(order);

                return JsonSerializer.Serialize(new { OrderId = order.Id, Total = order.TotalPrice });
            }
            catch (JsonException)
            {
                throw new Exception("Некорректный формат данных заказа");
            }
        }

        public string CustomizeBouquet(string jsonData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<CustomizeData>(jsonData);

                if (data == null)
                    throw new Exception("Некорректные данные кастомизации");

                var order = orders.FirstOrDefault(o => o.Id == data.OrderId);
                if (order == null)
                    throw new Exception($"Заказ с ID {data.OrderId} не найден");

                order.Bouquet.FlowersComposition += $", + {data.Customization}";
                order.Bouquet.Price += data.AdditionalPrice;
                order.TotalPrice += data.AdditionalPrice;

                BouquetPrepared?.Invoke(order);

                return JsonSerializer.Serialize(new { Message = "Букет изменен", NewPrice = order.TotalPrice });
            }
            catch (JsonException)
            {
                throw new Exception("Некорректный формат данных кастомизации");
            }
        }

        public string AssignFlorist(string jsonData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<AssignData>(jsonData);

                if (data == null)
                    throw new Exception("Некорректные данные назначения");

                var order = orders.FirstOrDefault(o => o.Id == data.OrderId);
                if (order == null)
                    throw new Exception($"Заказ с ID {data.OrderId} не найден");

                var florist = florists.FirstOrDefault(f => f.Id == data.FloristId && f.IsAvailable);
                if (florist == null)
                    throw new Exception($"Флорист с ID {data.FloristId} не найден или недоступен");

                order.Florist = florist;
                florist.AssignedOrders.Add(order);
                order.Status = "Processing";

                FloristAssigned?.Invoke(order);

                return JsonSerializer.Serialize(new { Message = $"Назначен флорист {florist.Name} {florist.Surname}" });
            }
            catch (JsonException)
            {
                throw new Exception("Некорректный формат данных назначения");
            }
        }

        public string ArrangeDelivery(string jsonData)
        {
            try
            {
                var data = JsonSerializer.Deserialize<DeliveryData>(jsonData);

                if (data == null)
                    throw new Exception("Некорректные данные доставки");

                var order = orders.FirstOrDefault(o => o.Id == data.OrderId);
                if (order == null)
                    throw new Exception($"Заказ с ID {data.OrderId} не найден");

                if (string.IsNullOrEmpty(data.Address))
                    throw new Exception("Адрес доставки не может быть пустым");

                var delivery = new Delivery
                {
                    Id = deliveryId++,
                    Address = data.Address,
                    DeliveryDate = data.DeliveryDate,
                    DeliveryCost = 50,
                    CourierName = data.CourierName ?? "",
                    CourierPhone = data.CourierPhone ?? "",
                    Notes = data.Notes ?? "",
                    Status = "Pending"
                };

                order.Delivery = delivery;
                deliveries.Add(delivery);
                order.Status = "Delivered";

                DeliveryArranged?.Invoke(order);

                return JsonSerializer.Serialize(new
                {
                    Message = "Доставка организована",
                    DeliveryId = delivery.Id,
                    Date = delivery.DeliveryDate
                });
            }
            catch (JsonException)
            {
                throw new Exception("Некорректный формат данных доставки");
            }
        }
    }
}
