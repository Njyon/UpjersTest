using Unity.VisualScripting;
using UnityEngine;

public abstract class AirEnemy : AEnemy
{
    Vector3 startPosition;
    public override void Init(float healthAmount, float speed, int damage, int reward)
    {
        base.Init(healthAmount, speed, damage, reward);

        startPosition = transform.position;
        movementTarget = GridManager.Instance.GridTiles[GridManager.Instance.Path[0].x, GridManager.Instance.Path[0].y].transform.position.IgnoreAxis(EAxis.Y, transform.position.y);
    }

    protected override void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, movementTarget, movementSpeed * Time.deltaTime);

    }

    public virtual float GetProgress()
    {
        Vector3 startToEnd = movementTarget - startPosition;
        Vector3 startToCurrent = transform.position - startPosition;

        float totalDistance = startToEnd.magnitude;
        if (totalDistance == 0f)
            return 0f;

        float projectedLength = Vector3.Dot(startToCurrent, startToEnd.normalized);
        return Mathf.Clamp01(projectedLength / totalDistance);
    }
}
