using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class AudienceSpawner : MonoBehaviour
{
    [Header("Audience Prefab Settings")]
    [SerializeField] Audience audiencePrefab;
    
    [Header("Spawn Count Settings")]
    [SerializeField] private int spawnCountPerTile = 3; // 한 타일당 생성할 관객 수
    
    [Header("Spawn Transform Settings")]
    [SerializeField] private Quaternion SpawRotation;
    [SerializeField] private Vector3 SpawnOffset;// 생성할 관객의 회전값
    
    [Header("Spawn Range Settings")]
    [SerializeField] Transform spawnRangeStart;
    [SerializeField] Transform spawnRangeEnd;
    

    [SerializeField] private AnimatorOverrideController audienceAOC;
    [SerializeField] private AnimationClip[] audienceAnimations;
    Dictionary<AnimationClip,AnimatorOverrideController> audienceAOCMap = new Dictionary<AnimationClip, AnimatorOverrideController>();
    public Dictionary<AnimationClip, AnimatorOverrideController> AudienceAOCMap => audienceAOCMap;
    public AnimationClip[] AudienceAnimations => audienceAnimations;
    
    void Start()
    {
        foreach (AnimationClip audienceClip in audienceAnimations)
        {
            var animatorOverrideController = new AnimatorOverrideController(audienceAOC);
            animatorOverrideController.name = $"AOC_{audienceClip.name}";
            animatorOverrideController["Audience_Idle"] = audienceClip;
            AudienceAOCMap[audienceClip] = animatorOverrideController;
        }
        
        float offset = 1f / spawnCountPerTile; // 타일의 절반 크기만큼 오프셋 설정
        for(int i = 0 ;i < spawnCountPerTile; i++)
        {
            float range = offset * i;
            var spawnPosition = Vector3.Lerp(spawnRangeStart.position, spawnRangeEnd.position, range);
            var audience = Instantiate(audiencePrefab, transform);
                    
            audience.Setup(this, spawnPosition, SpawRotation);
        }
    }
}
