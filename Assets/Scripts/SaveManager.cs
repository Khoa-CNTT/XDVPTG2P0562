using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class SaveManager : IDisposable
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance ??= new SaveManager();

    private string _connectionString;

    private SaveManager()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        _connectionString = $"URI=file:{Application.persistentDataPath}/save_data.db";
        
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS saves (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerName TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        PlayTime REAL DEFAULT 0
                    );";
                cmd.ExecuteNonQuery();
            }
            
            conn.Close();
        }
    }

    public int CreateSave(string playerName)
{
    try
    {
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            
            // Bắt đầu transaction
            using (var transaction = conn.BeginTransaction())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO saves (PlayerName, CreatedAt, PlayTime)
                        VALUES (@name, @date, 0);
                        SELECT last_insert_rowid();";
                    
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("o"));
                    
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    transaction.Commit(); // Quan trọng: Xác nhận ghi dữ liệu
                    
                    Debug.Log($"Đã tạo save ID: {newId}");
                    return newId;
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Lỗi tạo save: {ex}");
        return -1;
    }
}

public List<SaveData> GetAllSaves()
{
    var saves = new List<SaveData>();
    
    using (var conn = new SqliteConnection(_connectionString))
    {
        conn.Open();
        
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT Id, PlayerName, CreatedAt, PlayTime 
                FROM saves 
                ORDER BY Id DESC"; // Sắp xếp theo ID mới nhất
            
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        saves.Add(new SaveData
                        {
                            Id = reader.GetInt32(0),
                            PlayerName = reader[1].ToString(),
                            CreatedAt = reader[2].ToString(),
                            PlayTime = Convert.ToSingle(reader[3])
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Lỗi đọc save: {ex}");
                    }
                }
            }
        }
    }
    
    Debug.Log($"Tìm thấy {saves.Count} saves");
    return saves;
}
        public void Dispose()
    {
        _instance = null;
    }
}

[System.Serializable]
public class SaveData
{
    public int Id;
    public string PlayerName;
    public string CreatedAt;
    public float PlayTime;
}