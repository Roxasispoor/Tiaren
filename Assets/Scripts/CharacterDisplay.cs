using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private LivingPlaceable character;
    private bool isHovered = false;

    public LivingPlaceable Character
    {
        get
        {
            return character;
        }

        set
        {
            character = value;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>().Activate(Character);
        isHovered = true;
        character.Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>().Deactivate();
        isHovered = false;
        character.UnHighlight();
    }

    public void Update()
    {
        if (isHovered)
        {
            character.OnMouseOverWithLayer();
        }
    }
}
