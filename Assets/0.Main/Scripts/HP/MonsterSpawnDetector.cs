using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnDetector : MonoBehaviour
{
    [SerializeField] private MonsterHPUIManager hpUIManager;

    private readonly HashSet<MonoBehaviour> tracked = new();

    void Update()
    {
        var monsters = FindObjectsOfType<MonoBehaviour>();

        foreach (var mb in monsters)
        {
            if (mb.GetType().Name == "Monster" && tracked.Contains(mb) == false)
            {
                tracked.Add(mb);
                hpUIManager.RegisterMonster(mb);
            }
        }
    }
}