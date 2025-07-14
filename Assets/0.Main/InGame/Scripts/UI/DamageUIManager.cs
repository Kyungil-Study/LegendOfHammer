using System.Collections;
using TMPro;
using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    public static DamageUIManager Instance { get; private set; }

    [SerializeField] private GameObject damageUITextPrefab; // TMP Text prefab
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private float floatHeight = 0f;

    private Canvas canvas;
    private Camera mainCam;

    void Awake()
    {
        Instance = this;
        canvas = GetComponentInParent<Canvas>();       
        mainCam = Camera.main;
    }

    public void ShowDamage(int damage, Vector3 worldPos)
    {
        // 1) 월드 → 스크린
        Vector2 screenPt = mainCam.WorldToScreenPoint(worldPos);
        Debug.Log($"[DamageUIManager] ScreenPt: {screenPt}");

        // 2) 스크린 → 로컬(Canvas)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPt,
            canvas.worldCamera,
            out Vector2 localPos
        );
        Debug.Log($"[DamageUIManager] LocalPos: {localPos}");

        // 이후 기존 로직…
        var go = Instantiate(damageUITextPrefab, canvas.transform);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogError("[DamageUIManager] 프리팹에 TextMeshProUGUI가 없습니다.");
            Destroy(go);
            return;
        }
        tmp.text = damage.ToString();
        var rt = go.GetComponent<RectTransform>();
        StartCoroutine(FloatAndDestroy(rt, localPos));
    }


    private IEnumerator FloatAndDestroy(RectTransform rt, Vector2 startPos)
    {
        float elapsed = 0f;
        rt.anchoredPosition = startPos;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            rt.anchoredPosition = startPos + Vector2.up * (floatHeight * t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(rt.gameObject);
    }
}