using UnityEngine;

namespace GameResources.Features.SaveLoadSystem
{
    public sealed class SaveLoadService: ISaveLoadService
    {
        private const string SAVE_CATALOG = "Saves/";

        private string Path { get; set; }

        public void SaveData<T>(string data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            Save(data);
#if UNITY_EDITOR
            Debug.Log($"Data: {data}");
#endif
        }
        public void SaveData<T>(T data)
        {
            Path = SAVE_CATALOG + typeof(T).Name;
            Save(JsonUtility.ToJson(data));
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(data);
            Debug.Log($"Data: {json}");
#endif
        }

        public bool TryLoadData<T>(out T data)
        {
            data = default;
            Path = SAVE_CATALOG + typeof(T).Name;
#if UNITY_EDITOR
            Debug.Log($"Data: {Load()}");
#endif
            data = JsonUtility.FromJson<T>(Load());
            if (data == null)
            {
                Debug.LogError($"Data not loaded");
            }
            return data != null;
        }
        
        private void Save(string value)
        {
            PlayerPrefs.SetString(Path, value);
            PlayerPrefs.Save();
        }
        private string Load() => PlayerPrefs.GetString(Path, string.Empty);
    }

    public interface ISaveLoadService
    {
        public bool TryLoadData<T>(out T data);
        public void SaveData<T>(string data);
        public void SaveData<T>(T data);
    }
}