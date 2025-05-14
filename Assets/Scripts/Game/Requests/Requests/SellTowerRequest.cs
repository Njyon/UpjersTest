using System;
using UnityEngine;

[Serializable]
public class SellTowerRequest : ARequest
{
    public CurrencyTransaction towerCost;
    public Tower tower;

    public SellTowerRequest() : base() { }

    public override void ExecuteRequest(IRequestOwner owner)
    {
        // Invert Cost
        for (int i = 0; i < towerCost.Costs.Count; i++)
        {
            var c = towerCost.Costs[i];
            c.Cost = c.Cost * -1;
            towerCost.Costs[i] = c;
        }
        GameManager.Instance.ResourceAccountant.AddTransaction(towerCost);

        foreach (GridTile tile in tower.gridTilesTheTowerIsBuildOn)
        {
            tile.IsBuildOn = false;
        }

        GameObject.Destroy(tower.gameObject);
    }
}
