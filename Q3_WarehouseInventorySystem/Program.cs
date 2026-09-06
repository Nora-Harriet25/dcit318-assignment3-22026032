using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventorySystem
{
    // ===== a. Marker interface all inventory items must implement =====
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ===== b. ElectronicItem =====
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    // ===== c. GroceryItem =====
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    // ===== e. Custom exceptions =====
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // ===== d. Generic inventory repository =====
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"An item with ID {item.Id} already exists.");
            }

            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
            {
                throw new ItemNotFoundException($"Item with ID {id} was not found.");
            }

            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Cannot remove: item with ID {id} was not found.");
            }

            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return _items.Values.ToList();
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }

            var item = GetItemById(id); // reuses lookup + throws ItemNotFoundException if missing
            item.Quantity = newQuantity;
        }
    }

    // ===== f. WareHouseManager - manages both grocery and electronic inventories =====
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 50, "Sony", 6));

            _groceries.AddItem(new GroceryItem(1, "Rice (5kg)", 100, DateTime.Now.AddMonths(6)));
            _groceries.AddItem(new GroceryItem(2, "Milk (1L)", 40, DateTime.Now.AddDays(14)));
            _groceries.AddItem(new GroceryItem(3, "Canned Beans", 60, DateTime.Now.AddMonths(12)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased for item {id}. New quantity: {item.Quantity}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item {id} removed successfully.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Warehouse Inventory Management System ===\n");

            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("--- All Grocery Items ---");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("\n--- All Electronic Items ---");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("\n--- Testing Error Handling ---");

            // i. Try adding a duplicate item
            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // ii. Try removing a non-existent item
            manager.RemoveItemById(manager.Groceries, 99);

            // iii. Try updating with an invalid (negative) quantity
            try
            {
                manager.Electronics.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
