using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameManager gameManager;
    public Canvas canvas;
    public Button prefabAbilityButton;
    public Button prefabCharacterButton;
    public RectTransform SkillZone;
    public RectTransform TimelineZone;

    public int UpdateAbilities(LivingPlaceable character)
    {
        if (character == null)
        {
            return 0;
        }
        int numberInstantiated = 0;

        foreach (Skill skill in character.Skills)
        {
            Button button = Instantiate(prefabAbilityButton, SkillZone);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector3(-164 + 60 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.abilitySprite;
            //button.onClick.AddListener(skill.hightlight());
            numberInstantiated++;
        }
        return numberInstantiated;
    }

    public void UpdateTimeline()
    {
        int numberInstantiated = 0;
        foreach (StackAndPlaceable character in gameManager.TurnOrder)
        {
            Button button = Instantiate(prefabAbilityButton, TimelineZone);
            button.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector3(120 + 46 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = character.Character.characterSprite;
            numberInstantiated++;
        }
    }

    public void clearZone(RectTransform zoneToClear)
    {
        while(zoneToClear.childCount > 0)
        {
            Destroy(zoneToClear.GetChild(0));
        }
    }
}
