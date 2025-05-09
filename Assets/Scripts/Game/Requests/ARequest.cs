using System;
using UnityEngine;

[Serializable]
public abstract class ARequest
{
    public IRequestOwner Owner;

    public ARequest() { }

    public abstract void ExecuteRequest(IRequestOwner owner);
}
