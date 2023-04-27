using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float sizeMin = 1f, sizeMax = 1.5f;
    [SerializeField] float animSpeed = 3.5f;
    bool animatingHoverEnter, animatingHoverExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToggleHoverState(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleHoverState(false);
    }

    void ToggleHoverState(bool enter)
    {
        animatingHoverEnter = enter;
        animatingHoverExit = !enter;
        StartCoroutine(AnimateHover(enter));
    }

    IEnumerator AnimateHover(bool enter)
    {
        float percent = 0;
        Vector3 oldSize = transform.localScale;
        var desiredSize = Vector3.one * (enter ? sizeMax : sizeMin);
        while (enter ? (transform.localScale.x < sizeMax) && animatingHoverEnter
                     : (transform.localScale.x > sizeMin) && animatingHoverExit)
        {
            transform.localScale = Vector3.Lerp(oldSize,
                                                desiredSize, percent += animSpeed * Time.deltaTime);
            yield return null;
        }
    }
}