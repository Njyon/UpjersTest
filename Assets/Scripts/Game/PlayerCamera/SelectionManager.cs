using System.Collections.Generic;
using Ultra;
using UnityEngine;

// Refactor -> openContext should get its own Manager?

/// <summary>
/// Handle Selection like LeftClick, RectSelection
/// </summary>
public class SelectionManager : MonoSingelton<SelectionManager>
{
    public UltEvents.UltEvent<List<ISelectable>, List<ISelectable>> selectionEvent = new UltEvents.UltEvent<List<ISelectable>, List<ISelectable>>();
    public UltEvents.UltEvent<IContextAction, IContextAction> contextEvent = new UltEvents.UltEvent<IContextAction, IContextAction>();

    List<ISelectable> currentlySelectedObjects = new List<ISelectable>();
    IContextAction currentContext;

    public void Select(ISelectable selection)
    {
        List<ISelectable> tempList = new List<ISelectable> { selection };
        Select(tempList);
    }

    public void OpenContext(IContextAction context)
    {
        if (currentContext != null)
        {
            currentContext.Close();
        }

        IContextAction lastContext = currentContext;
        currentContext = context;

        if (currentContext != null)
        {
            currentContext.Action();
        }

        if (contextEvent != null) contextEvent.Invoke(currentContext, lastContext);
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

        if (selectionEvent != null) selectionEvent.Invoke(currentlySelectedObjects, lastSelection);
    }
}
