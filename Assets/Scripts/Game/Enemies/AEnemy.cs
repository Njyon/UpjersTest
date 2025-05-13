using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : MonoBehaviour 
{
    [SerializeField] protected float spawnHight;
    protected int currentPathIndex;
    protected ResourceBase health;
    protected float movementSpeed;
    public Action<AEnemy> onEnemyDied;
    public Action<AEnemy> reachedTarget;
    protected Vector3 movementTarget;
    protected int damage;
    protected int reward;
    public int Damage
    {
        get { return damage; }
    }

    public virtual void Init(float healthAmount, float speed, int damage, int reward)
    {
        // Set Current to last
        currentPathIndex = GridManager.Instance.Path.Count - 1;
        // Set Enemy on Tile position without Y Axis
        transform.position = GridManager.Instance.GridTiles[GridManager.Instance.Path[currentPathIndex].x, GridManager.Instance.Path[currentPathIndex].y].transform.position;
        // Set Enemy Y Axis
        transform.position = new Vector3(transform.position.x, transform.position.y + (GridManager.Instance.TileSize.y / 2) + spawnHight, transform.position.z);

        health = new ResourceBase(healthAmount, healthAmount);
        movementSpeed = speed;
        this.damage = damage;
        this.reward = reward;

        health.onCurrentValueChange += OnHealthValueChange;
        // Set currentPathIndex to next tile
        currentPathIndex--;
    }

    protected virtual void Update()
    {
        Move();
    }

    void OnDestroy()
    {
        if (health != null) health.onCurrentValueChange -= OnHealthValueChange;
    }

    protected virtual void OnHealthValueChange(float newValue, float oldValue)
    {
        if (newValue <= 0)
        {
            GameManager.Instance.ResourceAccountant.AddCurrentResourceValue(CurrencyType.Gold, reward);
            if (onEnemyDied != null) onEnemyDied.Invoke(this);
        }
    }

    public virtual float GetPathProgress() 
    {
        return 0;
    }

    protected virtual void Move() { }

    public virtual void DoDamage(float damage)
    {
        health.AddCurrentValue(-damage);
    }
}
