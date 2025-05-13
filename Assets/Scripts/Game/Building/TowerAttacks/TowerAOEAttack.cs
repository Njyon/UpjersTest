using System;
using UnityEngine;

[Serializable]
public class TowerAOEAttack : AAttackLogic
{
    public float aoeDamage;

    public override void Attack()
    {
        // make copy because enemys get removed on death
        AEnemy[] enemies = owner.enemiesInRange.ToArray();
        foreach (AEnemy enemy in enemies)
        {
            if (enemy != null)
                enemy.DoDamage(aoeDamage);
        }
    }
}
