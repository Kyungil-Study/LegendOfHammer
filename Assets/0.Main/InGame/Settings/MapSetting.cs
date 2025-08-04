using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSetting", menuName = "Settings/Map Setting")]
public class MapSetting : ScriptableObject
{
    public string MapName; // 맵 이름
    public string MapDescription; // 맵 설명
    public int MapWidth; // 맵 너비
    public int MapHeight; // 맵 높이
    public float ScrollSpeed; // 맵 스크롤 속도
    public bool IsLooping; // 맵이 반복되는지 여부
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
