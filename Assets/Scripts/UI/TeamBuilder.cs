using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuilder : MonoBehaviour
{
    enum States { TeamSelect, TeamModify };
    private States state = States.TeamSelect;

    public GameObject titleCard;
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

    public GameObject CharacterInfoDisplay;

    public Dictionary<SkillTypes, string> menuSkills;


    //PlayerPref variables

    private void Awake()
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
                string teamName = GetStringPref("TEAM_" + i + "_NAME");
                CharacterMenuInfo[] characters = new CharacterMenuInfo[3];
                for(int j = 0; j<3; j++)
                {
                    characters[j] = PossibleCharacters[GetIntPref("TEAM_" + i + "_Player_" + j)];
                }
                teams[i].UpdateInfo(teamName, characters);
            }
        }
    }

    //Switch to modification of a team
    public void ActivateModifyTeam(TeamInfo team)
    {
        titleCard.SetActive(false);
        activeTeam = team;
        foreach(TeamInfo teamTest in teams)
        {
            if(team != teamTest)
            {
                teamTest.gameObject.SetActive(false);
            }
        }
        team.ActivateModifyUI();
    }

    public void DeactivateModifyTeam(TeamInfo team)
    {
        titleCard.SetActive(true);
        activeTeam = null;
        foreach (TeamInfo teamTest in teams)
        {
            teamTest.gameObject.SetActive(true);
        }
        team.DeactivateModifyUI();
    }

    //Goes back to base team builer ui and update info
    public void ResetModify()
    {
        UpdateDisplay();
    }

    //save info of current active team to player pref
    public void SaveInfo()
    {
        int index = teams.FindIndex(activeTeam.Equals);
        SetPref("TEAM_" + index + "_NAME", activeTeam.TeamName);
        for (int i = 0; i < activeTeam.ChosenCharacters.Count; i++)
        {
            SetPref("TEAM_" + index + "_Player_" + i, PossibleCharacters.FindIndex(activeTeam.ChosenCharacters[i].Equals));
        }
    }

    //display the info of the currently selected character
    public void DisplayCharacterInfo(CharacterMenuInfo character)
    {
        if(state == States.TeamSelect)
        {
            return;
        }
        className.text = character.characterStat.className;
        hpDisplay.text = "HP : " + character.characterStat.maxHP;
        apDisplay.text = "AP : " + character.characterStat.paMax;
        jumpDisplay.text = "JUMP : " + character.characterStat.jump;
        speedDisplay.text = "Speed : " + character.characterStat.speed;
        movDisplay.text = "MOV : " + character.characterStat.pmMax;
        defDisplay.text = "Def : " + character.characterStat.def;
        mdefDisplay.text = "M.Def : " + character.characterStat.mdef;
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
        className.text = skill.name;
        cooldownDisplay.text = "Cooldown : " + skill.cooldown;
        costDisplay.text = "Cost : " + skill.cooldown;
        targetDescription.text = "Targets : " + skill.targets;
        minRangeDisplay.text = "Min Range : " + skill.minRange;
        maxRangeDisplay.text = "Max Range    : " + skill.maxRange;
        skillDescription.text = skill.description;

        if(skill.power != 0)
        {
            powerDisplay.text = "Power : " + skill.power;
        }

        shape.sprite = skill.targetSprite;
        skillImage.sprite = skill.skillSprite;
    }


    public void ResetSkillInfo()
    {
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
