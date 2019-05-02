using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillMenuInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string skillName;
    public int cooldown;
    public int cost;
    public Sprite skillSprite;
    public int maxRange;
    public int minRange;
    public string description;
    public string targetShape;
    public Sprite targetSprite;
    public string targets;
    public int power = 0;

    public SkillMenuInfo(string JSON, string name)
    {
        JObject deserializedSkillInfo = JObject.Parse(JSON);
        Initialize(deserializedSkillInfo, name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<TeamBuilder>().DisplaySkillInfo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInParent<TeamBuilder>().ResetSkillInfo();
    }

    private void Initialize(JObject deserializedSkillInfo, string name)
    {
        skillName = (string)deserializedSkillInfo[name]["skillName"];
        cooldown = (int)deserializedSkillInfo[name]["cooldown"];
        cost = (int)deserializedSkillInfo[name]["cost"];
        string spritePath = (string)deserializedSkillInfo[name]["skillName"];
        skillSprite = Resources.Load<Sprite>("UI_Images/Abilities/" + spritePath);
        maxRange = (int)deserializedSkillInfo[name]["maxRange"];
        minRange = (int)deserializedSkillInfo[name]["minRange"];
        description = (string)deserializedSkillInfo[name]["menuDescription"];
        targetShape = (string)deserializedSkillInfo[name]["targetShape"];
        targetSprite = Resources.Load<Sprite>("UI_Images/Abilities/Shapes/" + targetShape);
        targets = (string)deserializedSkillInfo[name]["targets"];
        power = (int?)deserializedSkillInfo[name]["power"] ?? 0;
    }
}