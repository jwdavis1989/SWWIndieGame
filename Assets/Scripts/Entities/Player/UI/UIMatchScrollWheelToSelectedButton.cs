using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMatchScrollWheelToSelectedButton : MonoBehaviour
{
    [SerializeField] GameObject currentSelected;
    [SerializeField] GameObject previouslySelected;
    [SerializeField] RectTransform currentSelectedTransform;

    [SerializeField] RectTransform contentPanel;
    [SerializeField] ScrollRect scrollRect;

    private void Update() {
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null) {
            //Check if this has updated in the last frame
            if (currentSelected != previouslySelected) {
                previouslySelected = currentSelected;
                currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                SnapTo(currentSelectedTransform);
            }
        }
    }

    private void SnapTo(RectTransform target) {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition = 
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - 
            (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        //We only want to lock the position on the Y Axis
        newPosition.x = 0;

        contentPanel.anchoredPosition = newPosition;
    }

}
