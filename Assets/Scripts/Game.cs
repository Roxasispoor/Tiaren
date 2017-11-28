﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Game : MonoBehaviour {
    private int numberTurn;
   
    
    public GameObject[] prefabPersos;

    public GameObject joueur1; //Should be Object
    
    public GameObject joueur2; //Should be Object
    public GameObject[] prefabMonstres;

    private List<GameObject> listeMonstresNeutres=new List<GameObject>(); 

    private List<Effect> listeEffectsDebutTour=new List<Effect>(); 
    private List<Effect> listeEffectsFinTour=new List<Effect>();

    private GameEffectManager gameEffectManager;
    private Grille grilleJeu;
    private Timer clock;
    private Joueur winner;
    private Vector3Int placeToGo;
    /// <summary>
    /// Permet de savoir si le jeu est fini
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// indique le numéro du tour actuel
    /// </summary>
    /// 
    public int NumberTurn
    {
        get
        {
            return numberTurn;
        }

        set
        {
            numberTurn = value;
        }
    }

    /// <summary>
    /// Indique le joueur en train de jouer
    /// </summary>
   

    public Grille GrilleJeu
    {
        get
        {
            return grilleJeu;
        }

        set
        {
            grilleJeu = value;
        }
    }

    public Joueur Winner
    {
        get
        {
            return winner;
        }

        set
        {
            winner = value;
        }
    }
    private void Awake()

    {
        //    DontDestroyOnLoad(gameObject);

        this.gameEffectManager = gameObject.GetComponent<GameEffectManager>();
        this.clock = gameObject.GetComponent<Timer>();
        this.grilleJeu = gameObject.GetComponent<Grille>();
        grilleJeu.CreateRandomGrid();
      


        //  grilleJeu.ActualisePosition();
        this.placeToGo = new Vector3Int(-1, -1, -1);


    }


    /** Déroulement 
1)On reset pm et le bool de tir/compétences
On applique les effets qui prennent effet en début de tour
On soustrait un tour aux effets persistants attachés

On laisse la main à l'utilisateur, qui peut utiliser une compétence et ses pm
A chaque fois qu'un effet se déclenche, il est ajouté au game manager
Le game manager applique vérifie que l'effet est activable /: pas contré/ autre	puis le use()
On continue jusqu'à la fin des 30s /
**/


    // Use this for initialization
    IEnumerator Start()
    {
        foreach (GameObject pers in joueur1.GetComponent<Joueur>().Personnages)
        {
            Vector3Int posPers = pers.GetComponent<Personnage>().Position;
            this.grilleJeu.Grid[posPers.x, posPers.y, posPers.z] = pers.GetComponent<Personnage>();
        }
       
        yield return new WaitForSeconds(1);
        grilleJeu.Gravite();
        //grilleJeu.SaveGridFile();
        grilleJeu.InitialiseExplored(false);
        Debug.Log(grilleJeu.IsGridAllExplored());
        grilleJeu.Explore(0, 0, 0);
        Debug.Log(grilleJeu.IsGridAllExplored());


        //La speed de turn est déterminée par l'élément le plus lent


        while (Winner == null)
        {
            this.gameEffectManager.ToBeTreated.AddRange(this.listeEffectsDebutTour);
            this.gameEffectManager.Solve();

            List<LivingPlaceable> liste = CreateTurnOrder();
            if(liste.Count != 0)
            { 
            foreach (LivingPlaceable placeable in liste)
            {
                    
                placeable.PmActuels = placeable.PmMax;
                placeable.NbFoisFiredThisTurn = 0;
                this.gameEffectManager.ToBeTreated.AddRange(placeable.OnDebutTour);
                this.gameEffectManager.Solve();
                //Ici l'utilisateur a la main, et 30 secondes.
                
              

                if (placeable.Joueur != null)
                {
                        clock.IsFinished = false;
                    clock.StartTimer(30f);

                    //Coroutine routine= StartCoroutine(PlayerChoice(clock));

                    bool endPhase = false;
                    Vector3Int positiongo = new Vector3Int(placeable.Position.x, placeable.Position.y-1, placeable.Position.z);
                        
                    DistanceAndParent[,,] inPlace=grilleJeu.CanGo(placeable, placeable.PmMax, positiongo);
                         Debug.Log("C'est le debut lol!");
                        Vector3Int vecTest = new Vector3Int(-1, -1, -1);
                        while (placeable.NbFoisFiredThisTurn < 1 && placeable.PmActuels > 0 && !endPhase && !clock.IsFinished && placeToGo == vecTest) 
                    {
                        yield return null;
                        
                    }
                        Debug.Log("C'est la fin lol!");
                    //On applique les changements
                    //StopCoroutine(routine);
                    }
                this.gameEffectManager.ToBeTreated.AddRange(this.listeEffectsFinTour);
                this.gameEffectManager.Solve();
            }
            }
            else
            {
                yield return null;
            }


        }
    }
 

    public  List<LivingPlaceable> CreateTurnOrder(){
        int maxSpeedStack = 0;
        List<LivingPlaceable> liste = new List<LivingPlaceable>();

        foreach (GameObject pers1b in joueur1.GetComponent<Joueur>().Personnages)
        {
            Personnage pers1 = pers1b.GetComponent<Personnage>();
            pers1.SpeedStack += 1 / pers1.Speed;

            liste.Add(pers1);
            if (maxSpeedStack< pers1.SpeedStack)
            {
                maxSpeedStack = pers1.SpeedStack;
            }

        }
        foreach (GameObject pers2b in joueur2.GetComponent<Joueur>().Personnages)
        {
            Personnage pers2 = pers2b.GetComponent<Personnage>();
            liste.Add(pers2);
            if (maxSpeedStack < pers2.SpeedStack)
            {
                maxSpeedStack = pers2.SpeedStack;
            }

        }
        foreach (GameObject monstre2 in listeMonstresNeutres)
        {
            LivingPlaceable monstre = monstre2.GetComponent<LivingPlaceable>();

            monstre.SpeedStack += 1 / monstre.Speed;
            liste.Add(monstre);
            if (maxSpeedStack < monstre.SpeedStack)
            {
                maxSpeedStack = monstre.SpeedStack;
            }

        }

        foreach (GameObject pers1b in joueur1.GetComponent<Joueur>().Personnages)
        {
            Personnage pers1 = pers1b.GetComponent<Personnage>();

            if (pers1.SpeedStack<maxSpeedStack + 1/pers1.Speed) // alors on peut le rerajouter
            {
                pers1.SpeedStack += 1 / pers1.Speed;
                liste.Add(pers1);
            }

        }

        foreach (GameObject pers2b in joueur2.GetComponent<Joueur>().Personnages)
        {
            Personnage pers2 = pers2b.GetComponent<Personnage>();
            if (pers2.SpeedStack < maxSpeedStack + 1 / pers2.Speed) // alors on peut le rerajouter
            {
                pers2.SpeedStack += 1 / pers2.Speed;
                liste.Add(pers2);
            }
        }
        foreach (GameObject monstre2 in listeMonstresNeutres)
        {
            LivingPlaceable monstre = monstre2.GetComponent<LivingPlaceable>();
            if (monstre.SpeedStack < maxSpeedStack + 1 / monstre.Speed) // alors on peut le rerajouter
            {
                monstre.SpeedStack += 1 / monstre.Speed;
                liste.Add(monstre);
            }
        }


        liste.Sort((x, y) => x.SpeedStack - y.SpeedStack);

        return liste;
     }
    public void Shootable(LivingPlaceable shooter)
    {
        Vector3 shootpos = shooter.position + shooter.ShootPosition;
        foreach (GameObject pers1b in joueur1.GetComponent<Joueur>().Personnages)
        {

            Personnage pers1 = pers1b.GetComponent<Personnage>();

            foreach (HitablePoint pointToHit in pers1.HitablePoints)
            {
                if (Physics.Raycast(shootpos, shootpos - (pers1b.transform.position + pointToHit.RelativePosition),
                    (shootpos - (pers1b.transform.position + pointToHit.RelativePosition)).magnitude))
                {
                    pointToHit.Shootable = true;
                }
                else
                {
                    pointToHit.Shootable = false;
                }
            }

        }

        foreach (GameObject pers2b in joueur2.GetComponent<Joueur>().Personnages)
        {
            Personnage pers2 = pers2b.GetComponent<Personnage>();

            foreach (HitablePoint pointToHit in pers2.HitablePoints)
            {
                if (Physics.Raycast(shootpos, shootpos - (pers2b.transform.position + pointToHit.RelativePosition),
                    (shootpos - (pers2b.transform.position + pointToHit.RelativePosition)).magnitude))
                {
                    pointToHit.Shootable = true;
                }
                else
                {
                    pointToHit.Shootable = false;
                }
            }



        }
        foreach (GameObject monstre2 in listeMonstresNeutres)
        {
            LivingPlaceable monstre = monstre2.GetComponent<LivingPlaceable>();


            foreach (HitablePoint pointToHit in monstre.HitablePoints)
            {
                if (Physics.Raycast(shootpos, shootpos - (monstre2.transform.position + pointToHit.RelativePosition),
                    (shootpos - (monstre2.transform.position + pointToHit.RelativePosition)).magnitude))
                {
                    pointToHit.Shootable = true;
                }
                else
                {
                    pointToHit.Shootable = false;
                }

            }
        }
    }

            /*
            private int MinSpeedPersos()
            {
               int minSpeed = 1000000000;
                foreach (Personnage pers1 in joueur1.Personnages)
                {
                    if(minSpeed>pers1.Speed)
                    {
                        minSpeed = pers1.Speed;
                    }

                }
                foreach (Personnage pers2 in joueur2.Personnages)
                {
                    if (minSpeed > pers2.Speed)
                    {
                        minSpeed = pers2.Speed;
                    }

                }
                return minSpeed;
            }
            */
            // Update is called once per frame
            void Update () {
		
	}
}
