using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject gameCanvas;
    public GameObject spawnCanvas;
    public GameObject TeamCanvas;
    public GameObject prefabAbilityButton;
    public Button prefabTeamButton;
    public Button prefabCharacterSpawnButton;
    public Sprite upChoice;
    public GameObject prefabCharacterImage;
    public GameObject basicImage;
    public RectTransform SkillZone;
    public RectTransform SpawnZone;
    public RectTransform TimelineZone;
    public GameObject hpDisplay;
    public Text movDisplay;
    public Image prefabCharacterChoices;
    public int numberOfPlayer;
    public float firstImagePosition;
    public float firstGap;
    public float timelineGap;
    public float AbilityGap;

    private List<GameObject> TeamParents = new List<GameObject>();
    private List<int> currentCharacters = new List<int>();

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

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void TeamSelectUI()
    {
        if (gameObject.GetComponent<Player>().isLocalPlayer)
        {
            TeamCanvas.transform.Find("TitleText").GetComponent<Text>().text = "Choose your characters";
            TeamCanvas.transform.Find("GoTeam").gameObject.SetActive(true);
            //display UI
            for (int i = 0; i < numberOfPlayer; i++)
            {
                currentCharacters.Add(mod(i, GameManager.instance.PossibleCharacters.Count));
                //display all sprites and hide them
                Vector3 position = new Vector3(-700 + 350 * i, 0);
                TeamParents.Add(new GameObject("Parent" + i.ToString()));
                TeamParents[i].transform.parent = TeamCanvas.transform;
                TeamParents[i].transform.localPosition = Vector3.zero;
                TeamParents[i].transform.localScale = Vector3.one;
                for (int j = 0; j < GameManager.instance.PossibleCharacters.Count; j++)
                {
                    Image image = Instantiate(prefabCharacterChoices, TeamParents[i].transform);
                    image.GetComponent<RectTransform>().transform.localPosition = position;
                    Sprite sprite = Resources.Load<Sprite>(GameManager.instance.PossibleCharacters[j].spritePath);
                    image.sprite = sprite;
                    if (j != currentCharacters[i])
                    {
                        image.gameObject.SetActive(false);
                    }
                }

                //display Buttons
                Button buttonUp = Instantiate(prefabTeamButton, TeamParents[i].transform);
                buttonUp.GetComponent<RectTransform>().transform.localPosition = new Vector3(-700 + 350 * i, 300);
                buttonUp.image.sprite = upChoice;
                buttonUp.transform.Rotate(new Vector3(0, 0, -1), 180);
                int tmp = i;
                buttonUp.onClick.AddListener(() => { UpChoice(tmp); });
                Button buttonDown = Instantiate(prefabTeamButton, TeamParents[i].transform);
                buttonDown.GetComponent<RectTransform>().transform.localPosition = new Vector3(-700 + 350 * i, -300);
                buttonDown.image.sprite = upChoice;
                buttonDown.onClick.AddListener(delegate { DownChoice(tmp); });
            }
        }
    }

    public void UpChoice(int i)
    {
        Image[] images = TeamParents[i].GetComponentsInChildren<Image>(true);
        images[CurrentCharacters[i]].gameObject.SetActive(false);
        CurrentCharacters[i] = mod(CurrentCharacters[i] + 1, GameManager.instance.PossibleCharacters.Count);
        images[CurrentCharacters[i]].gameObject.SetActive(true);
    }

    public void DownChoice(int i)
    {
        Image[] images = TeamParents[i].GetComponentsInChildren<Image>(true);
        images[CurrentCharacters[i]].gameObject.SetActive(false);
        CurrentCharacters[i] = mod(CurrentCharacters[i] - 1, GameManager.instance.PossibleCharacters.Count);
        images[CurrentCharacters[i]].gameObject.SetActive(true);
    }

    private void Update()
    {    
        if (GameManager.instance.State != States.Spawn && GameManager.instance.State != States.TeamSelect && GameManager.instance.PlayingPlaceable!=null && gameObject.GetComponent<Player>().isLocalPlayer)
        {
            if (GameManager.instance.PlayingPlaceable.Player == gameObject.GetComponent<Player>())
            {
                hpDisplay.transform.Find("HPDisplay").GetComponent<Text>().text = "HP : " + GameManager.instance.PlayingPlaceable.CurrentHP + " / " + GameManager.instance.PlayingPlaceable.MaxHP;
                hpDisplay.transform.Find("Bar").GetComponent<Image>().fillAmount = GameManager.instance.PlayingPlaceable.CurrentHP / GameManager.instance.PlayingPlaceable.MaxHP;
                movDisplay.text = "MOV : " + GameManager.instance.PlayingPlaceable.CurrentPM;
            }
            GameObject zoneToUpdate = gameObject.transform.Find("InGameCanvas").Find("Timeline").gameObject;
            Slider[] sliders = zoneToUpdate.GetComponentsInChildren<Slider>();
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = GameManager.instance.TurnOrder[i].Character.CurrentHP;
            }

            Button[] buttons = SkillZone.transform.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.GetComponent<SkillInfo>().Skill.TourCooldownLeft > 0)
                {
                    button.GetComponent<Image>().color = Color.gray;
                    button.transform.Find("Cooldown").GetComponent<Text>().text = button.GetComponent<SkillInfo>().Skill.TourCooldownLeft.ToString();
                }
            }
        }
    }
    public void SpawnUI()
    {
        TeamCanvas.SetActive(false);
        spawnCanvas.SetActive(true);
        int numberInstantiated = 0;
        if (gameObject == GameManager.instance.player1)
        {
            foreach (GameObject character in GameManager.instance.player1.GetComponent<Player>().Characters)
            {
                Button button = Instantiate(prefabCharacterSpawnButton, SpawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-398 + 200 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.GetComponent<LivingPlaceable>().characterSprite;
                button.onClick.AddListener(character.GetComponent<LivingPlaceable>().ChangeSpawnCharacter);
                numberInstantiated++;
            }
        }
        if (gameObject == GameManager.instance.player2)
        {
            foreach (GameObject character in GameManager.instance.player2.GetComponent<Player>().Characters)
            {
                Button button = Instantiate(prefabCharacterSpawnButton, SpawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-398 + 200 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.GetComponent<LivingPlaceable>().characterSprite;
                button.onClick.AddListener(character.GetComponent<LivingPlaceable>().ChangeSpawnCharacter);
                numberInstantiated++;
            }
        }

    }

    public int UpdateAbilities(LivingPlaceable character, Vector3Int position)

    {

        if (character == null)
        {
            return 0;
        }
        GameObject zoneToclear = gameCanvas.transform.Find("Skill Zone").gameObject;
        ClearZone(zoneToclear);
        int numberInstantiated = 0;

        foreach (Skill skill in character.Skills)
        {
            skill.TourCooldownLeft--;
            GameObject ability = Instantiate(prefabAbilityButton, SkillZone);
            Button button = ability.GetComponentInChildren<Button>();
            button.GetComponent<SkillInfo>().Skill = skill;
            ability.transform.localPosition = new Vector3(-431 + AbilityGap * numberInstantiated, 0);
            button.GetComponentInChildren<Image>().sprite = skill.AbilitySprite;
            if (skill.TourCooldownLeft > 0)
            {
                //button.GetComponent<Text>().text = skill.TourCooldownLeft.ToString();
                button.GetComponentInChildren<Image>().color = Color.gray;
            }
            button.onClick.AddListener(skill.Activate);
            button.onClick.AddListener(SoundHandler.Instance.PlayUISound);
            numberInstantiated++;
        }
        if (character.EquipedWeapon != null && character.EquipedWeapon.Skills != null)
        {
            foreach (Skill skill in character.EquipedWeapon.Skills)
            {
                Button button = Instantiate(prefabAbilityButton, SkillZone).GetComponentInChildren<Button>();
                button.GetComponent<SkillInfo>().Skill = skill;
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-431 + AbilityGap * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = skill.AbilitySprite;
                button.onClick.AddListener(skill.Activate);
                button.onClick.AddListener(SoundHandler.Instance.PlayUISound);
                numberInstantiated++;
                Debug.Log("new skill from weapon");
            }
        }
        foreach (ObjectOnBloc obj in GameManager.instance.GetObjectsOnBlockUnder(position))
        {
            foreach (Skill skill in obj.GivenSkills)
            {
                Button button = Instantiate(prefabAbilityButton, SkillZone).GetComponentInChildren<Button>();
                button.GetComponent<SkillInfo>().Skill = skill;
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-431 + AbilityGap * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = skill.AbilitySprite;
                button.onClick.AddListener(skill.Activate);
                button.onClick.AddListener(SoundHandler.Instance.PlayUISound);
                numberInstantiated++;
                Debug.Log("new skill from objectonbloc");
            }
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
            image.GetComponentInChildren<Image>().sprite = character.Character.characterSprite;
            image.GetComponentInChildren<Slider>().maxValue = character.Character.MaxHP;
            image.GetComponentInChildren<Slider>().value = character.Character.CurrentHP;
            GameObject filter = Instantiate(basicImage, image.transform);
            if (character.Character.Player.gameObject == gameObject)
            {
                filter.GetComponent<Image>().color = new Color(0, 1, 1, 0.5f);
            }
            else
            {
                filter.GetComponent<Image>().color = new Color(1, 0, 0, 0.5f);
            }
            if (numberInstantiated >= 1)
            {
                image.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                image.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, firstImagePosition - firstGap - timelineGap * (numberInstantiated - 1));
            }
            else
            {
                image.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, firstImagePosition);
            }
            numberInstantiated++;
        }
    }

    public void ChangeTurn()
    {
        if (GameManager.instance.playingPlaceable.Player.gameObject == gameObject)
        {
            GameObject zoneToclear = gameCanvas.transform.Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);

            UpdateAbilities(GameManager.instance.playingPlaceable, GameManager.instance.playingPlaceable.GetPosition());//WARNING can be messed up with animation and fast change of turn
            zoneToclear = gameCanvas.transform.Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("InGameCanvas").Find("SkipButton").gameObject.SetActive(true);
            hpDisplay.SetActive(true);
        }
        else if (GameManager.instance.playingPlaceable.Player.gameObject != gameObject)
        {
            GameObject zoneToclear = gameCanvas.transform.Find("Skill Zone").gameObject;
            ClearZone(zoneToclear);
            zoneToclear = gameCanvas.transform.Find("Timeline").gameObject;
            ClearZone(zoneToclear);
            UpdateTimeline();
            gameObject.transform.Find("InGameCanvas").Find("SkipButton").gameObject.SetActive(false);
            hpDisplay.SetActive(false);
        }
    }

    public void ResetEndTurn()
    {
        gameObject.GetComponentInChildren<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Deactivate();
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
