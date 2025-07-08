using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private bool onlyMid = false;
    [SerializeField] private EnemySpawnPatternType waveType;
    [SerializeField] WaveSlot[] waveSlots;
}
