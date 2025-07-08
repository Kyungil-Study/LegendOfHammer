using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[DefaultExecutionOrder(-1000)]
public class EnemyConfigLoader : MonoBehaviour
{
    // Inspector에 CSV 파일(TextAsset) 드래그
    [Header("CSV Data")]
    [SerializeField] private TextAsset enemyCsv;

    // ID → Config 매핑
    public static Dictionary<int, EnemyConfig> Configs { get; private set; }

    void Awake()
    {
        LoadConfigs();
    }

    private void LoadConfigs()
    {
        Configs = new Dictionary<int, EnemyConfig>();

        using var reader = new StringReader(enemyCsv.text);

        // ▶▶▶ 헤더 3줄 건너뛰기
        reader.ReadLine();
        reader.ReadLine();
        reader.ReadLine();

        string line;
        
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = line.Split(',');
            // 첫 칼럼이 숫자가 아니면 건너뛰기
            if (int.TryParse(cols[0].Trim(), out _) == false) continue;

            var config = EnemyConfig.FromCsvLine(line);
            Configs[config.EnemyID] = config;
        }

        Debug.Log($"[EnemyConfigLoader] Loaded {Configs.Count} EnemyConfigs");
    }
}