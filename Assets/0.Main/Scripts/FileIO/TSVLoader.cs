using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;

using Better.StreamingAssets;
public static class TSVLoader
{
     
    private static readonly CsvConfiguration TsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        Mode = CsvMode.NoEscape,
        HasHeaderRecord = false,
        MissingFieldFound = null,
        HeaderValidated = null,
    };
    
    public static Stream StringToStream(string str)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(byteArray);
    }

    /// <summary>
    /// Application.persistentDataPath/Table 폴더에서 주어진 테이블 이름의 TSV 파일을 읽어 List<T>로 반환합니다.
    /// </summary>
    /// <typeparam name="T"> 매핑할 클래스 타입 (public getter/setter 필수)</typeparam>
    /// <param name="tableName"> 파일 이름 (확장자 제외)</param>
    /// <returns> 파싱된 데이터 리스트</returns>
    public static async Task<List<T>> LoadTableAsync<T>(string tableName, bool isStreamingAssetPath = true)
    {
        string basePath = isStreamingAssetPath ? Application.streamingAssetsPath : Application.persistentDataPath;
        
        
        
        string folderPath = Path.Combine(basePath, "Table");
        string filePath = Path.Combine(folderPath, tableName + ".tsv");
        
        BetterStreamingAssets.Initialize();
        
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(File.Exists(filePath));

        if (isStreamingAssetPath)
        {
            string streamingAssetPath = Path.Combine("Table", tableName + ".tsv");
            filePath = streamingAssetPath;
            if (!BetterStreamingAssets.FileExists(streamingAssetPath))
            {
                Debug.LogError($"[TableLoader] {tableName + ".tsv"} 파일이 없습니다.");
                return null;
            }
        }
        else
        {
            if (File.Exists(filePath) == false)
            {
                Debug.LogError($"[TableLoader] 파일이 존재하지 않습니다: {filePath}");
                return null;
            }
        }

        

        try
        {
            using var reader = new StreamReader(StringToStream(BetterStreamingAssets.ReadAllText(filePath)) );
            
            //using var reader = new StreamReader(filePath);
            // 첫 번째 줄(1행) 스킵 (데이터 사용처에 대한 설명)
            await reader.ReadLineAsync();
            // 두 번째 줄(2행) 스킵 (데이터 타입에 대한 설명)
            await reader.ReadLineAsync();
            using var csv = new CsvReader(reader, TsvConfig);

            var records = new List<T>();
            await foreach (var record in csv.GetRecordsAsync<T>())
            {
                records.Add(record);
            }
            return records;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TableLoader] {tableName}.tsv 로딩 실패: {ex.Message}");
            return null;
        }
    }
}