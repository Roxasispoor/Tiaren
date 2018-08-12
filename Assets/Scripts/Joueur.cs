using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Représente un joueur 
/// </summary>
public class Joueur : NetworkBehaviour {
    [SyncVar]
    private bool acted;
    [SyncVar]
    private int score;
    [SyncVar]
    private float ressource;
    private bool isReadyToPlay = false;
    public List<GameObject> personnages=new List<GameObject>();
    public List<int> numeroPrefab;
    private Vector3Int placeToGo;
    private Placeable shotPlaceable;

    /// <summary>
    /// Updates where to go on server, and ask gameManager to do it
    /// </summary>
    /// <param name="toGo"></param>
    [Command]
    public void CmdMoveTo(Vector3 toGo)
    {

        this.placeToGo = new Vector3Int((int)toGo.x, (int)toGo.y, (int)toGo.z);
        if(isServer)
        {
            Debug.Log("Coucou commandé");
            Debug.Log(placeToGo);

        }
    }
    [Command]
    public void CmdInputShot(GameObject shotPlaceable)
    {

        this.shotPlaceable = shotPlaceable.GetComponent<Placeable>();
        if (isServer)
        {
            Debug.Log("Coucou shot commanded");
            Debug.Log(shotPlaceable);

        }
    }
    override
    public void OnStartLocalPlayer()
    {
        
        //On active la camera du joueur et on se dit prêt


    }

    /// <summary>
    /// Permet de savoir si le joueur a agi ce tour ci
    /// </summary>
    public bool Acted
    {
        get
        {
            return acted;
        }

        set
        {
            acted = value;
        }
    }

    /// <summary>
    /// Score du joueur
    /// </summary>
    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    public List<GameObject> Personnages
    {
        get
        {
            return personnages;
        }

        set
        {
            personnages = value;
        }
    }

    public List<int> NumeroPrefab
    {
        get
        {
            return numeroPrefab;
        }

        set
        {
            numeroPrefab = value;
        }
    }

    public float Ressource
    {
        get
        {
            return ressource;
        }

        set
        {
            ressource = value;
        }
        
    }

    public Vector3Int PlaceToGo
    {
        get
        {
            return placeToGo;
        }

        set
        {
            placeToGo = value;
        }
    }

    public Placeable ShotPlaceable
    {
        get
        {
            return shotPlaceable;
        }

        set
        {
            shotPlaceable = value;
        }
    }



    /// <summary>
    /// Liste des personnages du joueur
    /// </summary>



    // Use this for initialization
    void Start () {
        if(this.isServer)
        {
          
            if(!GameObject.Find("GameManager").GetComponent<Game>().joueur1)
            {
                GameObject.Find("GameManager").GetComponent<Game>().joueur1 = this.gameObject;
            }
            else
            {
                GameObject.Find("GameManager").GetComponent<Game>().joueur2 = this.gameObject;
            }
            this.acted = false;
        this.score = 0;
        this.ressource = 0;
        }
        //Retrieve data 
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
