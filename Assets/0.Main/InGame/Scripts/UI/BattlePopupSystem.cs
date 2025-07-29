using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePopupSystem : MonoSingleton<BattlePopupSystem>
{
    [SerializeField] UIAlarm bossAlram;
    public UIAlarm BossAlarm => bossAlram;
    
    [SerializeField] UIAlarm chaserAlram;
    public UIAlarm ChaserAlarm => chaserAlram;
    
    [SerializeField] UIAlarm fireballAlram;
    public UIAlarm FireballAlarm => fireballAlram;
    
    [SerializeField] UIAlarm meteorAlram;
    public UIAlarm MeteorAlarm => meteorAlram;
}
