using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TrainingManager tMan;

    public void OnPointerDown(PointerEventData data)
    {
        SelectModule(gameObject);
    }

    private void SelectModule(GameObject thisGO)
    {
        //get closest member on hierarchy that is a primary module
        if (thisGO.TryGetComponent(out PrimaryModule pm))
        {
            tMan.SelectModule(pm);
        }
        else if (thisGO.TryGetComponent(out SecondaryModule sm))
        {
            if (thisGO.transform.parent.TryGetComponent(out PrimaryModule parentPM))
            {
                tMan.SelectModule(parentPM);
            }
        }
        else if (thisGO.TryGetComponent(out TertiaryModule tm))
        {
            if (thisGO.transform.parent.parent.TryGetComponent(out PrimaryModule grandparentPM))
            {
                tMan.SelectModule(grandparentPM);
            }
        }
    }
}
