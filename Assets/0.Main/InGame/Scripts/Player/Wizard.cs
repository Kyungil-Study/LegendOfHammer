using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wizard : Hero
{
    public Transform projectileSpawnPoint;
    public WizardMagicBall projectilePrefab;

    protected override void Attack()
    {
        WizardMagicBall projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.damage = CalculateDamage();
        projectile.Fire();
    }

    // TODO: Implement Wizard's specific damage calculation logic.
    protected override int CalculateDamage()
    {
        return 0;
    }
}
