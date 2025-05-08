using System.Collections.Generic;
using Ultra;
using UnityEngine;

public class SelectionManager : MonoSingelton<SelectionManager>
{
    public UltEvents.UltEvent<List<ISelectable>, List<ISelectable>> selectionEvent = new UltEvents.UltEvent<List<ISelectable>, List<ISelectable>>();

    List<ISelectable> currentlySelectedObjects = new List<ISelectable>();

    public void Select(ISelectable selection)
    {
        List<ISelectable> tempList = new List<ISelectable> { selection };
        Select(tempList);
    }

    public void Select(List<ISelectable> selection)
    {
        List<ISelectable> realSelection = new List<ISelectable>();
        foreach (ISelectable selectable in selection)
        {
            if (selectable != null)
            {
                realSelection.Add(selectable);
            }
        }

        foreach (ISelectable selectable in currentlySelectedObjects)
        {
            selectable.Unselect();
        }
        List<ISelectable> lastSelection = currentlySelectedObjects;

        currentlySelectedObjects.Clear();

        foreach (ISelectable selectable in realSelection)
        {
            currentlySelectedObjects.Add(selectable);
            selectable.Select();
        }

        selectionEvent.Invoke(currentlySelectedObjects, lastSelection);
    }
}
