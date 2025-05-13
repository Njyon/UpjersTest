using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    public delegate void OnProjectileHit(WeaponProjectile projectile, Collider other);
    public delegate void OnProjectileLifeTimeEnd(WeaponProjectile projectile);

    bool isInit = false;
    protected AEnemy target;
    protected float speed;
    protected float damage;
    protected Tower owner;
    protected Rigidbody rigidBody;
    Ultra.Timer lifeTimeTimer;
    bool isBeingDestoryed = false;

    public void Init(Tower owner, AEnemy target, float speed, float damage, float lifeTime = 5f)
    {
        this.owner = owner;
        this.target = target;
        this.speed = speed;
        if (lifeTimeTimer == null) lifeTimeTimer = new Ultra.Timer();
        lifeTimeTimer.onTimerFinished += OnTimerFinished;
        lifeTimeTimer.Start(lifeTime);
        this.damage = damage;

        isInit = true;
    }


    void Update()
    {
        if (!isInit) return;
        if (lifeTimeTimer != null) lifeTimeTimer.Update(Time.deltaTime);

        Move();
    }

    protected virtual void Move()
    {
        if (!isInit) return;
        // Target can reach end.  Just kill projectile for easy fix
        if (target == null && !isBeingDestoryed)
        {
            isBeingDestoryed = true;
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (transform.position.IsNearlyEqual(target.transform.position, 0.1f))
        {
            target.DoDamage(damage);
            Destroy(gameObject);
        }
    }

    void RemoveSubscriptions()
    {
        if (lifeTimeTimer != null) lifeTimeTimer.onTimerFinished -= OnTimerFinished;
    }

    void OnDestroy()
    {
        RemoveSubscriptions();
    }

    protected virtual void OnTimerFinished()
    {
        Destroy(gameObject);
    }
}
