﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Classe centrale gérant le déroulement des tours et répertoriant les objets
/// </summary>
public class GameManager : NetworkBehaviour
{
    //can't be the network manager or isServer can't work
    public static GameManager instance;
    public NetworkManager networkManager;
    private int numberTurn = 0;

    private Placeable shotPlaceable;
    public GameObject[] prefabPersos;
    public GameObject[] prefabArmes;
    public GameObject gridFolder;
    public GameObject player1; //Should be Object

    public GameObject player2; //Should be Object
    public GameObject[] prefabMonstres;

    //private List<GameObject> listeMonstresNeutres = new List<GameObject>();

    private List<LivingPlaceable> turnOrder;
    //private List<Effect> listeEffectsDebutTour = new List<Effect>();
    //private List<Effect> listeEffectsFinTour = new List<Effect>();
    //private int playingPlaceable.CapacityinUse = 0;
    //private GameEffectManager gameEffectManager;
    //private Grille Grid.instance;

    private Player winner;
    private LivingPlaceable playingPlaceable;

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


    
    public Player Winner
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

    


    public LivingPlaceable PlayingPlaceable
    {
        get
        {
            return playingPlaceable;
        }

        set
        {
            playingPlaceable = value;
        }
    }

    public List<LivingPlaceable> TurnOrder
    {
        get
        {
            return turnOrder;
        }

        set
        {
            turnOrder = value;
        }
    }



    /// <summary>
    /// Return the player who has authority
    /// </summary>
    /// <returns></returns>
    public Player GetLocalPlayer()
    {
        Player toreturn = player1.GetComponent<Player>().hasAuthority ? player1.GetComponent<Player>() : player2.GetComponent<Player>();
        return toreturn;
    }

    private void Awake()

    {
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);



