using System;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "FireballPattern", menuName = "MapEvent/FireBallMapEvent")]
public class FireBallMapEvent : MapEventPatternSAO
{
    [Serializable]
    public class FireballSpawnJob
    {
        public float attackDelay ;
        public FireballPointType pointType ;

        public FireballSpawnJob() {}
        public FireballSpawnJob( float attackDelay, FireballPointType pointType)
        {
            this.attackDelay = attackDelay;
            this.pointType = pointType;
        }
    }
    [SerializeField] List<FireballSpawnJob> fireballSpawnJobs = new List<FireballSpawnJob>(); 
    
    public override void ExecuteEvent()
    {
        FireballSpawner.Instance.ExecuteMapEvent(MapEventDamage, fireballSpawnJobs);
    }
}
