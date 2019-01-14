using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour {

    public GameObject gameCanvas;
    public GameObject spawnCanvas;
    public Button prefabAbilityButton;
    public Button prefabCharacterButton;
    public Button prefabTeamButton;
    public RectTransform SkillZone;
    public RectTransform SpawnZone;
    public RectTransform TimelineZone;
    public RectTransform characterZone;
    public Image prefabCharacterChoices;

    private List<SpriteAndName> possibleCharacters;
    private List<int> currentCharacters;

    public List<int> CurrentCharacters
    {
        get
        {
            return currentCharacters;
        }

        set
        {
            currentCharacters = value;
        }
    }

    public List<SpriteAndName> PossibleCharacters
    {
        get
        {
            return possibleCharacters;
        }

        set
        {
            possibleCharacters = value;
        }
    }

    private void Start()
    {
        
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void TeamSelectUI()
    {
        if (gameObject == GameManager.instance.player1)
        {
            //init Posiible characters
            string path = "Teams.json";
            string line;

            StreamReader reader = new StreamReader(path);
            while ((line = reader.ReadLine()) != null)
            {
                SpriteAndName spriteAndName = JsonUtility.FromJson<SpriteAndName>(line);
                PossibleCharacters.Add(spriteAndName);
            }

            //display UI
            for (int i = 0; i < 5; ++i)
            {
                //display Sprite
                Vector3 position = new Vector3(-700 + 350 * i, 0);
                Image image = Instantiate(prefabCharacterChoices, position, Quaternion.identity);
                image.sprite = PossibleCharacters[CurrentCharacters[i]].Sprite;

                //display Buttons
                Button buttonUp = Instantiate(prefabTeamButton, image.transform);
                buttonUp.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 300);
                buttonUp.GetComponentInChildren<Text>().text = "^";
                buttonUp.onClick.AddListener(delegate { UpChoice(i); });
                Button buttonDown = Instantiate(prefabTeamButton, image.transform);
                buttonDown.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -300);
                buttonDown.GetComponentInChildren<Text>().text = "!^";
                buttonUp.onClick.AddListener(delegate { DownChoice(i); });

            }
        }
        else if (gameObject == GameManager.instance.player2)
        {
            //Display UI
            for (int i = 0; i < 5; ++i)
            {
                //display Sprite
                Vector3 position = new Vector3(-700 + 350 * i, 0);
                Image image = Instantiate(prefabCharacterChoices, position, Quaternion.identity);
                image.sprite = PossibleCharacters[CurrentCharacters[i]].Sprite;

                //display Buttons
                Button buttonUp = Instantiate(prefabTeamButton, image.transform);
                buttonUp.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 300);
                buttonUp.GetComponentInChildren<Text>().text = "^";
                buttonUp.onClick.AddListener(delegate { UpChoice(i); });
                Button buttonDown = Instantiate(prefabTeamButton, image.transform);
                buttonDown.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -300);
                buttonDown.GetComponentInChildren<Text>().text = "!^";
                buttonUp.onClick.AddListener(delegate { DownChoice(i); });

            }
        }
    }

    public void UpChoice(int i)
    {
        CurrentCharacters[i] = mod(CurrentCharacters[i] + 1, PossibleCharacters.Count);
    }

    public void DownChoice(int i)
    {
        CurrentCharacters[i] = mod(CurrentCharacters[i] - 1, PossibleCharacters.Count);
    }

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
