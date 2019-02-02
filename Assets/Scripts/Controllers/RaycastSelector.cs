using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            state = (state == 3 ? 0 : state+1);
            //Debug.Log(state);
        }

        if (GameManager.instance.playingPlaceable != null || GameManager.instance.State == States.Spawn )
        {

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, layerMask))
            {
                if (hit.transform.GetComponent<Placeable>() != null)
                {
                    hit.transform.GetComponent<Placeable>().OnMouseOverWithLayer();

                    if (GameManager.instance.Hovered != null)
                    {
                        if (area == null)
                            GameManager.instance.Hovered.UnHighlight();
                        else
                        {
                            foreach (Placeable block in area)
                                block.UnHighlight();
                        }

                    }

                    GameManager.instance.Hovered = hit.transform.GetComponent<Placeable>();

                    if (effectarea==0)
                    {
                        area = null;
   
                        if (GameManager.instance.Hovered != null)
                        {
                            GameManager.instance.Hovered.Highlight();
                        }
                    }
                    else
                    {
                        if (pattern == SkillArea.MIXEDAREA)
                            topblock = false;
                        else topblock = true;

                        if (pattern == SkillArea.NONE || pattern == SkillArea.MIXEDAREA)
                            area = Grid.instance.HighlightEffectArea(hit.transform.GetComponent<Placeable>(), effectarea, topblock);
                        else
                            area = Grid.instance.HighlightEffectArea(hit.transform.GetComponent<Placeable>(), effectarea, topblock, state, pattern);

                        if (pattern == SkillArea.MIXEDAREA)
                        {
                            List<LivingPlaceable> Targets = new List<LivingPlaceable>();
                            foreach (Placeable block in area)
                            {
                                block.Highlight();
                                Vector3 Pos = block.transform.position;
                                if (Pos.y + 1 < Grid.instance.sizeY && Grid.instance.GridMatrix[(int)Pos.x, (int)Pos.y + 1, (int)Pos.z] != null && Grid.instance.GridMatrix[(int)Pos.x, (int)Pos.y + 1, (int)Pos.z].IsLiving())
                                    Targets.Add((LivingPlaceable)Grid.instance.GridMatrix[(int)Pos.x, (int)Pos.y + 1, (int)Pos.z]);
                            }
                            GameManager.instance.PlayingPlaceable.TargetableUnits = Targets;
                        }
                        else foreach (Placeable block in area) block.Highlight();
                    }
                }
            } else
            {
                GameManager.instance.Hovered = null;
            }
        }
    }
}
