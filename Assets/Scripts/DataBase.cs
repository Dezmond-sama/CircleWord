using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    [System.Serializable]
    public class Row
    {
        public int key;
        public string[] value;
    }

    [System.Serializable]
    public class ReadedData
    {
        public Row[] DB;
    }

    private static DataBase _instance;
    private Dictionary<int, List<string>> _db = new Dictionary<int, List<string>>();

    private DataBase()
    {
        LoadDataBase();
    }

    private void LoadDataBase()
    {
        TextAsset file = (TextAsset)Resources.Load("DB");
        string jsonString = file.ToString();
        if (jsonString != null)
        {
            _db.Clear();
            ReadedData data = JsonUtility.FromJson<ReadedData>(jsonString);
            foreach (Row row in data.DB)
            {
                _db.Add(row.key, new List<string>(row.value));
            }
        }
    }
    public static DataBase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataBase();
            }
            return _instance;
        }
    }

    public static string GetWord(int len)
    {
        if (Instance._db.ContainsKey(len) && Instance._db[len].Count > 0)
        {
            int index = Random.Range(0, Instance._db[len].Count);
            return Instance._db[len][index];
        }
        else
        {
            return "";
        }
    }

    public static bool CheckWord(string word)
    {
        int len = word.Length;
        if (Instance._db.ContainsKey(len) && Instance._db[len].Count > 0)
        {
            return Instance._db[len].Contains(word.ToLower());
        }
        else
        {
            return false;
        }
    }
}