using UnityEngine;

public class DamageUIListener : MonoBehaviour
{
    [Header("데미지 폰트 색깔")]
    [SerializeField] private Color EnemyColor;
    [SerializeField] private Color ShieldColor;
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color CriticalColor;
    [SerializeField] private Color DoTColor;
    
    [Header("데미지 UI 위치 조정")]
    [SerializeField] private float verticalOffset = 1f;

    private void OnEnable()
    {
        BattleEventManager.RegistEvent<ReceiveDamageEventArgs>(HandleTakeDamage);
    }

   private void OnDisable()
   {
       BattleEventManager.UnregistEvent<ReceiveDamageEventArgs>(HandleTakeDamage);
   }

   private void HandleTakeDamage(ReceiveDamageEventArgs eventArgs)
   {
       var damageType = eventArgs.Type;
       Color damageColor = NormalColor;
       var mono = eventArgs.Self as MonoBehaviour;

       switch (damageType)
       {
           case DamageType.Normal:
               damageColor = NormalColor;
               break;
           case DamageType.Critical:
               damageColor = CriticalColor;
               break;
           case DamageType.Enemy:
               damageColor = EnemyColor;
               break;
           case DamageType.DoT:
               damageColor = DoTColor;
               break;
           case DamageType.Shield:
               damageColor = ShieldColor;
               break;
       }
       
       if (mono == null)
       {
           Debug.LogError("Target이 MonoBehaviour가 아닙니다.");
           return;
       }

       float halfHeight = 0f;
       var rend = mono.GetComponentInChildren<Renderer>();
       
       if (rend != null)
       {
           halfHeight = rend.bounds.extents.y;
       }

       Vector3 worldPos = mono.transform.position + Vector3.up * (halfHeight + verticalOffset);
       
       // **로그 추가**: 월드 좌표
       //Debug.Log($"[DamageUIListener] WorldPos: {worldPos}");

       if (eventArgs.ActualDamage == 0)
       {
           SoundManager.Instance.PlayMiss(); // 혹은 PlayMiss()가 있다면 그걸 호출
           DamageUIManager.Instance.ShowText("miss", worldPos);        // 텍스트 출력
           return;
       }
       DamageUIManager.Instance.ShowDamage(eventArgs.ActualDamage, damageColor, worldPos);
   }
}
