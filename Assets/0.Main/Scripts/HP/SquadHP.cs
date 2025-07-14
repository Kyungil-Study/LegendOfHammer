using UnityEngine;
using UnityEngine.UI;

public class SquadHP : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;       // Fill 오브젝트의 RectTransform
    [SerializeField] private RectTransform backgroundRect; // Background 기준 너비
    [SerializeField] private Transform target;             // 체력바 띄울 대상
    [SerializeField] private Vector3 offset = new Vector3(0, -0.7f, 0);
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Squad squad;

    private RectTransform rectTransform;

    private void Start()
    {
        if (squad == null) squad = Squad.Instance;
        if (target == null) target = squad.transform;
        if (mainCamera == null) mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        Debug.Log(squad.stats.CurrentHealth);
    }

    private void Update()
    {
        // 1. 체력바 위치를 캐릭터 위로 이동
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPos;

        // 2. 체력 비율 계산
        float ratio = (float)squad.stats.CurrentHealth / squad.stats.MaxHealth;
        ratio = Mathf.Clamp01(ratio);

        // 3. Fill 오브젝트의 width 조정 (Sliced 타입 유지)
        float maxWidth = backgroundRect.rect.width;
        fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * ratio);
    }
}