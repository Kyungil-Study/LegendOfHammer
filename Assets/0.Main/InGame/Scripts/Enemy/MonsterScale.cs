using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterScale : MonoBehaviour
{
    [Header("몬스터 모델 오브젝트 (자식)")]
    [SerializeField] private Transform model;
    [Header("적 스프라이트")]
    [SerializeField] private Sprite[] mSprites;
    [Header("애니메이션 클립")]
    [SerializeField] private AnimationClip[] mAnimationClips;
    [Header("콜라이더 크기 조정 계수")]
    [SerializeField][Range(0.1f, 2f)] private float mHitBoxSize = 1f;

    private SpriteRenderer  mSpriteRenderer;
    private BoxCollider2D   mCollider;
    private Animator        mAnimator;
    private float           mScaleFactor;

    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mCollider       = GetComponent<BoxCollider2D>();
        mAnimator       = GetComponent<Animator>();
    }

    private void Start()
    {
        PickRandomSprite();
        mScaleFactor = CalculateScaleFactor();
        
        ApplyModelScale(mScaleFactor);
        ApplyColliderFromPhysicsShape(mScaleFactor);
    }
    
    private void PickRandomSprite()
    {
        if (mSprites.Length == 0) return;
        int index = UnityEngine.Random.Range(0, mSprites.Length);
        mSpriteRenderer.sprite = mSprites[index];
        // 클립도 이 index에 맞게 바꿔주기
    }

    private float CalculateScaleFactor()
    {
        var monster = GetComponent<Monster>();
        var data    = EnemyDataManager.Instance.Records[monster.EnemyID];
        int enemyPixel = data.Enemy_Rank switch
        {
            EnemyRank.Elite  => 160,
            EnemyRank.Boss   => 240,
            EnemyRank.Normal =>  72
        };
        
        float originPixel = mSpriteRenderer.sprite.rect.width;
        return enemyPixel / originPixel;
    }

    private void ApplyModelScale(float scaleFactor)
    {
        model.localScale = Vector3.one * scaleFactor;
    }
    
    private void ApplyColliderFromPhysicsShape(float scaleFactor)
    {
        var sprite    = mSpriteRenderer.sprite;
        int count  = sprite.GetPhysicsShapeCount();
        var allPoints = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            var path = new List<Vector2>();
            sprite.GetPhysicsShape(i, path);
            allPoints.AddRange(path);
        }

        if (allPoints.Count == 0) return;

        float minX = allPoints[0].x, maxX = allPoints[0].x;
        float minY = allPoints[0].y, maxY = allPoints[0].y;
        
        foreach (var points in allPoints)
        {
            if (points.x < minX) minX = points.x;
            if (points.x > maxX) maxX = points.x;
            if (points.y < minY) minY = points.y;
            if (points.y > maxY) maxY = points.y;
        }

        Vector2 size   = new Vector2(maxX - minX, maxY - minY) * scaleFactor * mHitBoxSize;
        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f) * scaleFactor
                         + (Vector2)model.localPosition;

        mCollider.size   = size;
        mCollider.offset = center;
    }
}