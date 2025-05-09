using UnityEngine;

public interface IRequestOwner
{
    public void QueueRequest(RequestTransaction request);
}
