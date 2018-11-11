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

    private void Start()
    {
        gameManager = GameManager.instance;
    }

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
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-164 + 60 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.abilitySprite;
            //button.onClick.AddListener(skill.hightlight(skill));
            numberInstantiated++;
        }
        return numberInstantiated;
    }

    public void UpdateTimeline()
    {
        int numberInstantiated = 0;
        foreach (StackAndPlaceable character in gameManager.TurnOrder)
        {
            Button button = Instantiate(prefabCharacterButton, TimelineZone);
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 120 - 46 * numberInstantiated);
            button.GetComponentInChildren<Image>().sprite = character.Character.characterSprite;
            if (character.Character.player.gameObject == gameObject)
            {
                button.GetComponent<Image>().color = Color.cyan;
            }
            else
            {
                button.GetComponent<Image>().color = Color.red;
            }
            numberInstantiated++;
        }
    }

    public void ChangeTurn()
    {
        if (gameManager.playingPlaceable.player.gameObject == gameObject)
        {
            GameObject zoneToclear = gameObject.transform.Find("Canvas").Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            UpdateAbilities(gameManager.playingPlaceable);
            zoneToclear = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("Canvas").Find("SkipButton").gameObject.SetActive(true);
        }
        else if (gameManager.playingPlaceable.player.gameObject != gameObject)
        {
            GameObject zoneToclear = gameObject.transform.Find("Canvas").Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            zoneToclear = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("Canvas").Find("SkipButton").gameObject.SetActive(false);
        }
    }

    public void ClearZone(GameObject zoneToClear)
    {
        foreach (Transform child in zoneToClear.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
