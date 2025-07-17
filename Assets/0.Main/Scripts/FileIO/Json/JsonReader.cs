using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonReader
{
    public static T Read<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("[JsonReader] JSON 문자열이 비어있음.");
            return default;
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(json);
            Debug.Log("[JsonReader] JSON 역직렬화 완료.");
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[JsonReader] JSON 역직렬화 실패: {ex}");
            return default;
        }
        
    }
    
    /// <summary>
    /// 지정된 경로의 Json 파일을 읽어 T 타입으로 역직렬화합니다.
    /// </summary>
    public static T Load<T>(string filePath)
    {
        string fullPath = filePath.EndsWith(".json") ? filePath : $"{filePath}.json";

        if (File.Exists(fullPath) == false)
        {
            Debug.LogWarning($"[JsonLoader] 파일이 존재하지 않음: {fullPath}");
            return default;
        }

        try
        {
            string json = File.ReadAllText(fullPath);
            T data = JsonConvert.DeserializeObject<T>(json);
            Debug.Log($"[JsonLoader] 로드 완료: {fullPath}");
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[JsonLoader] 로드 실패: {fullPath}\n{ex}");
            return default;
        }
    }
}