using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Skill skill;

    public Skill Skill
    {
        get
        {
            return skill;
        }

        set
        {
            skill = value;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.State != States.TeamSelect && GameManager.instance.State != States.Spawn)
        {
            gameObject.GetComponentInParent<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Activate(Skill);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponentInParent<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Deactivate();
    }

}
