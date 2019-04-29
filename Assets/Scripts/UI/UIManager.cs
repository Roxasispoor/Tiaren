using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    //canvas
    public GameObject gameCanvas;
    public GameObject spawnCanvas;
    public GameObject teamCanvas;

    //prefabs
    public GameObject prefabAbilityButton;
    public Button prefabTeamButton;
    public Button prefabCharacterSpawnButton;
    public GameObject prefabCharacterImage;
    public GameObject basicImage;
    public Image prefabCharacterChoices;

    //Zones
    public RectTransform skillZone;
    public RectTransform specialSkillZone;
    public RectTransform spawnZone;
    public RectTransform timelineZone;

    //Link related
    public GameObject linkSkillZone;

    //Usefull ui elements
    public GameObject playerTurnObjects;
    public Sprite upChoice;
    public Image hpBar;
    public TMP_Text hpDisplay;
    public TMP_Text movDisplay;
    public TMP_Text paDisplay;
    public Image yourTurnCard;
    public Image ennemyTurnCard;
    public Image camModeDisplay;

    //Sprites to keep
    public Sprite worldCam;
    public Sprite charaCam;

    //Ability buttons
    private List<GameObject> abilityButtons;

    //usefull numbers
    public int numberOfPlayer;
    public float firstImagePosition;
    public float firstGap;
    public float timelineGap;
    public float abilityGap;
    public float specialAbilityGap;
    public float firstAbility;
    public float firstSpecialAbility;

    private List<GameObject> teamParents = new List<GameObject>();
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
            teamCanvas.transform.Find("TitleText").GetComponent<Text>().text = "Choose your characters";
            teamCanvas.transform.Find("GoTeam").gameObject.SetActive(true);
            //display UI
            for (int i = 0; i < numberOfPlayer; i++)
            {
                currentCharacters.Add(mod(i, GameManager.instance.PossibleCharacters.Count));
                //display all sprites and hide them
                Vector3 position = new Vector3(-700 + 350 * i, 0);
                teamParents.Add(new GameObject("Parent" + i.ToString()));
                teamParents[i].transform.parent = teamCanvas.transform;
                teamParents[i].transform.localPosition = Vector3.zero;
                teamParents[i].transform.localScale = Vector3.one;
                for (int j = 0; j < GameManager.instance.PossibleCharacters.Count; j++)
                {
                    Image image = Instantiate(prefabCharacterChoices, teamParents[i].transform);
                    image.GetComponent<RectTransform>().transform.localPosition = position;
                    Sprite sprite = Resources.Load<Sprite>(GameManager.instance.PossibleCharacters[j].spritePath);
                    image.sprite = sprite;
                    if (j != currentCharacters[i])
                    {
                        image.gameObject.SetActive(false);
                    }
                }

                //display Buttons
                Button buttonUp = Instantiate(prefabTeamButton, teamParents[i].transform);
                buttonUp.GetComponent<RectTransform>().transform.localPosition = new Vector3(-700 + 350 * i, 300);
                buttonUp.image.sprite = upChoice;
                buttonUp.transform.Rotate(new Vector3(0, 0, -1), 180);
                int tmp = i;
                buttonUp.onClick.AddListener(() => { UpChoice(tmp); });
                Button buttonDown = Instantiate(prefabTeamButton, teamParents[i].transform);
                buttonDown.GetComponent<RectTransform>().transform.localPosition = new Vector3(-700 + 350 * i, -300);
                buttonDown.image.sprite = upChoice;
                buttonDown.onClick.AddListener(delegate { DownChoice(tmp); });
            }
        }
    }

    public void UpChoice(int i)
    {
        Image[] images = teamParents[i].GetComponentsInChildren<Image>(true);
        images[CurrentCharacters[i]].gameObject.SetActive(false);
        CurrentCharacters[i] = mod(CurrentCharacters[i] + 1, GameManager.instance.PossibleCharacters.Count);
        images[CurrentCharacters[i]].gameObject.SetActive(true);
    }

    public void DownChoice(int i)
    {
        Image[] images = teamParents[i].GetComponentsInChildren<Image>(true);
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
                hpDisplay.text = GameManager.instance.PlayingPlaceable.CurrentHP.ToString();
                hpBar.fillAmount = GameManager.instance.PlayingPlaceable.CurrentHP / GameManager.instance.PlayingPlaceable.MaxHP;
                movDisplay.text = GameManager.instance.PlayingPlaceable.CurrentPM.ToString();
                paDisplay.text = GameManager.instance.PlayingPlaceable.CurrentPA.ToString();                
            }
            GameObject zoneToUpdate = timelineZone.gameObject;
            Slider[] sliders = zoneToUpdate.GetComponentsInChildren<Slider>();
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].value = GameManager.instance.TurnOrder[i].Character.CurrentHP;
            }
        }
    }

    public void Preview(int damage, int turns, LivingPlaceable target)
    {
        CharacterDisplay[] characters = timelineZone.transform.GetComponentsInChildren<CharacterDisplay>();
        target.DamagePreview(damage);
        foreach(CharacterDisplay character in characters)
        {
            if (character.Character == target)
            {
                character.PreviewEffect(damage, turns);
            }
        }
    }

    public void ResetPreview(LivingPlaceable target)
    {
        CharacterDisplay[] characters = timelineZone.transform.GetComponentsInChildren<CharacterDisplay>();
        target.ResetPreview();
        foreach (CharacterDisplay character in characters)
        {
            if (character.Character == target)
            {
                character.ResetPreview();
            }
        }
    }

    public void SpawnUI()
    {
        teamCanvas.SetActive(false);
        spawnCanvas.SetActive(true);
        int numberInstantiated = 0;
        if (gameObject == GameManager.instance.player1)
        {
            foreach (LivingPlaceable character in GameManager.instance.player1.GetComponent<Player>().Characters)
            {
                Button button = Instantiate(prefabCharacterSpawnButton, spawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-398 + 200 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.characterSprite;
                button.onClick.AddListener(character.ChangeSpawnCharacter);
                numberInstantiated++;
            }
        }
        if (gameObject == GameManager.instance.player2)
        {
            foreach (LivingPlaceable character in GameManager.instance.player2.GetComponent<Player>().Characters)
            {
                Button button = Instantiate(prefabCharacterSpawnButton, spawnZone);
                button.GetComponent<RectTransform>().transform.localPosition = new Vector3(-398 + 200 * numberInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = character.characterSprite;
                button.onClick.AddListener(character.ChangeSpawnCharacter);
                numberInstantiated++;
            }
        }

    }

    public void EndSpawn()
    {
        ClearZone(spawnZone.gameObject);
        spawnCanvas.gameObject.transform.Find("ReadyText").gameObject.SetActive(true);
        spawnCanvas.gameObject.transform.Find("ReadyButton").gameObject.SetActive(false);
        GameManager.instance.CharacterToSpawn = null;
        
    }

    /// <summary>
    /// Determine the max amount of skill possessed by a character and instantiate that many buttons
    /// </summary>
    /// <param name="player"></param>
    public void InitAbilities(Player player)
    {
        abilityButtons = new List<GameObject>();
        int maxNbSkills = 0;
        foreach (LivingPlaceable charac in player.Characters)
        {
            if (maxNbSkills < charac.Skills.Count)
                maxNbSkills = charac.Skills.Count;
        }
        for(int i = 0; i < maxNbSkills; i++)
        {
            GameObject ability = Instantiate(prefabAbilityButton, skillZone);
            ability.transform.localPosition = new Vector3(firstAbility + abilityGap * i, 0);
            abilityButtons.Add(ability);
        }
    }

    public void UpdateAbilities(LivingPlaceable character, Vector3Int position)
    {
        if (character == null)
        {
            return;
        }

        //Give each button the rigth skill
        for (int i = 0; i < abilityButtons.Count; i++)
        {
            if(i < character.Skills.Count)
            {
                abilityButtons[i].SetActive(true);
                abilityButtons[i].GetComponentInChildren<SkillInfo>().Skill = character.Skills[i];
                abilityButtons[i].GetComponentInChildren<SkillInfo>().UpdateButtonInfo();
                abilityButtons[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                abilityButtons[i].GetComponentInChildren<Button>().onClick.AddListener(character.Skills[i].Activate);
                abilityButtons[i].GetComponentInChildren<Button>().onClick.AddListener(SoundHandler.Instance.PlayUISound);
            }
            else
            {
                abilityButtons[i].SetActive(false);
            }
        }
        updateSpecialAbilities(character, position);
    }

    /// <summary>
    /// Check and add "local" skills
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position"></param>
    public void updateSpecialAbilities(LivingPlaceable character, Vector3Int position)
    {
        if (character == null)
        {
            return;
        }

        ClearZone(specialSkillZone.gameObject);
        int specialInstantiated = 0;
        bool anySpecial = false;
        foreach (ObjectOnBloc obj in GameManager.instance.GetObjectsOnBlockUnder(position))
        {
            foreach (Skill skill in obj.GivenSkills)
            {
                specialSkillZone.gameObject.SetActive(true);
                GameObject ability = Instantiate(prefabAbilityButton, specialSkillZone);
                ability.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                Button button = ability.GetComponentInChildren<Button>();
                button.GetComponent<SkillInfo>().Skill = skill;
                ability.GetComponent<RectTransform>().transform.localPosition = new Vector3(firstSpecialAbility + specialAbilityGap * specialInstantiated, 0);
                button.GetComponentInChildren<Image>().sprite = skill.AbilitySprite;
                button.onClick.AddListener(skill.Activate);
                button.onClick.AddListener(SoundHandler.Instance.PlayUISound);
                specialInstantiated++;
                Debug.Log("new skill from objectonbloc");
                anySpecial = true;
            }
            
        }
        if (!anySpecial)
        {
            specialSkillZone.gameObject.SetActive(false);
        }
    }

    public void ActivateLinkUI(Totem totem)
    {
        linkSkillZone.SetActive(true);
        linkSkillZone.GetComponent<LinkDisplayer>().InitializeLink(totem);
    }

    public void UpdateAvailability()
    {
        foreach(GameObject button in abilityButtons)
        {
            SkillInfo skillInfo = button.GetComponentInChildren<SkillInfo>();
            skillInfo.DisplayAvailability();
        }
    }

    public void UpdateTimeline()
    {
        int numberInstantiated = 0;
        foreach (StackAndPlaceable character in GameManager.instance.TurnOrder)
        {
            GameObject image = Instantiate(prefabCharacterImage, timelineZone);
            Image icon = image.transform.Find("CharacterIcon").GetComponent<Image>();
            Image filter = image.transform.Find("Filter").GetComponent<Image>();
            image.GetComponent<CharacterDisplay>().Character = character.Character;
            icon.sprite = character.Character.characterSprite;
            image.GetComponentInChildren<Slider>().maxValue = character.Character.MaxHP;
            image.GetComponentInChildren<Slider>().value = character.Character.CurrentHP;
            if (character.Character.Player.gameObject == gameObject)
            {
                filter.color = new Color(0, 1, 1, 0.7f);
            }
            else
            {
                filter.color = new Color(1, 0, 0, 0.7f);
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

    public void ShowTurnCard()
    {
        ClearZone(timelineZone.gameObject);
        UpdateTimeline();
        playerTurnObjects.SetActive(false);
        StartCoroutine(TurnCard());
    }

    private IEnumerator TurnCard()
    {
        if(GameManager.instance.PlayingPlaceable.Player.gameObject == gameObject)
        {
            yourTurnCard.gameObject.SetActive(true);
            yield return new WaitForSeconds(GameManager.instance.timeBetweenTurns);
            yourTurnCard.gameObject.SetActive(false);
            GameManager.instance.BeginningOfTurn();
        }
        else
        {
            ennemyTurnCard.gameObject.SetActive(true);
            yield return new WaitForSeconds(GameManager.instance.timeBetweenTurns);
            ennemyTurnCard.gameObject.SetActive(false);
            GameManager.instance.BeginningOfTurn();
        }
    }

    public void ChangeTurn()
    {
        if (GameManager.instance.PlayingPlaceable.Player.gameObject == gameObject)
        {
            playerTurnObjects.SetActive(true);
            ClearZone(specialSkillZone.gameObject);
            UpdateAbilities(GameManager.instance.PlayingPlaceable, GameManager.instance.PlayingPlaceable.GetPosition());//WARNING can be messed up with animation and fast change of turn
            
            ClearZone(timelineZone.gameObject);
            UpdateTimeline();

            if (GameManager.instance.isClient)
            {
                GameManager.instance.GetLocalPlayer().gameObject.GetComponentInChildren<Canvas>().gameObject.transform.Find("StatsDisplayer")
                    .GetComponent<StatDisplayer>().Activate(GameManager.instance.PlayingPlaceable);
            }
        }
    }

    public void ResetEndTurn()
    {
        gameObject.GetComponentInChildren<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Deactivate();
        if(GameManager.instance.State == States.Link)
        {
            linkSkillZone.GetComponent<LinkDisplayer>().BreakLink();
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

    public void DisplayCameraMode(int mode)
    {
        if (GameManager.instance.isGameStarted)
        {
            switch (mode)
            {
                case 1:
                    camModeDisplay.sprite = worldCam;
                    camModeDisplay.gameObject.GetComponent<FadingUI>().StartPulse();
                    break;
                case 0:
                    camModeDisplay.sprite = charaCam;
                    camModeDisplay.gameObject.GetComponent<FadingUI>().StartPulse();
                    break;
            }
        } 
    }
}
