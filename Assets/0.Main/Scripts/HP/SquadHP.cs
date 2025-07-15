using UnityEngine;
using UnityEngine.UI;

public class SquadHP : MonoBehaviour
{
    [SerializeField] private Image fillImage;       // Fill 오브젝트의 RectTransformz
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
        // 1. UI를 대상 위에 위치시키기 (2D에서 정상 작동)
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPos;

        // 2. fillAmount 조절 (Image.type = Filled 일 때 작동)
        float ratio = (float)squad.stats.CurrentHealth / squad.stats.MaxHealth;
        fillImage.fillAmount = ratio;
    }
}