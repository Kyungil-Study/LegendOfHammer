using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    public Vector3 worldOffset = new Vector3(0, -0.7f, 0); // 몬스터 위에 띄움
    [SerializeField] private Image fillImage;

    private Transform target;

    public void AttachTo(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void UpdatePosition()
    {
        if (target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + worldOffset);
        transform.position = screenPos;
    }

    // 추후 데미지 이벤트 연동 시 사용 가능
    public void SetRatio(float ratio)
    {
        fillImage.fillAmount = Mathf.Clamp01(ratio);
    }
}