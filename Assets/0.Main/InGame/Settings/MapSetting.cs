using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSetting", menuName = "Settings/Map Setting")]
public class MapSetting : ScriptableObject
{
    public string MapName; // 맵 이름
    public string MapDescription; // 맵 설명
    public float ScrollSpeed; // 맵 스크롤 속도
    [SerializeField] private float monsterScrollSpeed;
    public float MonsterScrollSpeed => Mathf.Max(0.001f,monsterScrollSpeed);
    [SerializeField] public float scrollAttackSpeed;
    public float ScrollAttackSpeed => Mathf.Max(0.001f,monsterScrollSpeed);
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
