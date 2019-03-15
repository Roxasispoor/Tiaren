using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplayer : MonoBehaviour {

    public TMP_Text className;
    public TMP_Text player;
    public TMP_Text hp;
    public TMP_Text pa;
    public TMP_Text mov;
    public TMP_Text speed;
    public TMP_Text def;
    public TMP_Text mDef;
    public Image sprite;
    public Image Filter;

    public void Activate(LivingPlaceable character)
    {
        gameObject.SetActive(true);
        className.text = character.ClassName;
        player.text = "Player : " + character.Player.name;
        hp.text = "HP : " + character.CurrentHP;
        mov.text = "MOV : " + character.CurrentPM;
        pa.text = "PA : " + character.CurrentPA;
        speed.text = "SPEED : " + (int) character.Speed;
        def.text = "DEF : " + character.Def;
        mDef.text = "M.DEF : " + character.Mdef;
        sprite.sprite = character.characterSprite;
        Filter.color = character.Player.color;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Preview(LivingPlaceable character, int damage)
    {
        Activate(character);
        hp.text = "HP : " + character.CurrentHP + "<color=#ff0000ff> - " + damage;
    }
}