        TurnOrder = new List<LivingPlaceable>();
        //    DontDestroyOnLoad(gameObject);
        //Initialize la valeur statique de chaque placeable, devrait rester identique entre deux versions du jeu, et ne pas poser problème si les new prefabs sont bien rajoutés a la fin
        for (int i = 0; i < networkManager.spawnPrefabs.Count; i++)
        {
            networkManager.spawnPrefabs[i].GetComponent<Placeable>().serializeNumber = i + 1; // sorte de valeur partagée a tous les préfabs, pas besoin d'etre statique


        }

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
    [Server]
    IEnumerator Start()
    {

        if (isServer)
        {
            //If you want to create one and save it
            // Grid.instance.CreateRandomGrid(gridFolder);
            //Grid.instance.SaveGridFile();

            //If you want to load one
            Grid.instance.FillGridAndSpawn(gridFolder);
        }
        while (player1 == null)
        {
            yield return null;
        }

        CreateCharacters(player1);
        while (player2 == null)
        {
            yield return null;
        }
        CreateCharacters(player2);

        //TODO modifier: les monstres neutres seront un autre joueur ne pouvant simplement pas gagner
/*        foreach (GameObject monstre in listeMonstresNeutres)
        {
            LivingPlaceable monstre1 = monstre.GetComponent<Personnage>();
            monstre1.GameManager = this;
            Vector3Int posPers = monstre1.Position;
            this.Grid.instance.Grid[posPers.x, posPers.y, posPers.z] = monstre1;
            monstre1.Armes.Add(Instantiate(prefabArmes[0], monstre.transform)); // a changer selon l'arme de départ
            monstre1.EquipedArm = monstre1.Armes[0].GetComponent<Arme>();
            tourOrder.Add(monstre1);
        }
        */
        Grid.instance.Gravity();
        Grid.instance.InitializeExplored(false);


        while (Winner == null)
        {
            
            //Sort by speedstack, add inverse of speed each time it plays
            TurnOrder.Sort((x, y) => x.SpeedStack == y.SpeedStack ? 1 : (int)((x.SpeedStack - y.SpeedStack) / (Mathf.Abs(x.SpeedStack - y.SpeedStack))));
            playingPlaceable = TurnOrder[0];
            playingPlaceable.SpeedStack += 1 / playingPlaceable.Speed;
            //Petite verif on sait jamais
            if (TurnOrder.Count != 0 && playingPlaceable.Player != null)
            {


                playingPlaceable.player.RpcSetCamera(playingPlaceable.netId);
                playingPlaceable.PmActuels = playingPlaceable.PmMax;
                playingPlaceable.TimesFiredThisTurn = 0;
                //Ici l'utilisateur a la main, et 30 secondes.

                if (!playingPlaceable.IsDead)
                {
                    playingPlaceable.Player.clock.IsFinished = false;
                    playingPlaceable.Player.clock.StartTimer(30f);
                    playingPlaceable.Player.RpcStartTimer(30f);


                    playingPlaceable.Player.EndPhase = false;
                    //TODO select spawn place
                    Vector3Int positionDepart = playingPlaceable.GetPosition() - new Vector3Int(0, 1, 0);//le pathfinding commence 1 sous le joueur

                    DistanceAndParent[,,] inPlace = Grid.instance.CanGo(playingPlaceable, playingPlaceable.PmMax, positionDepart);
                    Debug.Log("Tour de" + playingPlaceable.player);
                    Vector3Int vecTest = new Vector3Int(-1, -1, -1);
                    while (!playingPlaceable.Player.EndPhase && !playingPlaceable.Player.clock.IsFinished && (playingPlaceable.TimesFiredThisTurn < 1 || playingPlaceable.PmActuels > 0))
                    {
                        if (shotPlaceable != null && playingPlaceable.CapacityinUse == 0 && shotPlaceable != playingPlaceable && playingPlaceable.CanHit(shotPlaceable).Count > 0)// si il se tire pas dessus et qu'il a bien sélectionné quelqu'un
                        {
                            //piew piew
                            Debug.Log("Piew piew");
                            Vector3 thePlaceToShoot = playingPlaceable.ShootDamage(shotPlaceable); // TODO: pour les animations
                        }
                        else if (shotPlaceable != null && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Competences[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                            playingPlaceable.Competences[playingPlaceable.CapacityinUse].CompetenceType == CompetenceType.ONECLICKLIVING
                          
                            && shotPlaceable.GetType() == typeof(LivingPlaceable)
                            && playingPlaceable.Competences[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Competences[playingPlaceable.CapacityinUse].Use();

                        }
                        else if (shotPlaceable == playingPlaceable && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Competences[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                            playingPlaceable.Competences[playingPlaceable.CapacityinUse].CompetenceType == CompetenceType.SELFCOMPETENCE
                             
                            && playingPlaceable.Competences[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Competences[playingPlaceable.CapacityinUse].Use();
                          
                        }
                        else if (shotPlaceable != null && playingPlaceable.CapacityinUse != 0 && playingPlaceable.Competences[playingPlaceable.CapacityinUse].TourCooldownLeft == 0 &&
                          playingPlaceable.Competences[playingPlaceable.CapacityinUse].CompetenceType == CompetenceType.ONECLICKPLACEABLE
                         
                          && shotPlaceable.GetType() == typeof(Placeable)
                          && playingPlaceable.Competences[playingPlaceable.CapacityinUse].condition())
                        {
                            playingPlaceable.Competences[playingPlaceable.CapacityinUse].Use();
                          
                        }


                        else if (playingPlaceable.player.PlaceToGo != vecTest &&
                            inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z].GetDistance() > 0
                            && inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z].GetDistance() <= playingPlaceable.PmActuels)
                        {
                            List<Vector3> listAnimator = new List<Vector3>();
                            DistanceAndParent parent = inPlace[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y, playingPlaceable.player.PlaceToGo.z];
                            listAnimator.Add(parent.Pos);
                            while (parent.GetDistance() > 1)
                            {

                                parent = parent.GetParent();
                                listAnimator.Add(parent.Pos);
                            }
                            listAnimator.Add(playingPlaceable.GetPosition() - new Vector3Int(0, 1, 0)); //on veut l'emplacement dessous en fait
                            RpcMoveAlongBezier(listAnimator.ToArray(), playingPlaceable.netId, 1);
                            playingPlaceable.PmActuels -= listAnimator.Count - 1;

                            //on actualise les positions
                            Grid.instance.GridMatrix[playingPlaceable.player.PlaceToGo.x, playingPlaceable.player.PlaceToGo.y + 1, playingPlaceable.player.PlaceToGo.z] = playingPlaceable;
                            Grid.instance.GridMatrix[playingPlaceable.GetPosition().x, playingPlaceable.GetPosition().y, playingPlaceable.GetPosition().z] = null; // on est plus a l'emplacement précédent
                            
                            //On efface les anciens
                            PlayingPlaceable.Player.RpcChangeBackColor();

                            //On se déplace, donc on actualise ce qu'on peut voir, et on remet a 0 le vectest
                            inPlace = Grid.instance.CanGo(playingPlaceable, playingPlaceable.PmMax, playingPlaceable.player.PlaceToGo);
                            playingPlaceable.player.PlaceToGo = vecTest;
                            Debug.Log("runny run");

                        }
                        yield return null;

                    }
                    //on diminue de 1 le cooldown de la competence
                    foreach (Skill comp in playingPlaceable.Competences)
                    {
                        if (comp.TourCooldownLeft > 0)
                        {
                            comp.TourCooldownLeft--;
                        }
                    }
                    shotPlaceable = null;
                    //On efface les bleus encore actifs a la fin
                    PlayingPlaceable.Player.RpcChangeBackColor();

                    playingPlaceable.player.PlaceToGo = vecTest;


                    playingPlaceable.Player.clock.IsFinished = false;
                    Debug.Log("C'est la fin!");

                    //On applique les changements
 
                }
                else
                {
                    //On fast forward a la prochaine personne
                    playingPlaceable.TourRestantsCimetiere--;
                }



             

            }
            else
            {
                yield return null;
            }


        }
    }
    /// <summary>
    /// See https://www.gamedev.net/forums/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public float LengthQuadraticBezier(Vector3 start, Vector3 end, Vector3 control)
    {
        Vector3[] C = { start, control, end };
        // ASSERT:  C[0], C[1], and C[2] are distinct points.

        // The position is the following vector-valued function for 0 <= t <= 1.
        //   P(t) = (1-t)^2*C[0] + 2*(1-t)*t*C[1] + t^2*C[2].
        // The derivative is
        //   P'(t) = -2*(1-t)*C[0] + 2*(1-2*t)*C[1] + 2*t*C[2]
        //         = 2*(C[0] - 2*C[1] + C[2])*t + 2*(C[1] - C[0])
        //         = 2*A[1]*t + 2*A[0]
        // The squared length of the derivative is
        //   f(t) = 4*Dot(A[1],A[1])*t^2 + 8*Dot(A[0],A[1])*t + 4*Dot(A[0],A[0])
        // The length of the curve is
        //   integral[0,1] sqrt(f(t)) dt

        Vector3[] A ={ C[1] - C[0],  // A[0] not zero by assumption
        C[0] - 2.0f*C[1] + C[2]
    };

        float length;

        if (A[1] != Vector3.zero)
        {
            // Coefficients of f(t) = c*t^2 + b*t + a.
            float c = 4.0f * Vector3.Dot(A[1], A[1]);  // c > 0 to be in this block of code
            float b = 8.0f * Vector3.Dot(A[0], A[1]);
            float a = 4.0f * Vector3.Dot(A[0], A[0]);  // a > 0 by assumption
            float q = 4.0f * a * c - b * b;  // = 16*|Cross(A0,A1)| >= 0

            // Antiderivative of sqrt(c*t^2 + b*t + a) is
            // F(t) = (2*c*t + b)*sqrt(c*t^2 + b*t + a)/(4*c)
            //   + (q/(8*c^{3/2}))*log(2*sqrt(c*(c*t^2 + b*t + a)) + 2*c*t + b)
            // Integral is F(1) - F(0).

            float twoCpB = 2.0f * c + b;
            float sumCBA = c + b + a;
            float mult0 = 0.25f / c;
            float mult1 = q / (8.0f * Mathf.Pow(c, 1.5f));
            length =
                mult0 * (twoCpB * Mathf.Sqrt(sumCBA) - b * Mathf.Sqrt(a)) +
                mult1 * (Mathf.Log(2.0f * Mathf.Sqrt(c * sumCBA) + twoCpB) - Mathf.Log(2.0f * Mathf.Sqrt(c * a) + b));
        }
        else
        {
            length = 2.0f * A[0].magnitude;
        }

        return length;
    }
    /// <summary>
    /// Unused Hacky way to approximate bezierr length
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public float LengthBezierApprox(Vector3 start, Vector3 end, Vector3 control)
    {
        float chord = Vector3.Distance(end, start);
        float cont = Vector3.Distance(start, control) + Vector3.Distance(control, end);
        return (cont + chord) / 2;
    }
    /// <summary>
    /// Calculate distance of bezier, update isBezier and the needed controlPoint
    /// </summary>
    /// <param name="start"></param>
    /// <param name="nextNode"></param>
    /// <param name="isBezier"></param>
    /// <returns></returns>
    public float CalculateDistance(Vector3 start, Vector3 nextNode, ref bool isBezier, ref Vector3 controlPoint)
    {
        isBezier = start.y != nextNode.y;
        if (isBezier)// si il y a différence de hauteur
        {
            controlPoint = (nextNode + start + 2 * new Vector3(0, Mathf.Abs(nextNode.y - start.y), 0)) / 2;

            return LengthQuadraticBezier(start, nextNode, controlPoint);
        }
        else
        {

            return Vector3.Distance(start, nextNode);
        }

    }
    [ClientRpc]
    public void RpcMoveAlongBezier(Vector3[] path, NetworkInstanceId placeable, float speed)
    {
        Debug.Log(placeable);
        GameObject plc = ClientScene.FindLocalObject(placeable);

        List<Vector3> pathe = new List<Vector3>(path);
        StartCoroutine(MoveAlongBezier(pathe, plc.GetComponent<Placeable>(), speed));
        Debug.Log("Started on client though");
    }


    [Client]
    public IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, float speed)
    {
        float tempsBezier = 0f;
        Vector3 delta = placeable.transform.position - path[path.Count - 1];
        Vector3 startPosition = path[path.Count - 1];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;
        //For visual rotation
        Vector3 targetDir = path[path.Count - 2] - placeable.transform.position;
        targetDir.y = 0;

        int i = path.Count - 2;

        float distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        float distanceParcourue = 0;
        while (tempsBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            tempsBezier = distanceParcourue / distance;

            while (tempsBezier > 1 && i > 0) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i--;
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                tempsBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == 0 && tempsBezier > 1)
            {
                // on est arrivé au dernier node du chemin, dans la boucle précédente
                placeable.transform.position = path[i] + delta;
                // sortie de la boucle qui yield
                break;
            }
            if (isBezier)
            {
                placeable.transform.position = delta + Mathf.Pow(1 - tempsBezier, 2) * (startPosition) + 2 * (1 - tempsBezier) * tempsBezier * (controlPoint) + Mathf.Pow(tempsBezier, 2) * (path[i]);

            }
            else
            {
                placeable.transform.position = Vector3.Lerp(startPosition + delta, path[i] + delta, tempsBezier);

            }
            Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
            placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            //On change ce qu'on regarde

            yield return null;


        }

    }


