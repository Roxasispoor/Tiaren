using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    private LivingPlaceable character;
    public bool isHovered = false;
    private StatDisplayer displayer;

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

    private void Awake()
    {
        displayer = gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.GetLocalPlayer().cameraScript.SetTarget(Character.gameObject.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        displayer.Activate(Character);
        isHovered = true;
        character.Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        displayer.Deactivate();
        isHovered = false;
        character.UnHighlight();
        displayer.Activate(GameManager.instance.PlayingPlaceable);
    }

    public void PreviewEffect(int damage, int turns)
    {
        displayer.Preview(Character, damage, turns);
    }

    public void ResetPreview()
    {
        displayer.Activate(GameManager.instance.PlayingPlaceable);
    }

    public void Update()
    {
    }
    
}
