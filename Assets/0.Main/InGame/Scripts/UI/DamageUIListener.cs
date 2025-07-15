using UnityEngine;

public class DamageUIListener : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 1f;

    private void OnEnable()
    {
        BattleEventManager.Instance.Callbacks.OnTakeDamage += HandleTakeDamage;
    }

   private void OnDisable()
   {
       BattleEventManager.Instance.Callbacks.OnTakeDamage -= HandleTakeDamage;
   }

   private void HandleTakeDamage(TakeDamageEventArgs e)
   {
       var mb = e.Target as MonoBehaviour;
       if (mb == null)
       {
           Debug.LogError("Target이 MonoBehaviour가 아닙니다.");
           return;
       }

       float halfHeight = 0f;
       var rend = mb.GetComponent<Renderer>();
       if (rend != null)
           halfHeight = rend.bounds.extents.y;

       Vector3 worldPos = mb.transform.position 
                          + Vector3.up * (halfHeight + verticalOffset);

       // **로그 추가**: 월드 좌표
       Debug.Log($"[DamageUIListener] WorldPos: {worldPos}");

       DamageUIManager.Instance.ShowDamage(e.Damage, worldPos);
   }
}