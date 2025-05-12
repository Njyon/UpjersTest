using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerSelectionComponent
{
    Camera camera;
    float pointInteractionRayLengh;
    IHoverable cachedHoverable;
    IHoverable CachedHoverable
    {
        get { return cachedHoverable; }
        set {
            if (cachedHoverable == value) return;
            if (value != null)
            {
                if (cachedHoverable != null)
                {
                    cachedHoverable.UnHovered();
                    value.UnHovered();
                }
                value.Hovered();
            }
            else
            {
                if (cachedHoverable != null)
                    cachedHoverable.UnHovered();
            }
            cachedHoverable = value; 
        }
    }

    public PlayerSelectionComponent(Camera camera, float pointInteractionRayLengh)
    {
        this.camera = camera;
        this.pointInteractionRayLengh = pointInteractionRayLengh;

        // LOGS
        if (camera == null)
            Debug.Log(Ultra.Utilities.Instance.DebugErrorString("PlayerSelectionComponent", "PlayerSelectionComponentContstruktor", "Camera was NULL!"));
    }

    public void PointSelection(Vector2 selectionPoint)
    {
        if (IsUIBlockingPointSelect(selectionPoint)) return;
        ISelectable interactable = GetInteractable(selectionPoint, pointInteractionRayLengh);
        SelectionManager.Instance.Select(interactable);
    }

    ISelectable GetInteractable(Vector2 interactionPoint, float rayLength)
    {
        if (camera == null)
        {
            Debug.Log(Ultra.Utilities.Instance.DebugErrorString("PlayerSelectionComponent", "GetInteractable", "Camera was NULL!"));
            return null;
        }
        Ray ray = camera.ScreenPointToRay(interactionPoint);

        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        foreach (var hit in hits)
        {
            ISelectable interactable = hit.collider.GetComponent<ISelectable>();
            if (interactable != null)
            {
                return interactable;
            }
        }

        return null;
    }

    // Block input by UI to block unwanted behaviour
    bool IsUIBlockingPointSelect(Vector2 selectionPoint)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        UIManager.Instance.graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null)
            {
                Debug.Log("Getroffenes UI-Element: " + result.gameObject.name);
                return true;
            }
        }
        return false;
    }

    public void Select(ISelectable interactable)
    {
        if (interactable == null) return;
        interactable.Select();
    }


    public void TryToHover(Vector2 mousePosition)
    {
        GetHoverable(mousePosition, out IHoverable newHoverable);
        CachedHoverable = newHoverable;
        //if (cachedHoverable != null)
        //{
        //    if (lastHovered != null)
        //    {
        //        if (lastHovered != cachedHoverable) 
        //        {
        //            lastHovered.UnHovered();
        //            cachedHoverable.Hovered();
        //        }
        //    }
        //}else
        //{
        //    if (lastHovered != null)
        //    {
        //        lastHovered.UnHovered();
        //    }
        //}
    }

    private void GetHoverable(Vector2 mousePosition, out IHoverable interactable)
    {
        interactable = null;

        if (camera == null)
        {
            Debug.Log(Ultra.Utilities.Instance.DebugErrorString("PlayerSelectionComponent", "GetInteractable", "Camera was NULL!"));
            return;
        }
        Ray ray = camera.ScreenPointToRay(mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray, pointInteractionRayLengh);

        foreach (var hit in hits)
        {
            IHoverable h = hit.collider.GetComponent<IHoverable>();
            // Check if something is found otherwise it will be likly to return null
            if (h != null)
                interactable = h;
        }
    }
}
