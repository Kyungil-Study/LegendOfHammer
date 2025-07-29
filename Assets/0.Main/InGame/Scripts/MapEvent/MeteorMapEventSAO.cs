using UnityEngine;

[CreateAssetMenu(fileName = "MeteorMapEvent", menuName = "MapEvent/MeteorMapEvent")]
public class MeteorMapEvent : MapEventPatternSAO
{
    [Space(10), Header("Meteor Map Event Settings")]
    [SerializeField] int spawnCount = 1; // Meteor spawn count
    public override void ExecuteEvent()
    {
        MeteorSpawner.Instance.ExecuteMapEvent(damage: MapEventDamage, spawnCount: spawnCount);
    }
}