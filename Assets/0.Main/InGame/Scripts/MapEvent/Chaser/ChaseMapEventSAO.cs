using UnityEngine;

[CreateAssetMenu(fileName = "ChaseMapEvent", menuName = "MapEvent/ChaseMapEvent")]
public class ChaseMapEvent : MapEventPatternSAO
{
    public override void ExecuteEvent()
    {
        ChaseCrowd.Instance.ExecuteMapEvent( MapEventDamage);
    }
}