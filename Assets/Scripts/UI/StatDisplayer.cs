using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayer : MonoBehaviour {

    public Text className;
    public Text player;
    public Text team;
    public Text hp;
    public Text pa;
    public Text mov;
    public Text speed;
    public Text def;
    public Text mDef;
    public Image image;

    public void Activate(LivingPlaceable character)
    {
        gameObject.SetActive(true);
        className.text = "Name : " + character.ClassName;
        player.text = "Player : xXDarkSasukeXx";
        team.text = "Team : " + character.Player.name;
        hp.text = "HP : " + character.CurrentHP;
        mov.text = "MOV : " + character.CurrentPM;
        pa.text = "PA : " + character.CurrentPA;
        speed.text = "SPEED : " + (int) character.Speed;
        def.text = "DEF : " + character.Def;
        mDef.text = "M.DEF : " + character.Mdef;
        image.sprite = character.characterSprite; 
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
