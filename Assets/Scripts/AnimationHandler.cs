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
        yield return new WaitForSeconds(time);
        Grid.instance.InstantiateCube(go, position);
    }

    public IEnumerator WaitAndDestroyBlock(Placeable go, float time)
    {
        yield return new WaitForSeconds(time);
        go.Destroy();
    }

    public IEnumerator WaitAndPushBlock(Placeable Target, List <Vector3>  path, float speed, float time)
    {
        yield return new WaitForSeconds(time/2);
        GameManager.instance.playingPlaceable.Player.StartMoveAlongBezier(path, Target, speed);

    }

    public IEnumerator WaitAndDamageIsDone()
    {
        yield return null;
    }

}
