using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MonsterHPUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform hpBarParent;  // UICanvas 내 HP바 부모
    [SerializeField] private MonsterHPBar hpBarPrefab;   // 프리팹

    private readonly Dictionary<MonoBehaviour, MonsterHPBar> activeBars = new();

    public void RegisterMonster(MonoBehaviour monster)
    {
        if (activeBars.ContainsKey(monster)) return;

        var bar = Instantiate(hpBarPrefab, hpBarParent);
        activeBars[monster] = bar;
        bar.AttachTo(monster.transform);
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
        var type = monster.GetType();

        FieldInfo currentHpField = type.GetField("mCurrentHP", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo maxHpField     = type.GetField("mMaxHP", BindingFlags.NonPublic | BindingFlags.Instance);

        if (currentHpField == null || maxHpField == null) return 1f;

        int current = (int)currentHpField.GetValue(monster);
        int max     = (int)maxHpField.GetValue(monster);

        if (max <= 0) return 1f;
        return Mathf.Clamp01((float)current / max);
    }

}