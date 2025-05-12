using System;
using UnityEngine;

public abstract class GroundEnemy : AEnemy
{
    public override void Init(float healthAmount, float speed, int damage, Action<AEnemy> onEnemyDied, Action<AEnemy> reachedTarget)
    {
        base.Init(healthAmount, speed, damage, onEnemyDied, reachedTarget);

        movementTarget = GridManager.Instance.GridTiles[GridManager.Instance.Path[currentPathIndex].x, GridManager.Instance.Path[currentPathIndex].y].transform.position.IgnoreAxis(EAxis.Y, transform.position.y);
    }

    protected virtual void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, movementTarget, movementSpeed * Time.deltaTime);

        if (movementTarget.IsNearlyEqual(transform.position.IgnoreAxis(EAxis.Y, movementTarget.y), 0.1f))
        {
            if (currentPathIndex == 0)
            {
                if (reachedTarget != null) reachedTarget.Invoke(this);
                return;
            }
            currentPathIndex--;
            movementTarget = GridManager.Instance.GridTiles[GridManager.Instance.Path[currentPathIndex].x, GridManager.Instance.Path[currentPathIndex].y].transform.position.IgnoreAxis(EAxis.Y, transform.position.y);
        }
    }

    public virtual float GetProgress()
    {
        // not Perfekt but good enough
        return Mathf.Clamp01(currentPathIndex / GridManager.Instance.Path.Count - 1);
    }
}
