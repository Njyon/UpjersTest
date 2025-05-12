using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : MonoBehaviour 
{
    [SerializeField] protected float spawnHight;
    protected int currentPathIndex;
    protected ResourceBase health;
    protected float movementSpeed;
    protected Action<AEnemy> onEnemyDied;
    protected Action<AEnemy> reachedTarget;
    protected Vector3 movementTarget;
    protected int damage;
    public int Damage
    {
        get { return damage; }
    }

    public virtual void Init(float healthAmount, float speed, int damage, Action<AEnemy> onEnemyDied, Action<AEnemy> reachedTarget)
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

        health.onCurrentValueChange += OnHealthValueChange;

        this.onEnemyDied = onEnemyDied;
        this.reachedTarget = reachedTarget;
        // Set currentPathIndex to next tile
        currentPathIndex--;
    }

    void OnDestroy()
    {
        if (health != null) health.onCurrentValueChange -= OnHealthValueChange;
    }

    protected virtual void OnHealthValueChange(float newValue, float oldValue)
    {
        if (newValue <= 0)
        {
            if (onEnemyDied != null) onEnemyDied.Invoke(this);
        }
    }

    public virtual float GetPathProgress() 
    {
        return 0;
    }
}
