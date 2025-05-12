using UnityEngine;

public abstract class AirEnemy : AEnemy
{
    public void Move()
    {
        
    }

    public virtual float GetProgress()
    {
        return 0f;
    }
}
