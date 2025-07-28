using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MonsterHPUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform hpBarParent;  // UICanvas 내 HP바 부모
    [SerializeField] private MonsterHPBar hpBarPrefab;   // 프리팹

    private readonly Dictionary<MonoBehaviour, MonsterHPBar> activeBars = new();

    // public void RegisterMonster(MonoBehaviour monster)
    // {
    //     if (activeBars.ContainsKey(monster)) return;
    //     
    //     var bar = Instantiate(hpBarPrefab, hpBarParent);
    //     activeBars[monster] = bar;
    //     bar.AttachTo(monster.transform);
    // }
    
    public void RegisterMonster(MonoBehaviour monster)
    {
        if (activeBars.ContainsKey(monster)) return;

        // 1. EnemyID 가져오기 (리플렉션)
        var enemyIdProp = monster.GetType().GetProperty("EnemyID");
        if (enemyIdProp == null) return;

        if (enemyIdProp.GetValue(monster) is EnemyID enemyID &&
            EnemyDataManager.Instance.EnemyDatas.TryGetValue(enemyID, out var data))
        {
            // 2. 랭크 확인
            if (data.Enemy_Rank == EnemyRank.Elite || data.Enemy_Rank == EnemyRank.Boss)
            {
                // 3. HP바 생성 및 등록
                var bar = Instantiate(hpBarPrefab, hpBarParent);
                bar.AttachTo(monster.transform);

                // 4. 보스는 위치 보정 더 크게
                if (data.Enemy_Rank == EnemyRank.Boss)
                {
                    bar.worldOffset = new Vector3(0, -1.5f, 0); // 보스일 경우 더 위에 표시
                }
                else
                {
                    bar.worldOffset = new Vector3(0, -1f, 0); // 일반 몬스터
                }

                activeBars[monster] = bar;
            }
        }
    }


    public void UnregisterMonster(MonoBehaviour monster)
    {
        if (activeBars.TryGetValue(monster, out var bar))
        {
            Destroy(bar.gameObject);
            activeBars.Remove(monster);
        }
    }

    void LateUpdate()
    {
        var keys = new List<MonoBehaviour>(activeBars.Keys);

        foreach (var monster in keys)
        {
            if (monster == null)
            {
                UnregisterMonster(monster);
                continue;
            }

            var bar = activeBars[monster];
            bar.UpdatePosition();

            float ratio = GetHealthRatio(monster);
            bar.SetRatio(ratio);
        }
    }


    private float GetHealthRatio(MonoBehaviour monster)
    {
        var stat = monster.GetComponent<MonsterStat>();

        int current = stat.CurrentHP;
        int max = stat.MaxHP;
        
        if (max <= 0) return 1f;
        return Mathf.Clamp01((float)current / max);
    }
}