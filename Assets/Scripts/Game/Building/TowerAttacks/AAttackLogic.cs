using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class AAttackLogic 
{
    protected Tower owner;
    public AAttackLogic()
    {

    }

    public virtual void Init(Tower onwer)
    {
        this.owner = onwer;
    }

    public abstract void Attack();
}
