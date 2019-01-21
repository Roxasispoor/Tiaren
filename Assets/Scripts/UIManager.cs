using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas canvas;
    public Button prefabAbilityButton;
    public GameObject prefabCharacterImage;
    public RectTransform SkillZone;
    public RectTransform TimelineZone;
    public GameObject hpDisplay;
    public Text movDisplay;

    private void Update()
    {
        if (GameManager.instance.state != States.Spawn)
        {
            if (GameManager.instance.PlayingPlaceable.Player == gameObject.GetComponent<Player>())
            {
                hpDisplay.transform.Find("HPDisplay").GetComponent<Text>().text = "HP : " + GameManager.instance.PlayingPlaceable.CurrentHP + " / " + GameManager.instance.PlayingPlaceable.MaxHP;
                hpDisplay.transform.Find("Bar").GetComponent<Image>().fillAmount = GameManager.instance.PlayingPlaceable.CurrentHP / GameManager.instance.PlayingPlaceable.MaxHP;
                movDisplay.text = "MOV : " + GameManager.instance.PlayingPlaceable.CurrentPM;
            }
            GameObject zoneToUpdate = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            Slider[] sliders = zoneToUpdate.GetComponentsInChildren<Slider>();
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = GameManager.instance.TurnOrder[i].Character.CurrentHP;
            }
        }
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
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-431 + 106 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.abilitySprite;
            button.onClick.AddListener(skill.Activate);
            numberInstantiated++;
        }
        return numberInstantiated;
    }

    public void UpdateTimeline()
    {
        int numberInstantiated = 0;
        foreach (StackAndPlaceable character in GameManager.instance.TurnOrder)
        {
            GameObject image = Instantiate(prefabCharacterImage, TimelineZone);
            image.GetComponent<CharacterDisplay>().Character = character.Character;
            image.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 340 - 110 * numberInstantiated);
            image.GetComponentInChildren<Image>().sprite = character.Character.characterSprite;
            image.GetComponentInChildren<Slider>().maxValue = character.Character.MaxHP;
            image.GetComponentInChildren<Slider>().value = character.Character.CurrentHP;
            if (character.Character.Player.gameObject == gameObject)
            {
                image.GetComponent<Image>().color = Color.cyan;
            }
            else
            {
                image.GetComponent<Image>().color = Color.red;
            }
            numberInstantiated++;
        }
    }

    public void ChangeTurn()
    {
        if (GameManager.instance.playingPlaceable.Player.gameObject == gameObject)
        {
            GameObject zoneToclear = gameObject.transform.Find("Canvas").Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            UpdateAbilities(GameManager.instance.playingPlaceable);
            zoneToclear = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("Canvas").Find("SkipButton").gameObject.SetActive(true);
            hpDisplay.SetActive(true);
        }
        else if (GameManager.instance.playingPlaceable.Player.gameObject != gameObject)
        {
            GameObject zoneToclear = gameObject.transform.Find("Canvas").Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            zoneToclear = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("Canvas").Find("SkipButton").gameObject.SetActive(false);
            hpDisplay.SetActive(false);
        }
    }

    public void ClearZone(GameObject zoneToClear)
    {
        foreach (Transform child in zoneToClear.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                child.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            Destroy(child.gameObject);
        }
    }
}
