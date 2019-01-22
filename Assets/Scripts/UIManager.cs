using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Canvas canvas;
    public Button prefabAbilityButton;
    public Button prefabCharacterButton;
    public RectTransform SkillZone;
    public RectTransform TimelineZone;

    private void Start()
    {

    }
   
    public int UpdateAbilities(LivingPlaceable character,Vector3Int position)
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
            button.onClick.AddListener(skill.Activate);
            numberInstantiated++;
        }
        if(character.EquipedWeapon!=null && character.EquipedWeapon.Skills!=null)
        { 
        foreach(Skill skill in character.EquipedWeapon.Skills)
        {
            Button button = Instantiate(prefabAbilityButton, SkillZone);
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-164 + 60 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.abilitySprite;
            button.onClick.AddListener(skill.Activate);
            numberInstantiated++;
        }
        }
        foreach (ObjectOnBloc obj in GameManager.instance.GetObjectsOnBlockUnder(position))
        {
            foreach(Skill skill in obj.GivenSkills )
            { 
            Button button = Instantiate(prefabAbilityButton, SkillZone);
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-164 + 60 * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.abilitySprite;
            button.onClick.AddListener(skill.Activate);
            numberInstantiated++;
            }
        }
        return numberInstantiated;
    }

    public void UpdateTimeline()
    {
        int numberInstantiated = 0;
        foreach (StackAndPlaceable character in GameManager.instance.TurnOrder)
        {
            Button button = Instantiate(prefabCharacterButton, TimelineZone);
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 120 - 46 * numberInstantiated);
            button.GetComponentInChildren<Image>().sprite = character.Character.characterSprite;
            if (character.Character.Player.gameObject == gameObject)
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
        if (GameManager.instance.playingPlaceable.Player.gameObject == gameObject)
        {
            GameObject zoneToclear = gameObject.transform.Find("Canvas").Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            UpdateAbilities(GameManager.instance.playingPlaceable, GameManager.instance.playingPlaceable.GetPosition());//WARNING can be messed up with animation and fast change of turn
            zoneToclear = gameObject.transform.Find("Canvas").Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("Canvas").Find("SkipButton").gameObject.SetActive(true);
        }
        else if (GameManager.instance.playingPlaceable.Player.gameObject != gameObject)
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
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }
    }
}
