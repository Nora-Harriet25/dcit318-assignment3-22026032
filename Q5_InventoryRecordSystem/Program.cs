using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordSystem
{
    // ===== b. Marker interface for anything with an Id, used for logging =====
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // ===== a. Immutable record representing an inventory item =====
    // Using positional syntax; records give us built-in immutability and value equality
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // ===== c. Generic inventory logger =====
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"No data file found at '{_filePath}'.");
                    return;
                }

                using (var reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(json);

                    _log.Clear();
                    if (items != null)
                    {
                        _log.AddRange(items);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // ===== f. InventoryApp - integration layer =====
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Office Chair", 15, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Desk Lamp", 30, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Whiteboard", 8, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Printer Paper (Ream)", 100, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Stapler", 20, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:g}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Inventory Record System ===\n");

            string filePath = "inventory.json";

            // First "session": create and persist data
            var app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();
            Console.WriteLine("Sample data seeded and saved to file.\n");

            // Simulate a new session: create a fresh InventoryApp instance
            // (its logger's in-memory list starts empty, forcing a real reload from disk)
            Console.WriteLine("Simulating a new session (memory cleared)...\n");
            var newSessionApp = new InventoryApp(filePath);
            newSessionApp.LoadData();

            Console.WriteLine("--- Items Loaded From File ---");
            newSessionApp.PrintAllItems();
        }
    }
}
