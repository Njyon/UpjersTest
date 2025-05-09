using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RequestTransaction is a class that combines the Requests and the Costs of those so we can Queue them properly and dispose them of the queue
/// </summary>
public class RequestTransaction         // needs to be class to ensure there is no default constructor
{
    public readonly List<ARequest> requests;
    public readonly List<SRequestCost> costs;

    public RequestTransaction(List<ARequest> requests, List<SRequestCost> costs)
    {
        this.requests = requests;
        this.costs = costs;
    }
}
