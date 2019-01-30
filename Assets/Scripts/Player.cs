using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Represents a player
/// </summary>
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool acted;
    [SyncVar]
    private int score;
    public bool isReadyToPlay = false;
    public List<GameObject> characters = new List<GameObject>();
    public List<int> numberPrefab;
    public List<Vector3Int> spawnList;
    private Vector3Int placeToGo;
    private bool isready;
    public bool isWinner = false;
    public Text winText;
    public PlayerAccount account;
    private Placeable shotPlaceable;
    public CameraScript cameraScript;
    public delegate float Axis();
    public delegate bool Condition();
    private Dictionary<string, Axis> dicoAxis;
    private Dictionary<string, Condition> dicoCondition;
    public Timer clock;

    public Color color;

    /// <summary>
    /// Function to know if the player acted during this turn
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
    /// Score of player
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

    public List<GameObject> Characters
    {
        get
        {
            return characters;
        }

        set
        {
            characters = value;
        }
    }

    public List<int> NumberPrefab
    {
        get
        {
            return numberPrefab;
        }

        set
        {
            numberPrefab = value;
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



    public Dictionary<string, Axis> DicoAxis
    {
        get
        {
            return dicoAxis;
        }

        set
        {
            dicoAxis = value;
        }
    }

    public Dictionary<string, Condition> DicoCondition
    {
        get
        {
            return dicoCondition;
        }

        set
        {
            dicoCondition = value;
        }
    }

    public bool Isready
    {
        get
        {
            return isready;
        }

        set
        {
            isready = value;
        }
    }


    [Command]
    public void CmdSetName(string username)
    {
        Debug.LogError("Bienvenue dans cmd setname");  
        Debug.LogError(GameManager.instance.player1);  
        Debug.LogError(GameManager.instance.player1Username);  
        if(GameManager.instance.player1==null && GameManager.instance.player1Username=="" )
        {
                GameManager.instance.player1 = gameObject;
                Debug.LogError("player1 is " + username);
            GameManager.instance.player1Username = username;
        }
       else if (GameManager.instance.player2 == null  && GameManager.instance.player2Username=="")
        {

            GameManager.instance.player2 = gameObject;
            Debug.LogError("player2 is " + username);
            GameManager.instance.player2Username = username;
        }
        else if(GameManager.instance.player1 == null && GameManager.instance.player1Username == username)
        {
            Debug.LogError("Player1 reconnect!");
           
            Debug.LogError("Transmitter started");
            StartCoroutine(GameManager.instance.transmitter.AcceptTcp());
            RpcListen();
            

           /*
            GameManager.instance.player1 = gameObject;
            Debug.LogError("player1 is " + username);
            GameManager.instance.player1Username = username;*/
            //GameManager.instance.player2.GetComponent<Player>().RpcReconnectPlayer();
        }
        else if (GameManager.instance.player2 == null && GameManager.instance.player2Username == username)
        {
            Debug.LogError("Player2 reconnect!");
            Debug.LogError("Transmitter started");
            StartCoroutine(GameManager.instance.transmitter.AcceptTcp());
            RpcListen();
/*
            GameManager.instance.player2 = gameObject;
            Debug.LogError("player2 is " + username);
            GameManager.instance.player2Username = username;*/
        }
        else
        {
            Debug.LogError("Who dis? GTFO"); ;
        }


    }
/// <summary>
/// Reconnects player and gives ALL livings without owner to new
/// </summary>
    [ClientRpc]
    public void RpcReconnectPlayer()
    {
        if(!isLocalPlayer)
        { 
        Reforge();
        }
    }
    public void Reforge()
    {
        Debug.LogError("p1 is"+GameManager.instance.player1 + ". Is it me?"+ (GameManager.instance.player1 == gameObject));
        Debug.LogError("p2 is"+GameManager.instance.player2 + ". Is it me?" + (GameManager.instance.player2 == gameObject));
        if (GameManager.instance.player1 == null)
        {
            GameManager.instance.player1 = gameObject;
            Debug.LogError("Player 1 reconnects, trying to reforge links");
            if(isServer) 
            {
                GameManager.instance.player1.GetComponent<Player>().RpcReconnectPlayer(); //sent to the two new players, only the old one must do it though
            }
        }
        if (GameManager.instance.player2 == null)
        {
            GameManager.instance.player2 = gameObject;
            Debug.LogError("Player 2 reconnects, trying to reforge links");
            if (isServer)
            {
                GameManager.instance.player2.GetComponent<Player>().RpcReconnectPlayer(); //sent to the two new players, only the old one must do it though
            }
        }
        LivingPlaceable[] livings = FindObjectsOfType<LivingPlaceable>();
        foreach (LivingPlaceable living in livings)
        {
            if (living.Player == null)
            {
                living.Player = this;
                Characters.Add(living.gameObject);
            }
        }
    }

    [ClientRpc]
    public void RpcListen()
    {
        if(isLocalPlayer)
        { 
        Debug.LogError("Client listen to data start coroutine");
        StartCoroutine(GameManager.instance.transmitter.ListenToData(this));
        }
    }

    /// <summary>
    /// List of characters of the player
    /// </summary>

    private void Awake()
    {
       
        clock = GetComponent<Timer>();
        DicoAxis = new Dictionary<string, Axis>();
        DicoCondition = new Dictionary<string, Condition>();
        DicoAxis.Add("AxisXCamera", () => Input.GetAxis("Mouse X"));
        DicoAxis.Add("AxisYCamera", () => Input.GetAxis("Mouse Y"));
        DicoAxis.Add("AxisZoomCamera", () => Input.GetAxis("Mouse ScrollWheel"));
        DicoCondition.Add("BackToMovement", () => Input.GetButtonDown("BackToMovement"));
        DicoCondition.Add("OrbitCamera", () => Input.GetMouseButton(1));
        DicoCondition.Add("PanCamera", () => Input.GetMouseButton(2));
        Isready = false;
    }

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            if (GameManager.instance.State == States.TeamSelect)
            {
                GameObject firstCanva = gameObject.transform.Find("TeamCanvas").gameObject;
                firstCanva.SetActive(true);
                firstCanva.transform.Find("TitleText").GetComponent<Text>().text = "Waiting for other player";
            }
            account = FindObjectOfType<PlayerAccount>();
            if (account != null && account.AccountInfoPacket.Username != null)
            {
                CmdSetName(account.AccountInfoPacket.Username);
            }
            else
            {
                CmdSetName("Patate");
            }

        }
        if (isClient)
        { 
            if (GameManager.instance.player1 == null) //Initialize on client and ask server if this was possible
            {
                GameManager.instance.player1 = gameObject;
            }
            else
            {
                GameManager.instance.player2 = gameObject;
            }
        }
        //Debug.Log("STARTCLIENT!");

    }

    public void SendSpawnToCamera()
    {
        Vector3 spawncenter = new Vector3(0, 0, 0);
        foreach (Vector3Int point in spawnList)
            spawncenter += point;
        cameraScript.SpawnCenter = spawncenter / spawnList.Count;
        cameraScript.Init();
    }

    public void displaySpawn()
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<UIManager>().SpawnUI();

            Player localPlayer = GameManager.instance.GetLocalPlayer();
            Player enemyPlayer = GameManager.instance.GetOtherPlayer(localPlayer.gameObject).GetComponent<Player>();


            if (localPlayer == GameManager.instance.Player1)
            {
                SpawnLocalPlayer(localPlayer);
                SpawnEnemyPlayer(enemyPlayer);
            } else
            {
                SpawnEnemyPlayer(enemyPlayer);
                SpawnLocalPlayer(localPlayer);
            }
            
            GameManager.instance.ResetAllBatches();
            gameObject.GetComponent<UIManager>().SpawnUI();
        }
    }
    
    private void SpawnLocalPlayer(Player localPlayer)
    {
        for (int i = 0; i < localPlayer.spawnList.Count; i++)
        {
            Grid.instance.GridMatrix[localPlayer.spawnList[i].x, localPlayer.spawnList[i].y - 1,
                localPlayer.spawnList[i].z].GetComponent<MeshRenderer>().material = GameManager.instance.spawnAllyMaterial;

            Grid.instance.GridMatrix[localPlayer.spawnList[i].x,
                localPlayer.spawnList[i].y - 1,
                localPlayer.spawnList[i].z].IsSpawnPoint = true;
            if (i < localPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters.Count)
            {
                GameManager.instance.CreateCharacter(localPlayer.gameObject, localPlayer.spawnList[i], localPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters[i]);
            }
        }
    }

    private void SpawnEnemyPlayer(Player enemyPlayer)
    {
        for (int i = 0; i < enemyPlayer.spawnList.Count; i++)
        {
            Grid.instance.GridMatrix[enemyPlayer.spawnList[i].x, enemyPlayer.spawnList[i].y - 1,
                enemyPlayer.spawnList[i].z].GetComponent<MeshRenderer>().material = GameManager.instance.spawnEnemyMaterial;
            Grid.instance.GridMatrix[enemyPlayer.spawnList[i].x, enemyPlayer.spawnList[i].y - 1,
                enemyPlayer.spawnList[i].z].IsSpawnPoint = true;
            if (i < enemyPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters.Count)
            {
                GameManager.instance.CreateCharacter(enemyPlayer.gameObject, enemyPlayer.spawnList[i], enemyPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters[i]);
            }
        }

    }

    [Command]
    public void CmdTeamReady(int[] characterChoices)
    {
        Isready = true;
        List<int> numbers = new List<int>(characterChoices);
        gameObject.GetComponent<UIManager>().CurrentCharacters = numbers;

        GameManager.instance.InitStartGame();

        if (GameManager.instance.player1.GetComponent<Player>().Isready && GameManager.instance.player2.GetComponent<Player>().Isready)
        {
            GameManager.instance.player1.GetComponent<Player>().Isready = false;
            GameManager.instance.player2.GetComponent<Player>().Isready = false;
            GameManager.instance.State = States.Spawn;

            int[] choicesP1 = new int[GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters.Count];
            for (int i = 0; i < GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters.Count; i++)
            {
                choicesP1[i] = GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters[i];
            }

            int[] choicesP2 = new int[GameManager.instance.player2.GetComponent<UIManager>().CurrentCharacters.Count];
            for (int i = 0; i < GameManager.instance.player2.GetComponent<UIManager>().CurrentCharacters.Count; i++)
            {
                choicesP2[i] = GameManager.instance.player2.GetComponent<UIManager>().CurrentCharacters[i];
            }

            // Spawn the characters
            for (int i = 0; i < Grid.instance.SpawnPlayer1.Count; i++)
            {
                Grid.instance.GridMatrix[Grid.instance.SpawnPlayer1[i].x,
                    Grid.instance.SpawnPlayer1[i].y - 1,
                    Grid.instance.SpawnPlayer1[i].z].IsSpawnPoint = true;
                if (i < GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters.Count)
                {
                    GameManager.instance.CreateCharacter(GameManager.instance.player1, Grid.instance.SpawnPlayer1[i], GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters[i]);
                }
            }
            for (int i = 0; i < Grid.instance.SpawnPlayer2.Count; i++)
            {
                Grid.instance.GridMatrix[Grid.instance.SpawnPlayer2[i].x, Grid.instance.SpawnPlayer2[i].y - 1,
                    Grid.instance.SpawnPlayer2[i].z].IsSpawnPoint = true;
                if (i < GameManager.instance.player2.GetComponent<UIManager>().CurrentCharacters.Count)
                {
                    GameManager.instance.CreateCharacter(GameManager.instance.player2, Grid.instance.SpawnPlayer2[i], GameManager.instance.player2.GetComponent<UIManager>().CurrentCharacters[i]);
                }
            }
            GameManager.instance.State = States.Spawn;
            GameManager.instance.player1.GetComponent<Player>().RpcStartSpawn(choicesP2);
            GameManager.instance.player2.GetComponent<Player>().RpcStartSpawn(choicesP1);
        }
    }

    public void TeamReady()
    {
        int[] characterChoices = new int[gameObject.GetComponent<UIManager>().CurrentCharacters.Count];
        for (int i = 0; i < gameObject.GetComponent<UIManager>().CurrentCharacters.Count; i++)
        {
            characterChoices[i] = gameObject.GetComponent<UIManager>().CurrentCharacters[i];
        }
        gameObject.transform.Find("TeamCanvas").transform.Find("GoTeam").gameObject.SetActive(false);
        gameObject.transform.Find("TeamCanvas").transform.Find("TitleText").GetComponent<Text>().text = "Waiting for other Player";
        CmdTeamReady(characterChoices);
    }

    [ClientRpc]
    public void RpcStartSpawn(int[] otherPlayerChoices)
    {
        GameManager.instance.InitStartGame();
        List<int> numbers = new List<int>(otherPlayerChoices);
        GameManager.instance.GetOtherPlayer(gameObject).GetComponent<UIManager>().CurrentCharacters = numbers;
        GameManager.instance.State = States.Spawn;
        displaySpawn();
    }

    public void GameReady()
    {
        Isready = true;
        Vector3[] positions = new Vector3[Characters.Count];
        int[] ids = new int[Characters.Count];
        for(int i = 0; i < Characters.Count; i++)
        {
            positions[i] = Characters[i].transform.position;
            ids[i] = Characters[i].GetComponent<LivingPlaceable>().netId;
        }
        CmdGameReady(positions, ids);
    }

    [ClientRpc]
    public void RpcEndSwapSpawn(Vector3[] positions, int[] ids)
    {
        if (GameManager.instance.player2.GetComponent<Player>() != this)
        {
            foreach (GameObject c in GameManager.instance.player1.GetComponent<Player>().characters)
            {
                c.SetActive(false);
            }
        }
        if (GameManager.instance.player1.GetComponent<Player>() != this)
        {
            foreach (GameObject c in GameManager.instance.player2.GetComponent<Player>().characters)
            {
                c.SetActive(false);
            }
        }
        SwapPositionSpawn(positions, ids);
    }

    [Command]
    public void CmdGameReady(Vector3[] positions, int[] ids)
    {
        
        SwapPositionSpawn(positions, ids);

        RpcEndSwapSpawn(positions, ids);

        Isready = true;
        if (GameManager.instance.player1.GetComponent<Player>().Isready && GameManager.instance.player2.GetComponent<Player>().Isready)
        {
            RpcEndSpawnAndStartGame();
            GameManager.instance.IsGameStarted = true;
            GameManager.instance.BeginningOfTurn();
        }
    }

    public void Respawn(LivingPlaceable character)
    {
        foreach (Vector3Int spawns in spawnList)
        {
            Placeable spawn = Grid.instance.GridMatrix[spawns.x, spawns.y, spawns.z];
            if (spawn == null)
            {
                Grid.instance.MoveBlock(character, spawns);
                character.gameObject.SetActive(true);
                Vector3 transmit = new Vector3(spawns.x, spawns.y, spawns.z);
                CmdRespawn(transmit, character.netId);
            }
        }
    }

    [Command]
    public void CmdRespawn(Vector3 position, int netID)
    {
        Placeable placeable = GameManager.instance.FindLocalObject(netID);
        Vector3Int pos = new Vector3Int((int) position.x, (int) position.y, (int) position.z);
        Grid.instance.MoveBlock(placeable, pos, true);
        placeable.gameObject.SetActive(true);
        GameManager.instance.GetOtherPlayer(gameObject).GetComponent<Player>().RpcRespawn(position, netID);
    }

    [ClientRpc]
    public void RpcRespawn(Vector3 position, int netID)
    {
        if (isLocalPlayer)
        {
            Placeable placeable = GameManager.instance.FindLocalObject(netID);
            Vector3Int pos = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
            Grid.instance.MoveBlock(placeable, pos, true);
            placeable.gameObject.SetActive(true);
        }
    }

    private void SwapPositionSpawn(Vector3[] positions, int[] ids)
    {
        for (int i = 0; i < positions.Length; i++)
        {

            Vector3Int space = new Vector3Int((int)positions[i].x, (int)positions[i].y, (int)positions[i].z);

            Placeable oldPlac = Grid.instance.GetPlaceableFromVector(space);
            if (oldPlac == null)
            {
                Grid.instance.MoveBlock(GameManager.instance.FindLocalObject(ids[i]), space, true);
            }
            else
            {
                Grid.instance.SwitchPlaceable(GameManager.instance.FindLocalObject(ids[i]), oldPlac);
            }

        }
    }

    [ClientRpc]
    public void RpcEndSpawnAndStartGame()
    {
        // Make the other player's characters visiblr again
        foreach (GameObject c in GameManager.instance.player1.GetComponent<Player>().characters)
        {
            c.SetActive(true);
        }
        foreach (GameObject c in GameManager.instance.player2.GetComponent<Player>().characters)
        {
            c.SetActive(true);
        }

        // Remove the material from the spawning points
        for (int i = 0; i < Grid.instance.SpawnPlayer1.Count; i++)
        {
            Placeable bloc = Grid.instance.GridMatrix[Grid.instance.SpawnPlayer1[i].x, Grid.instance.SpawnPlayer1[i].y - 1,
                Grid.instance.SpawnPlayer1[i].z];
            bloc.GetComponent<MeshRenderer>().material = bloc.baseMaterial;
        }
        for (int i = 0; i < Grid.instance.SpawnPlayer2.Count; i++)
        {
            Placeable bloc = Grid.instance.GridMatrix[Grid.instance.SpawnPlayer2[i].x, Grid.instance.SpawnPlayer2[i].y - 1,
                Grid.instance.SpawnPlayer2[i].z];
            bloc.GetComponent<MeshRenderer>().material = bloc.baseMaterial;
        }

        GameManager.instance.ResetAllBatches();

        // Find the local player and activate the UI;
        GameObject localPlayer;
        if (isLocalPlayer)
        {
            localPlayer = gameObject;
        }else
        {
            localPlayer = GameManager.instance.GetOtherPlayer(gameObject);
        }

        localPlayer.GetComponent<UIManager>().spawnCanvas.SetActive(false);
        localPlayer.GetComponent<UIManager>().gameCanvas.SetActive(true);

        GameManager.instance.IsGameStarted = true;
        GameManager.instance.BeginningOfTurn();
    }

    [ClientRpc]
    public void RpcGivePlayingPlaceable(int netId)
    {
        if(isLocalPlayer)
        {
            GameManager.instance.playingPlaceable = (LivingPlaceable)GameManager.instance.idPlaceable[netId];
        }
    }



    [Command]
    public void CmdReconnectMe()
    {
        Reforge();
        RpcGivePlayingPlaceable(GameManager.instance.playingPlaceable.netId);
        //Reforge server links
        //Reforge on the good player on both clients
        
    }

    private void Update()
    {
        if (isServer && GameManager.instance.player1 != null && GameManager.instance.player2 != null)
        {
            if (clock.IsFinished && GameManager.instance.playingPlaceable && GameManager.instance.playingPlaceable.Player==this)
            {
               RpcEndTurn(); //permet une resynchronisation au rythme server
               GameManager.instance.EndOFTurn();
            }
        }
    }

    [ClientRpc]
    public void RpcEndTurn()
    {
        Debug.Log("Oui chef, mon tour est fini!");
        if (GameManager.instance.playingPlaceable && GameManager.instance.playingPlaceable.Player == this)
        {
            GameManager.instance.EndOFTurn();
        }
        
    }

    //launcher for end of turn
    public void EndTurn()
    {
        Debug.Log("EndTurn asked");
        CmdEndTurn();
    }

    [Command]
    private void CmdEndTurn()
    {
        if (GameManager.instance.playingPlaceable && GameManager.instance.playingPlaceable.Player == this)
        {
            GameManager.instance.EndOFTurn();
            RpcEndTurn();
        }
    }

    // A useless player actually acts, but the timer is unactive and unlinked to nothing on the canvas
    [ClientRpc]
    public void RpcStartTimer(float time)
    {

        clock.StartTimer(time);
        
    }
    public static Skill NumberToSkill(LivingPlaceable living, int skillNumber)
    {
        if (skillNumber < living.Skills.Count)
        {
            return living.Skills[skillNumber];
         }
        else
        {
            int total = living.Skills.Count;
            ///The one from weapon
                if (living.EquipedWeapon && living.EquipedWeapon.Skills!=null && total + living.EquipedWeapon.Skills.Count > skillNumber)
                {
                    return living.EquipedWeapon.Skills[skillNumber - total];
                }
                total += living.EquipedWeapon.Skills.Count;

            
            ///The bloc under
            foreach (ObjectOnBloc obj in living.GetObjectsOnBlockUnder())
            {
                if (total + obj.GivenSkills.Count > skillNumber)
                {
                    return obj.GivenSkills[skillNumber - total];
                }
                total += obj.GivenSkills.Count;

            }
        }
        return null;
    }

    public static int SkillToNumber(LivingPlaceable living, Skill skill)
    {
        int total = living.Skills.FindIndex(skill.Equals);
        if (total != -1)
        {
            return total;
        }
        else
        {
            total = living.Skills.Count;
            if (living.EquipedWeapon && living.EquipedWeapon.Skills != null && living.EquipedWeapon.Skills.FindIndex(skill.Equals)!= -1)
            {
                total += living.EquipedWeapon.Skills.FindIndex(skill.Equals);
                return total;
            }
            total += living.EquipedWeapon.Skills.Count;
            foreach (ObjectOnBloc obj in living.GetObjectsOnBlockUnder())
            {
                if (obj.GivenSkills.FindIndex(skill.Equals) == -1)
                {
                    total += obj.GivenSkills.Count;
                }
                else
                {
                    total += obj.GivenSkills.FindIndex(skill.Equals);
                    return total;
                }
            }
            Debug.LogError("Skill id was not found even in given skills!");
            return -1;
        }
    }

    public void ShowSkillEffectTarget(LivingPlaceable playingPlaceable, Skill skill)
    {

        if (skill.SkillType == SkillType.ALREADYTARGETED)
        {
            if (skill.SkillEffect == SkillEffect.UP) {
                Vector3 Playerpos = playingPlaceable.transform.position;
                if (Playerpos.y+1 < Grid.instance.sizeY && Grid.instance.GridMatrix[(int)Playerpos.x, (int)Playerpos.y + 1, (int)Playerpos.z]==null)
                    CmdUseSkill(SkillToNumber(playingPlaceable, skill), playingPlaceable.netId, new int[0]);
            }
            else CmdUseSkill(SkillToNumber(playingPlaceable, skill), playingPlaceable.netId, new int[0]); //whatever, auto targeted do not go through dispatch
            GameManager.instance.playingPlaceable.ResetAreaOfMovement();//whatever, auto targeted do not go through dispatch
            return;
        }

        RaycastSelector rayselector = GetComponentInChildren<RaycastSelector>();
        rayselector.Pattern = SkillArea.NONE;
        GameManager.instance.playingPlaceable.ResetAreaOfMovement();
        GameManager.instance.playingPlaceable.ResetHighlightSkill();
        GameManager.instance.playingPlaceable.ResetTargets();
        playingPlaceable.ResetAreaOfMovement();
        playingPlaceable.ResetTargets();
        if (skill.SkillType==SkillType.BLOCK || skill.SkillType == SkillType.AREA)
        {
            List<Vector3Int> vect= Grid.instance.HighlightTargetableBlocks(playingPlaceable.transform.position, skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS);

            if (skill.SkillArea == SkillArea.CROSS)
                vect = Grid.instance.DrawCrossPattern(vect, playingPlaceable.transform.position);
            else if (skill.SkillType == SkillType.AREA || skill.SkillArea == SkillArea.THROUGHBLOCKS || skill.SkillArea == SkillArea.TOPBLOCK)
                vect = Grid.instance.TopBlockPattern(vect);

            if(skill.SkillEffect == SkillEffect.DESTROY)
                vect = Grid.instance.DestroyBlockPattern(vect);
            else if (skill.SkillEffect == SkillEffect.CREATE)
                vect = Grid.instance.CreateBlockPattern(vect);
            else if (skill.SkillEffect == SkillEffect.MOVE)
                vect = Grid.instance.PushPattern(vect, playingPlaceable.transform.position);

            foreach (Vector3Int v3 in vect)
            {
                playingPlaceable.TargetArea.Add(Grid.instance.GridMatrix[v3.x, v3.y, v3.z]);
            }

            playingPlaceable.ChangeMaterialAreaOfTarget(GameManager.instance.targetMaterial);
            rayselector.layerMask = LayerMask.GetMask("Placeable");
            rayselector.EffectArea = skill.EffectArea;
            if (skill.SkillArea == SkillArea.LINE)
            {
                rayselector.Pattern = skill.SkillArea;
            }

        }
        else if (skill.SkillType == SkillType.LIVING)
        {
            List<LivingPlaceable> targetableunits = Grid.instance.HighlightTargetableLiving(playingPlaceable.transform.position, skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS);
            if (skill.SkillEffect == SkillEffect.SWORDRANGE)
                targetableunits = Grid.instance.SwordRangePattern(targetableunits, playingPlaceable.transform.position);
            playingPlaceable.TargetableUnits = targetableunits;
            GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("LivingPlaceable");
        }
        else if (skill.SkillType == SkillType.SELF)
        {
            if (skill.SkillArea == SkillArea.SURROUNDINGLIVING)
            {
                List<LivingPlaceable> targetableunits = Grid.instance.HighlightTargetableLiving(playingPlaceable.transform.position, skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS);
                if (skill.SkillEffect == SkillEffect.SPINNING)
                    targetableunits = Grid.instance.SpinningPattern(targetableunits, playingPlaceable.transform.position);
                playingPlaceable.TargetableUnits = targetableunits;
                GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("LivingPlaceable");
            }
        }
        else
        {
            GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("Everything");
            playingPlaceable.GetComponent<Renderer>().material.color = Color.red;
        }

    }


    [Command]
    public void CmdMoveTo(Vector3[] path)
    {
       // Debug.LogError("CheckPath" + Grid.instance.CheckPath(path, GameManager.instance.playingPlaceable));
        if (GameManager.instance.PlayingPlaceable.Player == this && path.Length>1)// updating only if it's his turn to play, other checkings are done in GameManager
        {
            //Move  placeable
            Debug.LogError("Start" + GameManager.instance.playingPlaceable.GetPosition());
            Grid.instance.GridMatrix[GameManager.instance.playingPlaceable.GetPosition().x, GameManager.instance.playingPlaceable.GetPosition().y,
                GameManager.instance.playingPlaceable.GetPosition().z] = null;
            Grid.instance.GridMatrix[(int)path[path.Length - 1].x, (int)path[path.Length - 1].y + 1,
                (int)path[path.Length - 1].z] = GameManager.instance.playingPlaceable;

            GameManager.instance.playingPlaceable.transform.position = path[path.Length - 1] + new Vector3(0, 1, 0);
            //Trigger effect the ones after the others, does not interrupt path
            foreach (Vector3 current in path)
            {
                foreach(Effect effect in Grid.instance.GridMatrix[(int)current.x, (int)current.y, (int)current.z].OnWalkEffects)
                {
                  
                        //makes the deep copy, send it to effect manager and zoo
                        Effect effectToConsider = effect.Clone();
                        effectToConsider.Launcher = Grid.instance.GridMatrix[(int)current.x, (int)current.y, (int)current.z];
                    //Double dispatch
                    GameManager.instance.PlayingPlaceable.DispatchEffect(effectToConsider);

                    
                }
            }
            GameManager.instance.playingPlaceable.CurrentPM -= path.Length - 1;
            RpcMoveTo(path);



        }
    }
    [ClientRpc]
    public void RpcMoveTo(Vector3[] path)
    {
        foreach (Vector3 current in path)
        {
            foreach (Effect effect in Grid.instance.GridMatrix[(int)current.x, (int)current.y, (int)current.z].OnWalkEffects)
            {
                //makes the deep copy, send it to effect manager and zoo
                Effect effectToConsider = effect.Clone();
                effectToConsider.Launcher = Grid.instance.GridMatrix[(int)current.x, (int)current.y, (int)current.z];
                //Double dispatch
                GameManager.instance.PlayingPlaceable.DispatchEffect(effectToConsider);
            }
        }
        //List<Vector3> bezierPath = new List<Vector3>(realPath);
        GameManager.instance.playingPlaceable.CurrentPM -= path.Length - 1;
        List<Vector3> bezierPath=new List<Vector3>(path);

        Debug.Log("CheckPath(): " + Grid.instance.CheckPath(path, GameManager.instance.playingPlaceable));
        Grid.instance.GridMatrix[GameManager.instance.playingPlaceable.GetPosition().x, GameManager.instance.playingPlaceable.GetPosition().y,
             GameManager.instance.playingPlaceable.GetPosition().z] = null;
        Grid.instance.GridMatrix[(int)path[path.Length - 1].x, (int)path[path.Length - 1].y + 1,
            (int)path[path.Length - 1].z] = GameManager.instance.playingPlaceable;

        if (GameManager.instance.playingPlaceable.MoveCoroutine != null)
        {
            GameManager.instance.playingPlaceable.StopCoroutine(GameManager.instance.playingPlaceable.MoveCoroutine);
            GameManager.instance.playingPlaceable.MoveCoroutine = null;

        }
        GameManager.instance.playingPlaceable.MoveCoroutine=StartCoroutine(Player.MoveAlongBezier(bezierPath, GameManager.instance.playingPlaceable, GameManager.instance.playingPlaceable.AnimationSpeed));
        GameManager.instance.MoveLogic(bezierPath);
        
        
    }
    

    [ClientRpc]
    public void RpcSetCamera(int mustPlay)
    {
        Placeable potential = GameManager.instance.FindLocalObject(mustPlay).GetComponent<Placeable>();

        this.cameraScript.target = potential.gameObject.transform;

    }
   
    
    public override void OnStartLocalPlayer()
    {
        transform.Find("Main Camera").gameObject.SetActive(true);
        transform.Find("TeamCanvas").gameObject.SetActive(true);
    }
    
    public void DispatchSkill(int skillID, LivingPlaceable caster, List<NetIdeable> targets)
    {
        Skill skill = caster.Skills[skillID];
        if (skill.Cooldown == 0 && GameManager.instance.PlayingPlaceable == caster)
        {
            GameManager.instance.UseSkill(skillID, caster, targets);
        }
    }


    //MOVING WITH BEZIER

    /// <summary>
    /// See https://www.gamedev.net/forums/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static float LengthQuadraticBezier(Vector3 start, Vector3 end, Vector3 control)
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
    /// Unused Hacky way to approximate bezier length
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
    public static float CalculateDistance(Vector3 start, Vector3 nextNode, ref bool isBezier, ref Vector3 controlPoint)
    {
        isBezier = start.y != nextNode.y;

        if (isBezier)// if difference of height
        {
            controlPoint = (nextNode + start + 2 * new Vector3(0, Mathf.Abs(nextNode.y - start.y), 0)) / 2;

            return LengthQuadraticBezier(start, nextNode, controlPoint);
        }
        else
        {

            return Vector3.Distance(start, nextNode);
        }

    }

    [Client]
    public void StartMoveAlongBezier(List<Vector3> path, Placeable placeable, float speed,bool justLerp=false)
    {
         if(path.Count>1)
        {
            if (placeable.MoveCoroutine!=null)
            {
                placeable.StopCoroutine(placeable.MoveCoroutine);
                placeable.MoveCoroutine = null;

            }
            placeable.MoveCoroutine=StartCoroutine(MoveAlongBezier(path, placeable, speed,justLerp));
         }
    }

    [Client]
    
    // ONLY FOR CHARACTER
    public static IEnumerator MoveAlongBezier(List<Vector3> path, LivingPlaceable placeable, float speed)
    {
        
        if (path.Count < 2)
        {
            yield break;
        }
        if (placeable.isMoving) //teleport to destination if it was already moving
        {
            placeable.transform.position = new Vector3(placeable.destination.x, placeable.destination.y, placeable.destination.z);
        }
        placeable.isMoving = true;
        placeable.destination = new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y + 1, (int)path[path.Count - 1].z);

        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[0];
        Vector3 startPosition = path[0];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;


        Animator anim = placeable.gameObject.GetComponent<Animator>();
        bool isJumping = false;
        
        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;

        int i = 1;
        int iref = 1;
        float distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        float distanceParcourue = 0;
        SoundHandler.Instance.StartWalkSound();
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && i < path.Count - 1) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i++; // changing movement
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == path.Count - 1 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[i] + delta;
                // exit yield loop
                break;
            }

            Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
            placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            //change what we look at
            
            if (isBezier)
            {
                if (!isJumping)
                {
                    isJumping = true;
                    iref = i;
                    anim.Play("jump");
                    SoundHandler.Instance.PauseWalkSound();

                }
                else
                {
                    if (i != iref)
                    {
                        iref = i;
                        anim.Play("land 0");
                        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

                    }
                }
                
                
                placeable.transform.position = delta + Mathf.Pow(1 - timeBezier, 2) * (startPosition) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[i]);

            }
            else
            {
                if (isJumping)
                {
                    isJumping = false;
                    anim.Play("land 1");
                    yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
                    SoundHandler.Instance.StartWalkSound();
                }
                else
                {
                    anim.Play("walking");
                    
                }
                
                placeable.transform.position = Vector3.Lerp(startPosition + delta, path[i] + delta, timeBezier);

            }




            yield return null;

        }
        if (isJumping)
        {
            anim.Play("land 2");
        }
        else
        {
            anim.SetTrigger("idle");
        }
        SoundHandler.Instance.StopWalkSound();
        placeable.isMoving = false;
        //GameManager.instance.playingPlaceable.destination = new Vector3Int();

        Debug.Log("End" + placeable.GetPosition());
   //Debug.Log("End transform" + placeable.transform);
    }
    
    // ONLY FOR OTHER PLACEABLE
    public static IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, float speed,bool justLerp=false)
    {
        if (path.Count < 2)
        {
            yield break;
        }
        if(placeable.isMoving)
        {
            placeable.transform.position = new Vector3(placeable.destination.x, placeable.destination.y, placeable.destination.z);
        }
        placeable.isMoving = true;
        placeable.destination = new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y + 1, (int)path[path.Count - 1].z);

        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[0];
        Vector3 startPosition = path[0];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;//TODO FIX NAN


        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;
        float distance=0;
        int i = 1;
        if(!justLerp)
        { 
        distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);
        }
        else
        {
            isBezier = false;
            distance = Vector3.Distance(startPosition, path[i]);
        }
        float distanceParcourue = 0;
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && i < path.Count - 1) //on go through 
            {


                distanceParcourue -= distance;
                startPosition = path[i];
                i++; // changing movement
                targetDir = path[i] - placeable.transform.position;//next one
                targetDir.y = 0;// don't move up 

                distance = CalculateDistance(startPosition, path[i], ref isBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }
            if (i == path.Count - 1 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[i] + delta;
                // exit yield loop
                break;
            }

            
            if (isBezier)
            {
                placeable.transform.position = delta + Mathf.Pow(1 - timeBezier, 2) * (startPosition) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[i]);

            }
            else
            {
                
                placeable.transform.position = Vector3.Lerp(startPosition + delta, path[i] + delta, timeBezier);

            }
            

            
            yield return null;


        }

        //GameManager.instance.OnEndAnimationEffectEnd();
        Debug.Log("End" + placeable.GetPosition());
        //Debug.Log("End transform" + placeable.transform);
        placeable.isMoving = false;
    }

    /// <summary>
    /// Check if use is possible and send rpc
    /// </summary>
    /// <param name="numSkill"></param>
    /// <param name="netidTarget"></param>
    [Command]
    public void CmdUseSkill(int numSkill, int netidTarget, int[] netidArea)
    {
        Skill skill = NumberToSkill(GameManager.instance.playingPlaceable, numSkill);
        if (skill.SkillType == SkillType.ALREADYTARGETED)
        {
            skill.UseTargeted(skill);
            RpcUseSkill(numSkill, netidTarget, new int[0]);
        }
        else
        {
            NetIdeable target = GameManager.instance.FindLocalObject(netidTarget);
            if (this == GameManager.instance.playingPlaceable.Player)
            {
                Vector3Int Playerpos = GameManager.instance.playingPlaceable.GetPosition();
                Vector3Int Pos = target.GetPosition();
                Vector3Int VectDist = Pos - Playerpos;
                int blockdistance = Math.Abs(VectDist.x) + Math.Abs(VectDist.z) + (VectDist.y == -1 ? 0 : Math.Abs(VectDist.y));
                bool blockallowed = false;

                if (blockdistance <= skill.Maxrange && blockdistance >= skill.Minrange)
                {
                    if (skill.SkillArea == SkillArea.THROUGHBLOCKS || !Grid.instance.RayCastBlock(VectDist.x, VectDist.y, VectDist.z,
                        VectDist.x == 0 ? 0 : VectDist.x / Math.Abs(VectDist.x), VectDist.y == 0 ? 0 : VectDist.y / Math.Abs(VectDist.y),
                        VectDist.z == 0 ? 0 : VectDist.z / Math.Abs(VectDist.z), GameManager.instance.playingPlaceable.GetPosition()))
                    {
                        if (skill.SkillArea == SkillArea.CROSS)
                        {
                            if (VectDist.y == 0 && (VectDist.x == 0 || VectDist.z == 0))
                                blockallowed = true;
                        }
                        else if (skill.SkillType == SkillType.AREA || skill.SkillArea == SkillArea.THROUGHBLOCKS || skill.SkillArea == SkillArea.TOPBLOCK)
                        {
                            if (Pos.y != Grid.instance.sizeY - 1 && Grid.instance.GridMatrix[Pos.x, Pos.y + 1, Pos.z] == null)
                                blockallowed = true;
                        }
                        else blockallowed = true;

                        if (blockallowed)
                        {
                            if (skill.SkillEffect == SkillEffect.DESTROY)
                            {
                                if (!Grid.instance.GridMatrix[Pos.x, Pos.y, Pos.z].Destroyable)
                                    blockallowed = false;
                            }
                            else if (skill.SkillEffect == SkillEffect.CREATE)
                            {
                                Placeable block = Grid.instance.GridMatrix[Pos.x, Pos.y, Pos.z];
                                if (block.GetType() == typeof(Goal) || block.GetType() == typeof(Spawn))
                                    blockallowed = false;
                            }
                            else if (skill.SkillEffect == SkillEffect.MOVE)
                            {
                                if (!Grid.instance.GridMatrix[Pos.x, Pos.y, Pos.z].Movable)
                                    blockallowed = false;
                                else
                                {
                                    if (Math.Abs((int)Playerpos.x - Pos.x) - Math.Abs((int)Playerpos.z - Pos.z) > 0)
                                    {
                                        int direction = (Pos.x - (int)Playerpos.x) / Math.Abs((int)Playerpos.x - Pos.x);
                                        if (Pos.x + direction < 0 || Pos.x + direction >= Grid.instance.sizeX || Grid.instance.GridMatrix[Pos.x + direction, Pos.y, Pos.z] != null)
                                            blockallowed = false;
                                    }
                                    else
                                    {
                                        int direction = (Pos.z - (int)Playerpos.z) / Math.Abs((int)Playerpos.z - Pos.z);
                                        if (Pos.z + direction < 0 || Pos.z + direction >= Grid.instance.sizeZ || Grid.instance.GridMatrix[Pos.x, Pos.y, Pos.z + direction] != null)
                                            blockallowed = false;
                                    }
                                }
                            }
                        }
                    }
                }

                Debug.Log(blockallowed);
                if (blockallowed)
                {
                    if (netidArea.Length == 0)
                    {
                        skill.Use(GameManager.instance.playingPlaceable, new List<NetIdeable>() { target });
                        RpcUseSkill(numSkill, netidTarget, new int[0]);
                    }
                    else
                    {
                        List<NetIdeable> idlist = new List<NetIdeable>();
                        foreach (int blockid in netidArea)
                            idlist.Add(GameManager.instance.FindLocalObject(blockid));
                        skill.Use(GameManager.instance.playingPlaceable, idlist);
                        RpcUseSkill(numSkill, netidTarget, netidArea);
                    }

                }

            }
        }
    }

    public void UseTargeted(Skill skill)
    {
        if (skill.SkillType == SkillType.ALREADYTARGETED) //Simply use them
        {
            foreach (Effect eff in skill.effects)
            {
                Effect effectToConsider = eff.Clone();
                effectToConsider.Launcher = GameManager.instance.playingPlaceable;
                effectToConsider.Use();
            }
        }
    }
  
    [ClientRpc]
    public void RpcUseSkill(int numSkill, int netidTarget, int[] netidArea)
    {
        Skill skill = NumberToSkill(GameManager.instance.playingPlaceable, numSkill);
        if (skill.SkillType == SkillType.ALREADYTARGETED) //Simply use them
        {
            skill.UseTargeted(skill);
            GameManager.instance.playingPlaceable.ResetTargets();

        }
        else
        {
            NetIdeable target = GameManager.instance.FindLocalObject(netidTarget);
            Debug.Log("Netid is" + netidTarget + "and target is at" + target.GetPosition());
            GameManager.instance.playingPlaceable.ResetTargets();

            if (netidArea.Length == 0)
                skill.Use(GameManager.instance.playingPlaceable, new List<NetIdeable>() { target });
            else
            {
                List<NetIdeable> idlist = new List<NetIdeable>();
                foreach (int blockid in netidArea)
                    idlist.Add(GameManager.instance.FindLocalObject(blockid));
                skill.Use(GameManager.instance.playingPlaceable, idlist);
            }

            if (GetComponentInChildren<RaycastSelector>() != null)
            {
                GetComponentInChildren<RaycastSelector>().EffectArea = 0;
                GetComponentInChildren<RaycastSelector>().Pattern = SkillArea.NONE;
            }
        }
    }

}
