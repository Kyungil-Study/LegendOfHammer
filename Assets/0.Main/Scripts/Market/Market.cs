using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;


[Serializable]
public class MarketData
{
    public int ID { get; set; }
    public string Product { get; set; }
    public string Product_Type { get; set; }
    public string Name { get; set; }
    public int Product_Image_ID { get; set; }
    public bool Check_Product_Availability { get; set; }
    public int Piece { get; set; }
    public int Currency { get; set; }
    public int Number { get; set; }
    public string Sale_Period { get; set; }
}

public class Market : MonoBehaviour
{
    private async void Start()
    {
        List<MarketData> marketList = await TSVLoader.LoadTableAsync<MarketData>("MarketTable");

        if (marketList == null || marketList.Count == 0)
        {
            Debug.LogWarning("[MarketManager] 마켓 데이터를 불러오지 못했습니다.");
            return;
        }

        foreach (var item in marketList)
        {
            Debug.Log($"[{item.ID}] {item.Name} - {item.Piece}원 ({item.Sale_Period})");
        }
    }
}
