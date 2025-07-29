using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    public static DamageUIManager Instance { get; private set; }

    [SerializeField] private GameObject damageUITextPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private float floatHeight   = 30f;

    private Canvas        canvas;
    private Camera        mainCam;
    private Stack<GameObject> pool = new Stack<GameObject>();

    void Awake()
    {
        Instance = this;
        canvas  = GetComponentInParent<Canvas>();
        mainCam = Camera.main;

        // 1. 풀 초기화
        for (int i = 0; i < initialPoolSize; i++)
        {
            var go = Instantiate(damageUITextPrefab, canvas.transform);
            go.SetActive(false);
            pool.Push(go);
        }
    }

    public void ShowDamage(int damage, Color color, Vector3 worldPos)
    {
        // 2. 풀에서 꺼내기
        GameObject go = pool.Count > 0 
            ? pool.Pop() 
            : Instantiate(damageUITextPrefab, canvas.transform);

        // 3. 활성화 및 초기 세팅
        var rt  = go.GetComponent<RectTransform>();
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = damage.ToString();
        tmp.color = color;
        go.SetActive(true);

        // 4. 위치 변환
        Vector2 screenPt = mainCam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPt,
            canvas.worldCamera,
            out Vector2 localPos
        );
        rt.anchoredPosition = localPos;

        // 5. 애니메이션 후 풀로 반납
        StartCoroutine(FloatAndReturn(rt, go));
    }

    private IEnumerator FloatAndReturn(RectTransform rt, GameObject go)
    {
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            rt.anchoredPosition = startPos + Vector2.up * (floatHeight * t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 반납 처리
        go.SetActive(false);
        pool.Push(go);
    }
}
