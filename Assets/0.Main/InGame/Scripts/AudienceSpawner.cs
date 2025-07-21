using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    
    [Header("Tilemap Settings")]
    [SerializeField] private TilemapRenderer tilemapRenderer; // 타일맵 컴포넌트

    private Grid grid;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapRenderer.enabled = false;
        tilemap = GetComponent<Tilemap>();
        grid = GetComponentInParent<Grid>();
        
        float offset = 1f / spawnCountPerTile; // 타일의 절반 크기만큼 오프셋 설정
        for(int i = 0 ;i < spawnCountPerTile; i++)
        {
            float range = offset * i;
            var spawnPosition = Vector3.Lerp(spawnRangeStart.position, spawnRangeEnd.position, range);
            var audience = Instantiate(audiencePrefab, transform);
                    
            audience.transform.position = spawnPosition;
            audience.transform.rotation = SpawRotation;
                    
            audience.Setup();
        }
    }
}