    /// <summary>
    /// déplace le personnage le long du chemin
    /// </summary>
    /// <param name="path"></param>
    /// <param name="placeable"></param>
    /// <returns></returns>
    public IEnumerator ApplyMove(List<Vector3> path, Placeable placeable)
    {
        Vector3 delta = placeable.transform.position - path[path.Count - 1];
        float speed = 1;
        // distance parcourue depuis le point de départ
        float travelDistance = 0;

        // position de départ
        Vector3 startPosition = path[path.Count - 1] + delta;

        Vector3 targetDir = path[path.Count - 2] - placeable.transform.position;
        targetDir.y = 0;
        // boucle sur tous les point du chemin (c'est toujours mieux de stocker le total dans une variable locale)
        for (int i = path.Count - 2, count = path.Count, lastIndex = 0; i >= 0; i--)
        {
            //targetDir = path[i] - placeable.transform.position;
            // distance entre le point de départ et le point d'arrivée (node actuel, node suivant)
            float distance = Vector3.Distance(startPosition, path[i] + delta);

            // vecteur directeur entre ces deux points
            Vector3 direction = (path[i] + delta - startPosition).normalized;

            // boucle tant qu'on a pas encore dépassé la position du node suivant
            while (travelDistance < distance)
            {

                // on avance en fonction de la vitesse de déplacement et du temps écoulé
                travelDistance += (speed * Time.deltaTime);

                // si on a dépassé ou atteint la position du node d'arrivée
                if (travelDistance >= distance)
                {

                    // si on est encore en chemin, 
                    if (i > lastIndex)
                    {
                        targetDir = path[i - 1] - placeable.transform.position;
                        targetDir.y = 0;
                        // on se positionne un peu plus loin sur le chemin 
                        // entre les deux nodes suivants, selon la distance parcourue au delà du node d'arrivée actuel
                        float distanceNext = Vector3.Distance(path[i - 1], path[i]);

                        float ratio = (travelDistance - distance) / distanceNext;

                        // si le ratio est supérieur à 1, c'est que la distance parcourue
                        // est aussi supérieur à la distance entre les deux node suivants (donc on bouge vite)
                        // cette boucle va sauter tous les nodes qu'on est censé avoir parcourus en se déplaçant
                        // à grande vitesse
                        while (ratio > 1)
                        {
                            i--;
                            if (i == lastIndex)
                            {
                                // on est arrivé au dernier node du chemin
                                placeable.transform.position = path[i] + delta;
                                // sortie de la boucle
                                break;
                            }
                            else
                            {
                                travelDistance -= distance;
                                distance = distanceNext;
                                distanceNext = Vector3.Distance(path[i - 1], path[i]);
                                ratio = (travelDistance - distance) / distanceNext;
                            }
                        }

                        if (i == lastIndex)
                        {
                            // on est arrivé au dernier node du chemin dans le while précédent
                            break;
                        }
                        else
                        {

                            transform.position = Vector3.Lerp(path[i], path[i - 1], ratio);
                        }

                    }
                    else
                    {
                        // on est arrivé au dernier node du chemin
                        placeable.transform.position = path[i] + delta;

                        break;
                    }
                }
                else
                {
                    // sinon on avance en direction du point d'arrivée
                    placeable.transform.position += direction * (speed * Time.deltaTime);
                }

                Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
                placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);

                yield return null;
            }

            // on retire la distance qu'il y avait à parcourir entre les deux nodes précédents
            travelDistance -= distance;

            // mise à jour de la position de départ pour l'itération suivante
            startPosition = path[i] + delta;
        }

    }

    [Server]
    private void CreateCharacters(GameObject player)
    {
        for (int i = 0; i < player.GetComponent<Player>().NumeroPrefab.Count; i++)
        {
            GameObject pers = Instantiate(prefabPersos[player.GetComponent<Player>().NumeroPrefab[i]], new Vector3(0, 4f, 0), Quaternion.identity);



            LivingPlaceable pers1 = pers.GetComponent<LivingPlaceable>();
          
            player.GetComponent<Player>().Personnages.Add(pers);
            pers1.Player = player.GetComponent<Player>();
           
            Vector3Int posPers = new Vector3Int(0, 5, 0);//POSITION DE DEPART
            Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = pers1;
            pers1.Armes.Add(Instantiate(prefabArmes[0], pers.transform)); // a changer selon l'arme de départ
            pers1.EquipedArm = pers1.Armes[0].GetComponent<Arme>();

            TurnOrder.Add(pers1);
            NetworkServer.Spawn(pers);
            RpcCreatePerso(pers, player);

        }

    }
    [ClientRpc]
    public void RpcCreatePerso(GameObject pers, GameObject joueur)
    {
        LivingPlaceable pers1 = pers.GetComponent<LivingPlaceable>();
        joueur.GetComponent<Player>().Personnages.Add(pers);
        pers1.Player = joueur.GetComponent<Player>();
        Vector3Int posPers = new Vector3Int(0,4,0);
        Grid.instance.GridMatrix[posPers.x, posPers.y, posPers.z] = pers1;
        pers1.Armes.Add(Instantiate(prefabArmes[0], pers.transform)); // a changer selon l'arme de départ
        pers1.EquipedArm = pers1.Armes[0].GetComponent<Arme>();
    }

    /// <summary>
    /// Fonctions inutilisée qui permet d'appliquer une fonction a tous les personnages visable
    /// </summary>
    /// <param name="shooter"></param>
    public void Shootable(LivingPlaceable shooter)
    {
        Vector3 shootpos = shooter.GetPosition() + shooter.ShootPosition;
        foreach (GameObject pers1b in player1.GetComponent<Player>().Personnages)
        {

            LivingPlaceable pers1 = pers1b.GetComponent<LivingPlaceable>();

            if (shooter.CanHit(pers1).Count > 0)
            {

                //typiquement, on change sa couleur
            }

        }

        foreach (GameObject pers2b in player2.GetComponent<Player>().Personnages)
        {
            LivingPlaceable pers2 = pers2b.GetComponent<LivingPlaceable>();

            if (shooter.CanHit(pers2).Count > 0)
            {

                //typiquement, on change sa couleur
            }


        }
      
    }


    // Update is called once per frame
    void Update()
    {


    }
}