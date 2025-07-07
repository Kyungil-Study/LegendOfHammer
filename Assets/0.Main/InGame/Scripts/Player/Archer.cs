using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Hero
{
    public Transform arrowSpawnPoint;
    public ArcherArrow arrowPrefab;
    private void Awake()
    {
        attackDamage = 250;
        attackPerSec = 2;
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            Attack();
            ApplyCooldown();
        }
    }

    public override void Attack()
    {
        var arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        arrow.Fire();
    }
}
