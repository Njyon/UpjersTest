using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TowerShoot : AAttackLogic
{
    public WeaponProjectile projectilePrefab;
    public float projectileSpeed;
    public float projectileDamage;
    public Transform muzzleTip;

    public override void Attack()
    {
        WeaponProjectile projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = muzzleTip.position;
        projectile.Init(owner, owner.attackingTarget, projectileSpeed, projectileDamage);
    }
}
