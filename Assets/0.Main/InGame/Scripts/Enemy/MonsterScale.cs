using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterScale : MonoBehaviour
{
    [Serializable]
    private struct Appearance
    {
        [Header("몬스터 외형 스프라이트")]
        public Sprite sprite;
        [Header("애니메이션 Idle 상태 클립")]
        public AnimationClip idleClip;
        [Header("콜라이더 크기 조정 계수, 스프라이트에 맞게")]
        [Range(0.1f, 2f)] public float hitBoxSize;
    }
    
    [Header("몬스터 모델 오브젝트 (자식)")]
    [SerializeField] private Transform model;
    [Header("몬스터 외형 세팅")] 
    [SerializeField] private Appearance[] appearances;
    [Header("콜라이더 크기 조정 계수 : 기본값")]
    [SerializeField][Range(0.1f, 2f)] private float mHitBoxSize = 1f;
    [Header("이펙트 세팅 (피격 & 자폭)")]
    [SerializeField] private float hitFlashInterval = 0.2f;
    [SerializeField] private Color hitFlashColor = Color.white;
    [SerializeField] private Color suicideFlashColor = Color.red;
    
    [SerializeField,LabelText("펀치 효과")] PunchEffctor punchEffect;
    
    private SpriteRenderer  mSpriteRenderer;
    private BoxCollider2D   mCollider;
    private Animator        mAnimator;
    private Material        mMaterial;
    
    private int mAppearanceIndex;
    private float mScaleFactor;
    
    // 피격효과 필드
    private bool mIsSuicideMode;
    private Coroutine mFlashRoutine;
    private Sprite mSprite;
    private float mHitBox;
    
    public float ScaleFactor => mScaleFactor;
    
    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mAnimator       = model.GetComponent<Animator>();
        mMaterial       = model.GetComponent<Renderer>().material;
        mCollider       = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        PickRandomSprite();
        mScaleFactor = CalculateScaleFactor();
        ApplyModelScale(mScaleFactor);
    }

    private void OnEnable()
    {
        BattleEventManager.RegistEvent<TakeDamageEventArgs>(PlayDamageEffect);
    }

    private void OnDisable()
    {
        BattleEventManager.UnregistEvent<TakeDamageEventArgs>(PlayDamageEffect);
    }
    
    void LateUpdate()
    {
        float currentHitBox = (appearances != null && mAppearanceIndex < appearances.Length)
            ? appearances[mAppearanceIndex].hitBoxSize : mHitBoxSize;

        if (mSpriteRenderer.sprite != mSprite || Mathf.Approximately(currentHitBox, mHitBox) == false)
        {
            mSprite = mSpriteRenderer.sprite;
            mHitBox = currentHitBox;
            ApplyColliderFromPhysicsShape(mScaleFactor, currentHitBox);
        }
    }

    public void EnterSuicideMode()
    {
        mIsSuicideMode = true;
    }
    
    private void PlayDamageEffect(TakeDamageEventArgs eventArgs)
    {
        if ((eventArgs.Target as Monster)?.gameObject != gameObject) return;
        if (mIsSuicideMode) return;               
        PlayHitFlash();       
        punchEffect.PlayEffect();
    }
    
    private void PlayHitFlash()
    {
        if (mFlashRoutine != null) StopCoroutine(mFlashRoutine);
        mFlashRoutine = StartCoroutine(FlashCoroutine(hitFlashColor, 1, hitFlashInterval));
    }
    
    public void StartSuicideFlash(float delay, float interval)
    {
        if (mFlashRoutine != null) StopCoroutine(mFlashRoutine);
        mIsSuicideMode = true;
        mFlashRoutine = StartCoroutine(SuicideFlashCoroutine(delay, interval));
    }
    
    private IEnumerator FlashCoroutine(Color color, int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            mMaterial.EnableKeyword("HITEFFECT_ON");
            mMaterial.SetColor ("_HitEffectColor", color);
            mMaterial.SetFloat("_HitEffectBlend", 1f);

            yield return new WaitForSeconds(interval);

            mMaterial.SetFloat("_HitEffectBlend", 0f);
            yield return new WaitForSeconds(interval * 0.5f);
        }
        mFlashRoutine = null;
    }

    private IEnumerator SuicideFlashCoroutine(float delay, float interval)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            mMaterial.EnableKeyword("HITEFFECT_ON");
            mMaterial.SetColor ("_HitEffectColor", suicideFlashColor);
            mMaterial.SetFloat("_HitEffectBlend", 1f);
            yield return new WaitForSeconds(interval);

            mMaterial.SetFloat("_HitEffectBlend", 0f);
            yield return new WaitForSeconds(interval);

            elapsed += interval * 2f;
        }
        
        mIsSuicideMode = false;
        mFlashRoutine  = null;
    }
    private void PickRandomSprite()
    {
        if (appearances == null || appearances.Length == 0)
        {
            Debug.LogError("적 외형을 설정해주세요");
            return;
        }

        mAppearanceIndex = UnityEngine.Random.Range(0, appearances.Length);
        var appearance = appearances[mAppearanceIndex];

        mSpriteRenderer.sprite = appearance.sprite;

        if (appearance.idleClip != null)
        {
            var overrideCtrl = new AnimatorOverrideController(mAnimator.runtimeAnimatorController);
            overrideCtrl["Idle"] = appearance.idleClip;  
            mAnimator.runtimeAnimatorController = overrideCtrl;
        }
        
        mScaleFactor = CalculateScaleFactor();
        ApplyModelScale(mScaleFactor);
        ApplyColliderFromPhysicsShape(mScaleFactor, appearance.hitBoxSize);
    }

    private float CalculateScaleFactor()
    {
        var monster = GetComponent<Monster>();
        var data    = EnemyDataManager.Instance.EnemyDatas[monster.EnemyID];
        
        int enemyPixel = data.Enemy_Rank switch
        {
            EnemyRank.Elite  => 160,
            EnemyRank.Boss   => 240,
            EnemyRank.Normal =>  72,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        float originPixel = mSpriteRenderer.sprite.rect.width;
        return enemyPixel / originPixel;
    }

    private void ApplyModelScale(float scaleFactor)
    {
        model.localScale = Vector3.one * scaleFactor;
    }
    
    private void ApplyColliderFromPhysicsShape(float scaleFactor, float hitBoxSize)
    {
        var sprite = mSpriteRenderer.sprite;
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

        foreach (var p in allPoints)
        {
            if (p.x < minX) minX = p.x; if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y; if (p.y > maxY) maxY = p.y;
        }

        Vector2 size   = new Vector2(maxX - minX, maxY - minY) * (scaleFactor * hitBoxSize);
        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f) * scaleFactor
                         + (Vector2)model.localPosition;

        mCollider.size   = size;
        mCollider.offset = center;
    }
}