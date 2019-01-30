using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static AnimationHandler m_Instance = null;
    public static AnimationHandler Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("AnimationHandler")).AddComponent<AnimationHandler>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    public IEnumerator WaitAndCreateBlock(GameObject go, Vector3Int position, float time)
    {
        Grid.instance.InstantiateCube(go, position);
        Grid.instance.GetPlaceableFromVector(position).gameObject.SetActive(false);
        Debug.Log("Cube has been instantiated, now waiting");
        yield return new WaitForSeconds(time);
        Grid.instance.GetPlaceableFromVector(position).gameObject.SetActive(true);
    }

    public IEnumerator WaitAndDestroyBlock(Placeable go, float time)
    {
        yield return new WaitForSeconds(time);
        go.Destroy();
    }

    public IEnumerator WaitAndPushBlock(Placeable Target, List <Vector3>  path, float speed, float time,bool justLerp=false)
    {
        Debug.Log("Coroutine move starting");
        yield return new WaitForSeconds(time/2);
        GameManager.instance.playingPlaceable.Player.StartMoveAlongBezier(path, Target, speed,justLerp);

    }
    

    public IEnumerator WaitAndDamageIsDone()
    {
        yield return null;
    }

}
