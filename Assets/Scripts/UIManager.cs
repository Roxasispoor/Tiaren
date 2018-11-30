using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject gameCanvas;
    public GameObject spawnCanvas;
    public Button prefabAbilityButton;
    public Button prefabCharacterButton;
    public RectTransform SkillZone;
    public RectTransform SpawnZone;
    public RectTransform TimelineZone;

    public void SpawnUI()
    {
        int numberInstantiated = 0;
        if (gameObject == GameManager.instance.player1)
        {
            foreach (GameObject character in GameManager.instance.player1.GetComponent<Player>().Characters) {
                Button button = Instantiate(prefabAbilityButton, SpawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-170 + 45 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.GetComponent<LivingPlaceable>().characterSprite;
                button.onClick.AddListener(character.GetComponent<LivingPlaceable>().ChangeSpawnCharacter);
            }
        }
        if (gameObject == GameManager.instance.player2)
        {
            foreach (GameObject character in GameManager.instance.player2.GetComponent<Player>().Characters)
            {
                Button button = Instantiate(prefabAbilityButton, SpawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-170 + 45 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.GetComponent<LivingPlaceable>().characterSprite;
                button.onClick.AddListener(character.GetComponent<LivingPlaceable>().ChangeSpawnCharacter);
                //button.onClick.AddListener(() => { button.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f); });
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
            button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-164 + 60 * numberInstantiated, 0);
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
        if (GameManager.instance.playingPlaceable.player.gameObject == gameObject)
        {
            GameObject zoneToclear = gameCanvas.transform.Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            UpdateAbilities(GameManager.instance.playingPlaceable);
            zoneToclear = gameCanvas.transform.Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameCanvas.transform.Find("SkipButton").gameObject.SetActive(true);
        }
        else if (GameManager.instance.playingPlaceable.player.gameObject != gameObject)
        {
            GameObject zoneToclear = gameCanvas.transform.Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            zoneToclear = gameCanvas.transform.Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameCanvas.transform.Find("SkipButton").gameObject.SetActive(false);
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
