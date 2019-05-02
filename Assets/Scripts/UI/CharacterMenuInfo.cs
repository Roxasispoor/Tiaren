using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuInfo : MonoBehaviour
{

    public Stats characterStat;
    public Sprite characterSprite;
    public string description;
    List<SkillMenuInfo> skillsInfos = new List<SkillMenuInfo>();

    public Image toggle;

    public void Initialize(string className)
    {
        StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, className+".json"));
        string JSON = reader.ReadToEnd();
        JObject deserializedCharac = JObject.Parse(JSON);
        characterStat.className = (string)deserializedCharac["className"];
        characterStat.maxHP.BaseValue = (float)deserializedCharac["maxHP"]["baseValue"];
        characterStat.paMax.BaseValue = (float)deserializedCharac["paMax"]["baseValue"];
        characterStat.pmMax.BaseValue = (float)deserializedCharac["pmMax"]["baseValue"];
        characterStat.jump.BaseValue = (float)deserializedCharac["jump"]["baseValue"];
        characterStat.speed.BaseValue = (float)deserializedCharac["speed"]["baseValue"];
        characterStat.def.BaseValue = (float)deserializedCharac["def"]["baseValue"];
        characterStat.mdef.BaseValue = (float)deserializedCharac["mdef"]["baseValue"];
        description = (string)deserializedCharac["MenuDescription"];
        characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + characterStat.className);

        List<JToken> jTokens = deserializedCharac["ChosenSkills"].Children().ToList();
        StreamReader skillReader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "Skills.json"));
        string skillJson = skillReader.ReadToEnd();
        foreach (JToken jToken in jTokens)
        {
            SkillTypes type = jToken.ToObject<SkillTypes>();
            string typeName;
            if (GetComponentInParent<TeamBuilder>().menuSkills.TryGetValue(type, out typeName))
            {
                skillsInfos.Add(new SkillMenuInfo(skillJson, typeName));
            }
        }
        toggle.sprite = characterSprite;
    }

    public void displayInformation()
    {
        GetComponentInParent<TeamBuilder>().DisplayCharacterInfo(this);
    }
}
