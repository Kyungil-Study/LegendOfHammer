using UnityEngine;
using UnityEngine.UI;

public class SquadHP : MonoBehaviour
{
    [SerializeField] private Image fillImage;           // Fill에 연결
    [SerializeField] private Transform target;          // 체력바를 따라다닐 대상
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // 머리 위 위치
    [SerializeField] private Camera mainCamera;         // UI가 카메라 모드일 때 필요

    [SerializeField] private Squad squad;               // 체력 정보 가져오기

    private RectTransform rectTransform;

    private void Start()
    {
        if (squad == null)
            squad = Squad.Instance;

        if (target == null)
            target = squad.transform;

        if (mainCamera == null)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // 1. UI를 대상 위에 위치시키기 (2D에서 정상 작동)
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPos;

        // 2. fillAmount 조절 (Image.type = Filled 일 때 작동)
        float ratio = (float)squad.stats.CurrentHealth / squad.stats.MaxHealth;
        fillImage.fillAmount = ratio;
    }
}