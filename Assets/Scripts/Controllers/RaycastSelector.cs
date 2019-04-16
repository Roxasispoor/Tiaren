using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RaycastSelector : MonoBehaviour
{
    public LayerMask layerMask = 0;
    
    new public Camera camera;

    private SelectionInfo currentHovered = new SelectionInfo();
    
    public SelectionInfo CurrentHovered
    {
        get
        {
            return currentHovered;
        }
    }
    /*
        set
        {
            
            if (currentHovered.placeable != null)
            {
                Placeable placeable = currentHovered.placeable;
                if (GameManager.instance.State == States.UseSkill && GameManager.instance.ActiveSkill != null)
                {
                    GameManager.instance.ActiveSkill.UnPreview(placeable);
                }
                placeable.UnHighlight();
            }

            if (value.placeable != null)
            {
                Placeable placeable = value.placeable;
                placeable.Highlight();
                if (GameManager.instance.State == States.UseSkill && GameManager.instance.PlayingPlaceable.IsPlaceableInTarget(placeable))
                {
                    GameManager.instance.ActiveSkill.Preview(placeable);
                }
            }
            currentHovered = value;
        }
    }*/

    // Use this for initialization
    void Start()
    {
        camera = GetComponent<Camera>();
        layerMask = ~layerMask;
    }

    private void OnEnable()
    {
        GameManager.instance.RaycastSelector = this;
    }

    // Update is called once per frame
    void Update()
    {
        //deactivate input during change of turn
        if (GameManager.instance.State == States.TurnChange)
        {
            return;
        }

        if (GameManager.instance.State == States.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentHovered.orientationState = (currentHovered.orientationState == 3 ? 0 : currentHovered.orientationState + 1);
            //Debug.Log(state);
        }

        if (GameManager.instance.isGameStarted || GameManager.instance.State == States.Spawn )
        {

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
            {
                Placeable placeableHitted = hit.transform.GetComponent<Placeable>();
                Vector3 faceHitted = hit.normal;

                currentHovered.face = faceHitted;
                if (placeableHitted != currentHovered.placeable)
                {
                    UnHighligthAndUnPreviewCurrent();
                    GameManager.instance.Hovered = placeableHitted;
                    HighligthAndPreviewCurrent();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    GameManager.instance.ClickOnPlaceable(placeableHitted);
                }
            }
            else
            {
                if (GameManager.instance.Hovered != null)
                {
                    currentHovered = new SelectionInfo();
                }
            }
        }
    }

    /// <summary>
    /// Unhighlight the currently placeable considered as hovered.
    /// </summary>
    private void UnHighligthAndUnPreviewCurrent()
    {
        if (currentHovered.placeable != null)
        {
            Placeable placeable = currentHovered.placeable;
            if (GameManager.instance.State == States.UseSkill && GameManager.instance.ActiveSkill != null)
            {
                GameManager.instance.ActiveSkill.UnPreview(placeable);
            }
            placeable.UnHighlight();
        }
    }

    /// <summary>
    /// Highlight the currently placeable considered as hovered.
    /// </summary>
    private void HighligthAndPreviewCurrent()
    {
        if (currentHovered.placeable != null)
        {
            Placeable placeable = currentHovered.placeable;
            placeable.Highlight();
            if (GameManager.instance.State == States.UseSkill && GameManager.instance.PlayingPlaceable.IsPlaceableInTarget(placeable))
            {
                GameManager.instance.ActiveSkill.Preview(placeable);
            }
        }
    }
}
