using Ultra;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingelton<UIManager>
{
    // there is probably a better solution than this but good enough for now
    public GraphicRaycaster graphicRaycaster;

    public ResourceDisplayer healthVisulizer;
    public ResourceDisplayer goldVisulizer;
}
