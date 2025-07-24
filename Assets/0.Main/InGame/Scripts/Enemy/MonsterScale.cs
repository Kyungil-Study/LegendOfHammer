using System;
using System.Collections;
using System.Collections.Generic;
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
    [Header("콜라이더 크기 조정 계수, 기본값")]
    [SerializeField][Range(0.1f, 2f)] private float mHitBoxSize = 1f;
    [Header("피격 이펙트")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float PlayCount = 4f;
    [SerializeField] private float PlayInterval = 1f;
    
    private SpriteRenderer  mSpriteRenderer;
    private BoxCollider2D   mCollider;
    private Animator        mAnimator;
    private Material        mMaterial;
    
    private int mAppearanceIndex;
    private float mScaleFactor;
    
    private Coroutine damageRoutine;
    private Color    originalColor;
    
    Sprite lastSprite;
    float  lastHitBox;

    public float ScaleFactor => mScaleFactor;
    
    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mAnimator       = model.GetComponent<Animator>();
        mCollider       = GetComponent<BoxCollider2D>();
        mMaterial       = model.GetComponent<Renderer>().material;
        
        originalColor   = mSpriteRenderer.color;
    }

    private void Start()
    {
        PickRandomSprite();
        mScaleFactor = CalculateScaleFactor();
        ApplyModelScale(mScaleFactor);
    }

    private void OnEnable()
    {
        BattleEventManager.Instance.Callbacks.OnTakeDamage += PlayDamageEffect;
    }

    private void OnDisable()
    {
        BattleEventManager.Instance.Callbacks.OnTakeDamage -= PlayDamageEffect;
    }
    
    void LateUpdate()
    {
        float currentHitBox = (appearances != null && mAppearanceIndex < appearances.Length)
            ? appearances[mAppearanceIndex].hitBoxSize : mHitBoxSize;

        if (mSpriteRenderer.sprite != lastSprite || Mathf.Approximately(currentHitBox, lastHitBox) == false)
        {
            lastSprite = mSpriteRenderer.sprite;
            lastHitBox = currentHitBox;
            ApplyColliderFromPhysicsShape(mScaleFactor, currentHitBox);
        }
    }
    
    private void PlayDamageEffect(TakeDamageEventArgs eventArgs)
    {
        var targetMonster = eventArgs.Target as Monster;
        
        if (targetMonster == null)
        {
            return;
        }

        if (targetMonster.gameObject != gameObject)
        {
            return;
        }
        
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            mSpriteRenderer.color = originalColor;
        }
        
        damageRoutine = StartCoroutine(PlayDamageEffectCoroutine());
    }
    
    private IEnumerator PlayDamageEffectCoroutine()
    {
        mMaterial.EnableKeyword("HITEFFECT_ON");

        mMaterial.SetColor ("_HitEffectColor", Color.white);
        mMaterial.SetFloat("_HitEffectBlend", 1f);

        yield return new WaitForSeconds(PlayInterval);

        mMaterial.SetFloat("_HitEffectBlend", 0f);

        damageRoutine = null;
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