using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridTile : MonoBehaviour, ISelectable
{
    bool isPath;
    public bool IsPath
    {
        get { return isPath; }
        set { isPath = value; }
    }

    bool isBuildOn;
    public bool IsBuildOn
    {
        get { return isBuildOn; }
        set { isBuildOn = value; }
    }

    public bool IsBlocked
    {
        get { return IsPath || IsBuildOn; }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #region Debug Tile
    // Implement Selectable to debug Tile
    public void Select()
    {
        Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Teal + gameObject.name + " Selected | Is blocked = " + IsBlocked + StringColor.EndColor, 5);
    }

    public void Unselect()
    {

    }
    #endregion
}
