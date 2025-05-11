using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemy : MonoBehaviour 
{
    [SerializeField] float spawnHight;
    int currentPathIndex;
    ResourceBase health;
    float movementSpeed;
    Action<AEnemy> onEnemyDied;

    public virtual void Init(float healthAmount, float speed, Action<AEnemy> onEnemyDied)
    {
        // Set Current to last
        currentPathIndex = GridManager.Instance.Path.Count - 1;
        // Set Enemy on Tile position without Y Axis
        transform.position = GridManager.Instance.GridTiles[GridManager.Instance.Path[currentPathIndex].x, GridManager.Instance.Path[currentPathIndex].y].transform.position;
        // Set Enemy Y Axis
        transform.position = new Vector3(transform.position.x, transform.position.y + (GridManager.Instance.TileSize.y / 2) + spawnHight, transform.position.z);

        health = new ResourceBase(healthAmount, healthAmount);
        movementSpeed = speed;

        health.onCurrentValueChange += OnHealthValueChange;

        this.onEnemyDied = onEnemyDied;
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
}
