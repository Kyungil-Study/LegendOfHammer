using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // todo: 스테이지별 스폰 몬스터 달라짐 , 데이터 테이블 연동 필요
    [SerializeField] Monster monsterPrefab;
    [SerializeField] EliteMonster eliteMonsterPrefab;
    [SerializeField] Boss bossMonsterPrefab;
    
    [SerializeField] GameObject TestPlayer; // For testing purposes, remove later
    
    [SerializeField] float spawnInterval = 5f; // Time in seconds between spawns
    [SerializeField] private float eliteSpawnInterval = 40;
    [SerializeField] private float bossSpawnInterval = 100;
    
    [SerializeField] private Transform[] spawnPoints; // Maximum number of monsters allowed on screen
    // Start is called before the first frame update
    
    Coroutine spawnCoroutine;
    Coroutine spawnEliteCoroutine;
    Coroutine spawnBossCoroutine;
    
    private void Awake()
    {
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnStartBattle += StartGame;
        callbacks.OnEndBattle += EndGame;
    }

    private void EndGame(EndBattleEventArgs args)
    {
        StopCoroutine(spawnCoroutine);
        StopCoroutine(spawnEliteCoroutine);
        StopCoroutine(spawnBossCoroutine);
    }

    void StartGame(StartBattleEventArgs args)
    {
        spawnCoroutine = StartCoroutine(SpawnMonster_Coroutine());
        spawnEliteCoroutine = StartCoroutine(SpawnEliteMonster_Coroutine());
        spawnBossCoroutine = StartCoroutine(SpawnBossMonster_Coroutine());
    }
    
    IEnumerator SpawnMonster_Coroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            Monster newMonster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            newMonster.GetComponent<Monster>().SetPlayer(TestPlayer);   // For testing purposes, remove later
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    IEnumerator SpawnEliteMonster_Coroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(eliteSpawnInterval);
            EliteMonster newEliteMonster = Instantiate(eliteMonsterPrefab, transform.position, Quaternion.identity);
        }
    }
    
    IEnumerator SpawnBossMonster_Coroutine()
    {
        yield return new WaitForSeconds(bossSpawnInterval);
        // Example of spawning a boss monster
        Boss newBossMonster = Instantiate(bossMonsterPrefab, transform.position, Quaternion.identity);
    }
}
