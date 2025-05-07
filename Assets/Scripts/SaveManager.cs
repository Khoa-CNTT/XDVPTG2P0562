using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public static class SaveManager
{
    public static string CreateNewSaveFolder(string playerName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string saveFolder = Path.Combine(Application.persistentDataPath, $"Save_{timestamp}_{playerName}");
        Directory.CreateDirectory(saveFolder);

        string dbPath = Path.Combine(saveFolder, "save_data.db");
        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS save_info (
                        SaveID INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerName TEXT,
                        CreatedAt TEXT
                    );
                    CREATE TABLE IF NOT EXISTS player_data (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        PositionX REAL,
                        PositionY REAL,
                        CurrentHP INTEGER,
                        PotionCount INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS inventory_items (
                        ItemID TEXT PRIMARY KEY,
                        Quantity INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS money_data (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Amount INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS equipped_items (
                        Slot TEXT PRIMARY KEY,
                        ItemID TEXT
                    );
                    CREATE TABLE IF NOT EXISTS potion_data (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        PotionCount INTEGER
                    );
                    DELETE FROM save_info;
                    INSERT INTO save_info (PlayerName, CreatedAt)
                    VALUES (@name, @created);";

                cmd.Parameters.AddWithValue("@name", playerName);
                cmd.Parameters.AddWithValue("@created", DateTime.Now.ToString("o")); // ISO 8601
                cmd.ExecuteNonQuery();
            }
        }

        return saveFolder;
    }

    public static void SaveGame(string saveFolder, PlayerData playerData, List<InventoryItemData> inventory, int money)
    {
        string dbPath = Path.Combine(saveFolder, "save_data.db");
        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // Player data
                cmd.CommandText = "DELETE FROM player_data;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO player_data (PositionX, PositionY, CurrentHP, PotionCount) VALUES (@x, @y, @hp, @potions);";
                cmd.Parameters.AddWithValue("@x", playerData.PositionX);
                cmd.Parameters.AddWithValue("@y", playerData.PositionY);
                cmd.Parameters.AddWithValue("@hp", playerData.CurrentHP);
                cmd.Parameters.AddWithValue("@potions", playerData.PotionCount);
                cmd.ExecuteNonQuery();

                // Inventory
                cmd.CommandText = "DELETE FROM inventory_items;";
                cmd.ExecuteNonQuery();
                foreach (var item in inventory)
                {
                    cmd.CommandText = "INSERT INTO inventory_items (ItemID, Quantity) VALUES (@itemID, @quantity);";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@itemID", item.ItemID);
                    cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                    cmd.ExecuteNonQuery();
                }

                // Money
                cmd.CommandText = "DELETE FROM money_data;";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO money_data (Amount) VALUES (@amount);";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@amount", money);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void LoadGame(string saveFolder, out PlayerData playerData, out List<InventoryItemData> inventory, out int money)
    {
        playerData = new PlayerData();
        inventory = new List<InventoryItemData>();
        money = 0;

        string dbPath = Path.Combine(saveFolder, "save_data.db");
        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // Player data
                cmd.CommandText = "SELECT PositionX, PositionY, CurrentHP, PotionCount FROM player_data LIMIT 1;";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        playerData.PositionX = reader.GetFloat(0);
                        playerData.PositionY = reader.GetFloat(1);
                        playerData.CurrentHP = reader.GetInt32(2);
                        playerData.PotionCount = reader.GetInt32(3);
                    }
                }

                // Inventory
                cmd.CommandText = "SELECT ItemID, Quantity FROM inventory_items;";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventory.Add(new InventoryItemData
                        {
                            ItemID = reader.GetString(0),
                            Quantity = reader.GetInt32(1)
                        });
                    }
                }

                // Money
                cmd.CommandText = "SELECT Amount FROM money_data LIMIT 1;";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        money = reader.GetInt32(0);
                    }
                }
            }
        }
    }

    public static void SaveEquippedItems(string saveFolder, Dictionary<string, string> equipped)
    {
        string dbPath = Path.Combine(saveFolder, "save_data.db");
        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM equipped_items;";
                cmd.ExecuteNonQuery();

                foreach (var kvp in equipped)
                {
                    cmd.CommandText = "INSERT INTO equipped_items (Slot, ItemID) VALUES (@slot, @itemID);";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@slot", kvp.Key);
                    cmd.Parameters.AddWithValue("@itemID", kvp.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public static Dictionary<string, string> LoadEquippedItems(string saveFolder)
    {
        var result = new Dictionary<string, string>();

        string dbPath = Path.Combine(saveFolder, "save_data.db");
        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Slot, ItemID FROM equipped_items;";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string slot = reader.GetString(0);
                        string itemID = reader.GetString(1);
                        result[slot] = itemID;
                    }
                }
            }
        }

        return result;
    }

    public static List<string> GetAllSaveFolders()
    {
        string[] dirs = Directory.GetDirectories(Application.persistentDataPath, "Save_*");
        return new List<string>(dirs);
    }

    public static void SaveInventory(string path, List<InventoryItemData> items)
    {
        string connStr = $"URI=file:{path}/save_data.db";

        using (var conn = new SqliteConnection(connStr))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM inventory_items;";
                cmd.ExecuteNonQuery();

                foreach (var i in items)
                {
                    cmd.CommandText = "INSERT INTO inventory_items (ItemID, Quantity) VALUES (@id, @qty);";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", i.ItemID);
                    cmd.Parameters.AddWithValue("@qty", i.Quantity);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public static (string playerName, DateTime createdAt)? GetSaveInfo(string saveFolder)
    {
        string dbPath = Path.Combine(saveFolder, "save_data.db");
        if (!File.Exists(dbPath)) return null;

        string connectionString = $"URI=file:{dbPath}";

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT PlayerName, CreatedAt FROM save_info LIMIT 1;";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string dateStr = reader.GetString(1);
                        DateTime dt = DateTime.Parse(dateStr);
                        return (name, dt);
                    }
                }
            }
        }

        return null;
    }
}
