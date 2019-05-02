using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour{

    public GameObject baseMenu;
    public GameObject tutorial;
    public GameObject teamBuilder;
    public GameObject credits;
    public Button quit;
    public List<Sprite> slides;
    public Image PrefabImageTuto;
    public int slideNumber = 0;
    public List<string> descriptions;
    public Text explaination;
    public string path;

    private void Awake()
    {
        descriptions = new List<string>();
        StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            descriptions.Add(line);
        }
        reader.Close();
        quit.onClick.AddListener(Application.Quit);

        SoundHandler.Instance.StartMenuMusic();
    }

    public void ToggleTuto()
    {
        baseMenu.SetActive(false);
        tutorial.SetActive(true);
        PrefabImageTuto.sprite = slides[slideNumber];
        explaination.text = descriptions[slideNumber];
    }

    public void ToggleCredits()
    {
        baseMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void ToggleTeamBuilder()
    {
        baseMenu.SetActive(false);
        teamBuilder.SetActive(true);
    }

    public void BackToBase()
    {
        tutorial.SetActive(false);
        credits.SetActive(false);
        teamBuilder.SetActive(false);
        baseMenu.SetActive(true);
    }

    public void NextImage()
    {
        if (slideNumber == slides.Count - 1)
        {
            slideNumber = 0;
        }
        else
        {
            slideNumber++;
        }
        PrefabImageTuto.sprite = slides[slideNumber];
        explaination.text = descriptions[slideNumber];
    }

    public void PreviousImage()
    {
        if (slideNumber == 0)
        {
            slideNumber = slides.Count - 1;
        }
        else
        {
            slideNumber--;
        }
        PrefabImageTuto.sprite = slides[slideNumber];
        explaination.text = descriptions[slideNumber];
    }
}
