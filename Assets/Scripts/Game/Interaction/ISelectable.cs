using UnityEngine;

public interface ISelectable
{
    public void Select();
    public void Unselect();
}

public interface IContextAction
{
    public void Action();
    public void Close();
}
