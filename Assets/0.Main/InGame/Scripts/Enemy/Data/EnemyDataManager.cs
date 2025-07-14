using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyDataManager : GenericDictionaryResourceManager<EnemyData, EnemyID, EnemyDataManager>
{
    protected override EnemyID GetKey(EnemyData record)
    {
        return record.Enemy_ID;
    }

}