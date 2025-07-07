using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour
{
    [SerializeField] PageScroller pageScroller;
    [SerializeField] Material[] stageMaterials;
    [SerializeField] TilemapRenderer[] tilemapRenderers;

    private void Awake()
    {
        tilemapRenderers = GetComponentsInChildren<TilemapRenderer>();
        BattleEventManager.Instance.Callbacks.OnStartBattle += StartGame;
        BattleEventManager.Instance.Callbacks.OnEndBattle += EndGame;
    }

    private void EndGame(EndBattleEventArgs args)
    {
        pageScroller.enabled = false;
    }

    private void StartGame(StartBattleEventArgs startEventArgs)
    {
        pageScroller.enabled = true;
        int mapIndex = (startEventArgs.StageIndex % stageMaterials.Length);
        foreach (TilemapRenderer tilemapRenderer in tilemapRenderers)
        {
            tilemapRenderer.material = stageMaterials[mapIndex];
        }
    }
}
