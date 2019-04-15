using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RaycastSelector : MonoBehaviour
{
    public LayerMask layerMask = 0;

    private bool topblock = true;
    private int state;
    private List<Placeable> area;
    new public Camera camera;

    public List<Placeable> Area
    { get { return area; } set { area = value; } }

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
            GameManager.instance.orientationState = (GameManager.instance.orientationState == 3 ? 0 : GameManager.instance.orientationState + 1);
            //Debug.Log(state);
        }

        if (GameManager .instance.isGameStarted || GameManager.instance.State == States.Spawn )
        {

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
            {
                Placeable placeableHitted = hit.transform.GetComponent<Placeable>();

                if (placeableHitted != null && placeableHitted != GameManager.instance.Hovered)
                {
                    GameManager.instance.Hovered = placeableHitted;
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
                    GameManager.instance.Hovered = null;
                }
            }
        }
    }
}
