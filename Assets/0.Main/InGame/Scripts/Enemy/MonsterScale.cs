using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScale : MonoBehaviour
{
    [Header("몬스터 모델 오브젝트 연결 (자식)")]
    [SerializeField] private Transform model;

    private SpriteRenderer mSpriteRenderer;
    private BoxCollider2D  mCollider;

    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mCollider = GetComponent<BoxCollider2D>();
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

        float origPixels = mSpriteRenderer.sprite.rect.width;
        float scaleFactor = pixelSize / origPixels;
        model.localScale = Vector3.one * scaleFactor;

        // 2) TextureRect에서 투명 제외된 실제 이미지 크기(px)
        Rect texRect   = mSpriteRenderer.sprite.textureRect;
        float ppu      = mSpriteRenderer.sprite.pixelsPerUnit;
        float w_local  = texRect.width  / ppu;  // 로컬(unscaled) 너비
        float h_local  = texRect.height / ppu;  // 로컬(unscaled) 높이

        // 3) BoxCollider2D 사이즈 = 로컬 크기에 스케일 곱하기
        mCollider.size = new Vector2(w_local * scaleFactor, h_local * scaleFactor);

        Vector2 pivotLocal = (mSpriteRenderer.sprite.pivot / ppu) - new Vector2(w_local, h_local) * 0.5f;
        
        // 4) Pivot(0~pixels) → 로컬 좌표 기준 offset 계산
        //    pivot(픽셀) / ppu  → 로컬좌표
        //    -(w_local/2, h_local/2) → bottom-left 기준에서 중앙정렬
        Vector2 modelPos2D = new Vector2(model.localPosition.x, model.localPosition.y);
        mCollider.offset    = modelPos2D + pivotLocal * scaleFactor;
    }
}