using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Transform trainingCanvasTransform;
    [SerializeField] GameObject buildingCanvas;

    int MAX_COLLISIONS_HELD = 10;

    public void OnBeginDrag(PointerEventData data)
    {
        //get sibling index to retain module order on canvas
        int siblingIndex = gameObject.transform.GetSiblingIndex();
        //make a copy of the selected module that will be left behind
        GameObject clone = Instantiate(gameObject, gameObject.transform.parent);
        //make clone take place of original
        clone.transform.SetSiblingIndex(siblingIndex);
        //move current GameObject from module canvas to training canvas
        gameObject.transform.SetParent(trainingCanvasTransform, true);

        //update position
        UpdatePosition(data);

    }
    public void OnDrag(PointerEventData data)
    {
        //while module is being dragged, update position based on cursor movement
        UpdatePosition(data);
    }
    public void OnDrop(PointerEventData data)
    {
        //when module is released, check where module is:
        //1. destroy if not in building canvas or there is no appropriate module
        //2. attach if pointer is over appropriate module (secondary over primary)

        //create array to get results of all current collisions
        BoxCollider2D moduleCollider = gameObject.GetComponent<BoxCollider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        BoxCollider2D[] currentCollisions = new BoxCollider2D[MAX_COLLISIONS_HELD];
        //fill array with all current collisions
        int collisions = moduleCollider.OverlapCollider(contactFilter.NoFilter(), currentCollisions);

        //now that array is filled, check collisions for appropriate module
        if (gameObject.TryGetComponent(out PrimaryModule pMod))
        {
            //check if module is in the building canvas
            if (!inCanvas())
            {
                Destroy(gameObject);
                return;
            }

            //if module is primary module, add to building canvas
            gameObject.transform.SetParent(buildingCanvas.transform);
        }
        else if (gameObject.TryGetComponent(out SecondaryModule sMod))
        {
            bool parentFound = false;
            //if module is secondary module, add to first unfilled primary module
            for (int collisionIndex = 0; collisionIndex < collisions; collisionIndex++)
            {
                //check if current collision is a primary module
                if (currentCollisions[collisionIndex].gameObject.TryGetComponent(out PrimaryModule collisionPMod))
                {
                    if (collisionPMod.searchModule == null)
                    {
                        //if collision has an empty slot for a secondary module, fill it
                        collisionPMod.searchModule = sMod;
                        gameObject.transform.SetParent(currentCollisions[collisionIndex].gameObject.transform);
                        RectTransform moduleTransform = gameObject.GetComponent<RectTransform>();
                        moduleTransform.localPosition = Vector3.zero;
                        parentFound = true;
                        break;
                    }
                }
            }
            if (!parentFound)
            {
                Destroy(gameObject);
            }
        }
        else if (gameObject.TryGetComponent(out TertiaryModule tMod))
        {
            bool parentFound = false;
            //if module is tertiary module, add to first unfilled secondary module
            for (int collisionIndex = 0; collisionIndex < collisions; collisionIndex++)
            {
                //check if current collision is a secondary module
                if (currentCollisions[collisionIndex].gameObject.TryGetComponent(out SecondaryModule collisionSMod))
                {
                    if (collisionSMod.actionModule == null)
                    {
                        //if collision has an empty slot for a tertiary module, fill it
                        collisionSMod.actionModule = tMod;
                        gameObject.transform.SetParent(currentCollisions[collisionIndex].gameObject.transform);
                        RectTransform moduleTransform = gameObject.GetComponent<RectTransform>();
                        moduleTransform.localPosition = Vector3.zero;
                        parentFound = true;
                        break;
                    }
                }
            }
            if (!parentFound)
            {
                Destroy(gameObject);
            }
        }

        //disable this script once module is placed in building canvas
        this.enabled = false;
    }
    private void UpdatePosition(PointerEventData data)
    {
        //load current module's x-y position as a Vector2
        Vector2 modulePosition = new Vector2(gameObject.transform.position.x, 
            gameObject.transform.position.y);
        //update module's position based on pointer delta (change in position)
        modulePosition += data.delta;
        //load new position back into current module
        gameObject.transform.position = modulePosition;
    }
    private bool inCanvas()
    {
        BoxCollider2D moduleCollider = gameObject.GetComponent<BoxCollider2D>();
        BoxCollider2D buildingCanvasCollider = buildingCanvas.GetComponent<BoxCollider2D>();

        return moduleCollider.IsTouching(buildingCanvasCollider);
    }
}
