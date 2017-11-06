using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yieldTest : MonoBehaviour {
    private bool isRunning;
    private Timer clock;

    public yieldTest()
    {

    }
    // Use this for initialization
    IEnumerator Start () {
        int i = 0;
        clock = gameObject.AddComponent<Timer>();
           clock.StartTimer(20.0f);

        while (!clock.IsFinished)
        {
            i++;
            print("c'est parti");
            yield return StartCoroutine(getInput(KeyCode.Space));
            print("il a appuyé!"+i);
            yield return StartCoroutine(Waiting());
           
        }
        print("fin du tour");
    }
    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(3.0f);
    }
	IEnumerator getInput(KeyCode keyCode)
    {
        
     
        while (!Input.anyKeyDown)
        { 
            yield return null;
           
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
