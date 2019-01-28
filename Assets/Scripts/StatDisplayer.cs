using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayer : MonoBehaviour {

    public Text className;
    public Text player;
    public Text team;
    public Text hp;
    public Text mov;
    public Text speed;
    public Text dext;
    public Text def;
    public Text mDef;
    public Text str;
    public Text mStr;
    public Image image;

    public void Activate(LivingPlaceable character)
    {
        gameObject.SetActive(true);
        className.text = "Name : " + character.ClassName;
        player.text = "Player : xXDarkSasukeXx";
        team.text = "Team : " + character.Player.name;
        hp.text = "HP : " + character.CurrentHP;
        mov.text = "MOV : " + character.CurrentPM;
        speed.text = "SPEED : " + character.Speed;
        dext.text = "DEXT : " + character.Dexterity;
        def.text = "DEF : " + character.Def;
        mDef.text = "M.DEF : " + character.Mdef;
        str.text = "STR : " + character.Force;
        mStr.text = "M.STR : " + character.Mstr;
        image.sprite = character.characterSprite; 
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
