using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScale : MonoBehaviour
{
    [Header("몬스터 모델 오브젝트 연결 (자식)")]
    [SerializeField] private Transform model;

    private SpriteRenderer    spriteRenderer;
    private BoxCollider2D  Collider;

    private void Awake()
    {
        spriteRenderer = model.GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        var monster = GetComponent<Monster>();
        var data    = EnemyDataManager.Instance.Records[monster.EnemyID];
        int pixelSize;
        
        switch (data.Enemy_Rank)
        {
            case EnemyRank.Elite: pixelSize = 160; break;
            case EnemyRank.Boss:  pixelSize = 240; break;
            default:              pixelSize =  72; break;
        }

        float origPixels = spriteRenderer.sprite.rect.width;
        float scaleFactor = pixelSize / origPixels;
        model.localScale = Vector3.one * scaleFactor;

        Vector2 size = spriteRenderer.bounds.size;
        
        // spriteRenderer.bounds.size.x 는 '모델.localScale' 이 적용된 월드 너비
        Collider.size   = size;
        Collider.offset = spriteRenderer.bounds.center - (Vector3)transform.position;
    }
}