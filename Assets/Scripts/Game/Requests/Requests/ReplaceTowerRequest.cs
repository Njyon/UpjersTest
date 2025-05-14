using System;
using UnityEngine;

[Serializable]
public class ReplaceTowerRequest : ARequest
{
    public Tower towerToReplace;

    public ReplaceTowerRequest() : base() { }

    public override void ExecuteRequest(IRequestOwner owner)
    {
        BuildingManager.Instance.ReplaceTower(towerToReplace);
    }
}
