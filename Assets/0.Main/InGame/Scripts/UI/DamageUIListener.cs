using UnityEngine;

public class DamageUIListener : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 1f;

    private void OnEnable()
    {
        BattleEventManager.Instance.Callbacks.OnSendDamage += HandleTakeDamage;
    }

   private void OnDisable()
   {
       BattleEventManager.Instance.Callbacks.OnSendDamage -= HandleTakeDamage;
   }

   private void HandleTakeDamage(ReceiveDamageEventArgs eventArgs)
   {
       var mono = eventArgs.Self as MonoBehaviour;
       
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
       Debug.Log($"[DamageUIListener] WorldPos: {worldPos}");

       DamageUIManager.Instance.ShowDamage(eventArgs.ActualDamage, worldPos);
   }
}