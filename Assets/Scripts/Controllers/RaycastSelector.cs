using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastSelector : MonoBehaviour
{
    public LayerMask layerMask = 0;
    private int effectarea; //effectarea radius
    private bool topblock = true;
    private int state;
    private SkillArea pattern;
    private List<Placeable> area;
    new public Camera camera;

    public SkillArea Pattern
    {get {return pattern;} set {pattern = value;}}

    public int State
    { get { return state; } set { state = value; } }

    public int EffectArea
    { get { return effectarea; } set { effectarea = value; } }

    public List<Placeable> Area
    { get { return area; } set { area = value; } }

    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
        layerMask = ~layerMask;
        state = 0;
        pattern = SkillArea.NONE;
        effectarea = 0;
    }

    private void OnEnable()
    {
        GameManager.instance.RaycastSelector = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            state = (state == 3 ? 0 : state+1);
            //Debug.Log(state);
        }

        if (GameManager .instance.isGameStarted || GameManager.instance.State == States.Spawn )
        {

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (hit.transform.GetComponent<Placeable>() != null && hit.transform.GetComponent<Placeable>() != GameManager.instance.Hovered)
                {
                    if (GameManager.instance.Hovered != null)
                    {
                        if (area != null)
                        {
                            foreach (Placeable block in area)
                            {
                                if (block == null)
                                    continue;
                                block.UnHighlight();
                                if (GameManager.instance.ActiveSkill != null)
                                {
                                    foreach (Effect effect in GameManager.instance.ActiveSkill.effects)
                                    {
                                        effect.Preview(block);
                                    }
                                }
                            }
                        }
                    }

                    GameManager.instance.Hovered = hit.transform.GetComponent<Placeable>();
                    
                    if (GameManager.instance.State == States.UseSkill)
                    {
                        area = GameManager.instance.ActiveSkill.patternUse(GameManager.instance.Hovered);
                        foreach (Placeable block in area)
                        {
                            if (block == GameManager.instance.Hovered)
                                continue;
                            block.Highlight();
                            if (GameManager.instance.ActiveSkill != null)
                            {
                                foreach (Effect effect in GameManager.instance.ActiveSkill.effects)
                                {
                                    effect.Preview(block);
                                }
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    GameManager.instance.ClickOnPlaceable(hit.transform.GetComponent<Placeable>());
                }
            }
            else
            {
                if (GameManager.instance.Hovered != null)
                {
                    GameManager.instance.Hovered = null;
                }
            }
        }
    }
}
