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

