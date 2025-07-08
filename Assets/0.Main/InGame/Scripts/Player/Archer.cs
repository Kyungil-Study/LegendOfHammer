using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Archer : Hero
{
    public Transform projectileSpawnPoint;
    public ArcherArrow projectilePrefab;
    
    protected override void Attack()
    {
        ArcherArrow projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.damage = CalculateDamage();
        projectile.Fire();
    }

    // TODO: Implement Archer's specific damage calculation logic.
    protected override int CalculateDamage()
    {
        return 0;
    }
}
