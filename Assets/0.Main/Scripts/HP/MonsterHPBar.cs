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

    // public void UpdatePosition()
    // {
    //     if (target == null) return;
    //
    //     Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + worldOffset);
    //     transform.position = screenPos;
    // }
    
    public void UpdatePosition()
    {
        if (target == null || Camera.main == null) return;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Vector3 worldPos = target.position + worldOffset;

        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // 월드 스페이스 캔버스일 경우, 직접 월드 위치 적용
            transform.position = worldPos;
        }
        else
        {
            // 스크린 공간일 경우, 스크린 좌표로 변환 후 적용
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = screenPos;
        }
    }


    // 추후 데미지 이벤트 연동 시 사용 가능
    public void SetRatio(float ratio)
    {
        fillImage.fillAmount = Mathf.Clamp01(ratio);
    }
}