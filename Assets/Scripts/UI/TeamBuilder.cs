using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuilder : MonoBehaviour
{
    enum States { TeamSelect, TeamModify };
    private States state = States.TeamSelect;

    public GameObject prefabTeamInfo;
    public Vector3 firstPosition;

    public GameObject addTeamButton;
    public Vector3 teamGap;

    public GameObject titleCard;
    public FadingUI teamNameWarning;

    public List<TeamInfo> teams;
    private TeamInfo activeTeam;
    private CharacterMenuInfo activeCharacter;

    //Character display related
    public TMP_Text className;
    public TMP_Text hpDisplay;
    public TMP_Text apDisplay;
    public TMP_Text jumpDisplay;
    public TMP_Text speedDisplay;
    public TMP_Text defDisplay;
    public TMP_Text mdefDisplay;
    public TMP_Text movDisplay;
    public TMP_Text characterDescription;
    public Image characterImage;

    //skill display related
    public TMP_Text skillName;
    public TMP_Text costDisplay;
    public TMP_Text cooldownDisplay;
    public TMP_Text targetDescription;
    public TMP_Text minRangeDisplay;
    public TMP_Text maxRangeDisplay;
    public TMP_Text powerDisplay;
    public TMP_Text skillDescription;
    public Image skillImage;
    public Image shape;

    public List<CharacterMenuInfo> PossibleCharacters;

    public GameObject characterInfoDisplay;
    public GameObject skillInfoDisplay;
    public GameObject possibleCharactersDisplay;
    public GameObject cancelButton;
    public GameObject saveButton;
    public GameObject returnButton;

    public Dictionary<SkillTypes, string> menuSkills;



    //PlayerPref variables

    private void Start()
    {
        menuSkills = new Dictionary<SkillTypes, string>();
        menuSkills.Add(SkillTypes.PUSH, "PushSkill");
        menuSkills.Add(SkillTypes.CREATE, "CreateSkill");
        menuSkills.Add(SkillTypes.DESTROY, "DestroySkill");
        menuSkills.Add(SkillTypes.SWORDATTACK, "SwordAttack");
        menuSkills.Add(SkillTypes.BLEEDING, "BleedingAttack");
        menuSkills.Add(SkillTypes.LEGSWIPE, "LegSwipe");
        menuSkills.Add(SkillTypes.SPINNINGATTACK, "SpinningAttack");
        menuSkills.Add(SkillTypes.TACKLE, "Tackle");
        menuSkills.Add(SkillTypes.BOWSHOT, "BowShot");
        menuSkills.Add(SkillTypes.PIERCINGSHOT, "PiercingShot");
        menuSkills.Add(SkillTypes.HIGHGROUND, "HighGround");
        menuSkills.Add(SkillTypes.ZIPLINE, "CreateZiplineSkill");
        menuSkills.Add(SkillTypes.ACROBATIC, "AcrobaticSkill");
        menuSkills.Add(SkillTypes.MAGICMISSILE, "MagicMissile");
        menuSkills.Add(SkillTypes.FISSURE, "Fissure");
        menuSkills.Add(SkillTypes.FIREBALL, "Fireball");
        menuSkills.Add(SkillTypes.WALL, "Wall");
        menuSkills.Add(SkillTypes.EARTHBENDING, "EarthBending");
        menuSkills.Add(SkillTypes.REPULSIVEGRENADE, "RepulsiveGrenade");
        menuSkills.Add(SkillTypes.GRAPPLE, "GrappleHook");
        menuSkills.Add(SkillTypes.MAKIBISHI, "MakibishiSkill");
        menuSkills.Add(SkillTypes.CREATETOTEMHP, "CreateTotemHP");
        menuSkills.Add(SkillTypes.CREATETOTEMAP, "CreateTotemAP");
        menuSkills.Add(SkillTypes.TOTEMDESTROY, "DestroyTotem");
        menuSkills.Add(SkillTypes.TOTEMLINK, "TotemLinkSkill");
        menuSkills.Add(SkillTypes.TOTEMHEAL, "TotemHealSkill");
        menuSkills.Add(SkillTypes.TOTEMEXPLOSION, "TotemExplosionSkill");
        
        List<string> characterNames = new List<string>();
        characterNames.Add("Knight");
        characterNames.Add("Ranger");
        characterNames.Add("Mage");
        characterNames.Add("Ninja");
        characterNames.Add("Shaman");

        for(int i = 0; i < PossibleCharacters.Count; i ++)
        {
            PossibleCharacters[i].Initialize(characterNames[i]);
        }

        UpdateTeamInfo();
    }


    //Modifies the info shown on the team select screen
    private void UpdateDisplay()
    {
        foreach(TeamInfo team in teams)
        {
            team.UpdateDisplay();
        }
    }

    //Modifies the data in the team info components from the prefs
    private void UpdateTeamInfo()
    {
        for(int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.HasKey("TEAM_" + i + "_NAME")){
                GameObject teamObject = Instantiate(prefabTeamInfo, gameObject.transform);
                teamObject.transform.localPosition = firstPosition - teamGap * i;
                teams.Add(teamObject.GetComponent<TeamInfo>());
                string teamName = GetStringPref("TEAM_" + i + "_NAME");
                int character;
                CharacterMenuInfo[] characters = new CharacterMenuInfo[3];
                for(int j = 0; j<3; j++)
                {
                    character = GetIntPref("TEAM_" + i + "_Player_" + j);
                    if(character != -1)
                    {
                        characters[j] = PossibleCharacters[GetIntPref("TEAM_" + i + "_Player_" + j)];
                    }
                }
                teams[i].UpdateInfo(teamName, characters);
                teams[i].teamNumber = i;
                addTeamButton.transform.localPosition = firstPosition - teamGap * i - new Vector3(680, 100, 0);
            }
        }
        if (teams.Count >= 5)
        {
            addTeamButton.SetActive(false);
        }
        else if(teams.Count == 0)
        {
            AddNewTeam();
        }
    }

    //Switch to modification of a team
    public void ActivateModifyTeam(TeamInfo team)
    {
        titleCard.SetActive(false);
        addTeamButton.SetActive(false);
        activeTeam = team;
        state = States.TeamModify;
        returnButton.SetActive(false);
        foreach(TeamInfo teamTest in teams)
        {
            if(team != teamTest)
            {
                teamTest.gameObject.SetActive(false);
            }
        }
    }

    public void ShowCharacters(TeamInfo team)
    {
        possibleCharactersDisplay.SetActive(true);
        if (team.chosenCharacters[0] != null)
        {
            ActivateCharacter(team.chosenCharacters[0]);
        }
        else
        {
            activeCharacter = PossibleCharacters[0];
            PossibleCharacters[0].toggle.isOn = true;
        }
    }

    public void AddNewTeam()
    {
        if (teams.Count < 5)
        {
            GameObject teamObject = Instantiate(prefabTeamInfo, gameObject.transform);
            teamObject.transform.localPosition = firstPosition - teamGap * teams.Count;
            teams.Add(teamObject.GetComponent<TeamInfo>());
            teams[teams.Count - 1].teamNumber = teams.Count - 1;
            addTeamButton.transform.localPosition = firstPosition - teamGap * (teams.Count - 1) - new Vector3(680, 100, 0);
            SaveToPlayerPref(teams[teams.Count - 1]);
        }
        if(teams.Count >= 5)
        {
            addTeamButton.SetActive(false);
        }
    }

    public void RemoveTeam(TeamInfo team)
    {
        int index = team.teamNumber;
        RemoveTeamPref(index);
        teams.Remove(team);
        Destroy(team.gameObject);
        for(int i = index; i < teams.Count; i++)
        {
            teams[i].teamNumber = i;
            teams[i].gameObject.transform.localPosition = firstPosition - teamGap * i;
            teams[i].UpdatePositionInfo();
        }
        addTeamButton.transform.localPosition = firstPosition - teamGap * (teams.Count-1) - new Vector3(680, 100, 0);
    }

    private void RemoveTeamPref(int teamToRemove)
    {
        for (int i = teamToRemove; i < teams.Count - 1; i++)
        {
            int nextElem = i + 1;
            SetPref("TEAM_" + i + "_NAME", GetStringPref("TEAM_" + nextElem + "_NAME"));
            for (int j = 0; j < teams[i].chosenCharacters.Length; j++)
            {
                SetPref("TEAM_" + i + "_Player_" + j, GetIntPref("TEAM_" + nextElem + "_Player_" + j));
            }
        }
        int lastElem = teams.Count - 1;
        PlayerPrefs.DeleteKey("TEAM_" + lastElem + "_NAME");
        for (int j = 0; j < teams[lastElem].chosenCharacters.Length; j++)
        {
            PlayerPrefs.DeleteKey("TEAM_" + lastElem + "_Player_" + j);
        }
    }

    private void ActivateCharacter(CharacterMenuInfo character)
    {
        int actualCharacter = PossibleCharacters.FindIndex(character.Equals);
        activeCharacter = PossibleCharacters[actualCharacter];
        PossibleCharacters[actualCharacter].toggle.isOn = true;

        cancelButton.SetActive(true);
        saveButton.SetActive(true);
    }

    public void DeactivateModifyTeam()
    {
        state = States.TeamSelect;
        cancelButton.SetActive(false);
        saveButton.SetActive(false);
        returnButton.SetActive(true);
        titleCard.SetActive(true);
        activeCharacter.skillDisplay.SetActive(false);
        activeCharacter.toggle.group.SetAllTogglesOff();
        activeCharacter = null;
        possibleCharactersDisplay.SetActive(false);
        characterInfoDisplay.SetActive(false);
        activeTeam.DeactivateModifyUI();
        activeTeam = null;
        if(teams.Count < 5)
        {
            addTeamButton.SetActive(true);
        }
    }

    public void ActivateTeams()
    {
        foreach (TeamInfo teamTest in teams)
        {
            teamTest.gameObject.SetActive(true);
        }
    }

    public void CancelModification()
    {
        activeTeam.ResetModification();
        DeactivateModifyTeam();
    }

    public void ModifyActiveTeamInfo()
    {
        if(state == States.TeamSelect)
        {
            return;
        }
        activeTeam.SetCharacterForSlot(activeCharacter);
    }

    /// <summary>
    /// Save Team to Player pref
    /// </summary>
    /// <param name="team"></param>
    public void SaveToPlayerPref(TeamInfo team)
    {
        int index = teams.FindIndex(team.Equals);
        if(null != team.TeamName)
        {
            SetPref("TEAM_" + index + "_NAME", team.TeamName);
        }
        else
        {
            team.TeamName = "NewTeam " + index;
            SetPref("TEAM_" + index + "_NAME", "NewTeam " + index);
        }
        for (int i = 0; i < team.chosenCharacters.Length; i++)
        {
            if (null != team.chosenCharacters[i])
            {
                SetPref("TEAM_" + index + "_Player_" + i, PossibleCharacters.FindIndex(team.chosenCharacters[i].Equals));
            }
            else
            {
                SetPref("TEAM_" + index + "_Player_" + i, -1);
            }
        }
        Debug.Log("Team : " + GetStringPref("TEAM_" + index + "_NAME").ToString());
        
    }

    /// <summary>
    /// Called to save from button when in modify
    /// </summary>
    public void SaveInfo()
    {
        SaveToPlayerPref(activeTeam);
        activeTeam.UpdateDisplay();
        DeactivateModifyTeam();
    }

    //display the info of the currently selected character
    public void DisplayCharacterInfo(CharacterMenuInfo character)
    {
        if(state == States.TeamSelect)
        {
            return;
        }
        if(character != activeCharacter && activeCharacter != null)
        {
            activeCharacter.DeactivateInfoDisplay();
        }
        character.ActivateInfoDisplay();
        characterInfoDisplay.SetActive(true);
        className.text = character.characterStat.className;
        hpDisplay.text = "HP : " + character.characterStat.maxHP.ToString();
        apDisplay.text = "AP : " + character.characterStat.paMax.ToString();
        jumpDisplay.text = "JUMP : " + character.characterStat.jump.ToString();
        speedDisplay.text = "Speed : " + character.characterStat.speed.ToString();
        movDisplay.text = "MOV : " + character.characterStat.pmMax.ToString();
        defDisplay.text = "Def : " + character.characterStat.def.ToString();
        mdefDisplay.text = "M.Def : " + character.characterStat.mdef.ToString();
        characterDescription.text = character.description;
        characterImage.sprite = character.characterSprite;
        activeCharacter = character;
    }

    //display the info of the currently 
    public void DisplaySkillInfo(SkillMenuInfo skill)
    {
        if (state == States.TeamSelect)
        {
            return;
        }
        characterInfoDisplay.SetActive(false);
        skillInfoDisplay.SetActive(true);
        skillName.text = skill.skillName;
        cooldownDisplay.text = "Cooldown : " + skill.cooldown;
        costDisplay.text = "Cost : " + skill.cooldown;
        targetDescription.text = "Targets : " + skill.targets;
        minRangeDisplay.text = "Min Range : " + skill.minRange;
        maxRangeDisplay.text = "Max Range    : " + skill.maxRange;
        skillDescription.text = skill.description;

        if(skill.power != 0)
        {
            powerDisplay.gameObject.SetActive(true);
            powerDisplay.text = "Power : " + skill.power;
        }
        else
        {
            powerDisplay.gameObject.SetActive(false);
        }

        shape.sprite = skill.targetSprite;
        skillImage.sprite = skill.skillSprite;
    }


    public void ResetSkillInfo()
    {
        skillInfoDisplay.SetActive(false);
        DisplayCharacterInfo(activeCharacter);
    }


    #region PlayerPref setters

    private void SetPref(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    private void SetPref(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    private string GetStringPref(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    private int GetIntPref(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    #endregion

}
