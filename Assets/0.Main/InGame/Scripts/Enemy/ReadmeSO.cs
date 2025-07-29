using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterTestReadme", menuName = "Readme/Monster Test Readme")]
public class ReadmeSO : ScriptableObject
{
    public Sprite   icon;         
    public string   title;        
    [TextArea(10, 20)]
    public string   description;  
    [Tooltip("외부 가이드 링크")]
    public string   guideUrl;     
}