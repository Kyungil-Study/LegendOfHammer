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
    [Header("콜라이더 오프셋 보정")]
    [SerializeField][Tooltip("콜라이더 중앙에서 얼마나 X축 방향으로 이동할지")]
    private float mHitBoxOffsetX = 0f;
    [SerializeField][Tooltip("콜라이더 중앙에서 얼마나 Y축 방향으로 이동할지")]
    private float mHitBoxOffsetY = 0f;
    
    private SpriteRenderer  mSpriteRenderer;
    private BoxCollider2D   mCollider;
    private Animator        mAnimator;
    private Rect            mTrimmedRect;
    private float           mScaleFactor;
    private float           mPixelPerUnit;

    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mCollider       = GetComponent<BoxCollider2D>();
        mAnimator       = GetComponent<Animator>();
    }

    private void Start()
    {
        PickRandomSprite();
        SetSpriteData();
        
        mScaleFactor = CalculateScaleFactor();
        
        ApplyModelScale(mScaleFactor);
        ApplyColliderSize(mScaleFactor);
        ApplyColliderOffset(mScaleFactor);
    }
    
    private void PickRandomSprite()
    {
        if (mSprites.Length == 0) return;
        int index = UnityEngine.Random.Range(0, mSprites.Length);
        mSpriteRenderer.sprite = mSprites[index];
        // 클립도 이 index에 맞게 바꿔주기
    }
    
    private void SetSpriteData()
    {
        mTrimmedRect = mSpriteRenderer.sprite.textureRect;
        mPixelPerUnit = mSpriteRenderer.sprite.pixelsPerUnit;
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

    private void ApplyColliderSize(float scaleFactor)
    {
        Vector2 baseSizeUnits = new Vector2(mTrimmedRect.width, mTrimmedRect.height) / mPixelPerUnit;
        Vector2 finalSize = baseSizeUnits * scaleFactor * mHitBoxSize;
        mCollider.size = finalSize;
    }

    private void ApplyColliderOffset(float scaleFactor)
    {
        Vector2 rectCenterPx = new Vector2(mTrimmedRect.width, mTrimmedRect.height) * 0.5f;
        Vector2 pivotPx      = mSpriteRenderer.sprite.pivot;
        Vector2 offsetPx     = rectCenterPx - pivotPx;

        Vector2 baseOffsetUnits = offsetPx / mPixelPerUnit * scaleFactor;
        Vector2 modelPos2D = (Vector2)model.localPosition;
        Vector2 manualOffset = new Vector2(mHitBoxOffsetX, mHitBoxOffsetY);

        mCollider.offset = baseOffsetUnits + modelPos2D + manualOffset;
    }
}
