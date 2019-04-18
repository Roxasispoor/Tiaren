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
    public List<LivingPlaceable> characters = new List<LivingPlaceable>();
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

    public List<LivingPlaceable> Characters
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
        if (GameManager.instance.player1 == null && GameManager.instance.player1Username == "")
        {
            GameManager.instance.player1 = gameObject;
            Debug.LogError("player1 is " + username);
            GameManager.instance.player1Username = username;
        }
        else if (GameManager.instance.player2 == null && GameManager.instance.player2Username == "")
        {

            GameManager.instance.player2 = gameObject;
            Debug.LogError("player2 is " + username);
            GameManager.instance.player2Username = username;
        }
        else if (GameManager.instance.player1 == null && GameManager.instance.player1Username == username)
        {
            Debug.LogError("Player1 reconnect!");

            Debug.LogError("Transmitter started");
            StartCoroutine(GameManager.instance.transmitter.AcceptTcp());
            RpcListen(GameManager.instance.State);


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
            RpcListen(GameManager.instance.State);
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
        if (!isLocalPlayer)
        {
            Reforge();
        }
    }
    public void Reforge()
    {
        Debug.LogError("p1 is" + GameManager.instance.player1 + ". Is it me?" + (GameManager.instance.player1 == gameObject));
        Debug.LogError("p2 is" + GameManager.instance.player2 + ". Is it me?" + (GameManager.instance.player2 == gameObject));
        if (GameManager.instance.player1 == null)
        {
            GameManager.instance.player1 = gameObject;
            Debug.LogError("Player 1 reconnects, trying to reforge links");
            if (isServer)
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
                Characters.Add(living);
            }
        }
    }

    [ClientRpc]
    public void RpcListen(States state)
    {
        if (isLocalPlayer)
        {
            if (state == States.Move || state == States.UseSkill)
            {
                GameManager.instance.State = state;
                Debug.LogError("Client listen to data start coroutine");

                StartCoroutine(GameManager.instance.transmitter.ListenToData(this));
            }

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
        DicoCondition.Add("SwitchCameraType", () => Input.GetButtonDown("SwitchCameraType"));
        DicoCondition.Add("OrbitCamera", () => Input.GetMouseButton(1));
        DicoCondition.Add("PanCamera", () => Input.GetMouseButton(2));
        Isready = false;
    }

    // Use this for initialization
    private void Start()
    {
        if (isLocalPlayer)
        {
            FloatingTextController.Initialize(gameObject.transform.Find("InGameCanvas").gameObject, cameraScript.GetComponent<Camera>());
            if (GameManager.instance.State == States.TeamSelect)
            {
                GameObject firstCanva = gameObject.transform.Find("TeamCanvas").gameObject;
                firstCanva.SetActive(true);
                firstCanva.transform.Find("TitleText").GetComponent<Text>().text = "Waiting for other player";
            }
            account = FindObjectOfType<PlayerAccount>();
            if (account != null && account.AccountInfoPacket != null && account.AccountInfoPacket.Username != null)
            {
                CmdSetName(account.AccountInfoPacket.Username);
            }
            else
            {
                CmdSetName("Patate");
            }

        }
    }

    /// <summary>
    /// Called by the server to set this player as player 1.
    /// </summary>
    [ClientRpc]
    public void RpcSetPlayer1()
    {
        GameManager.instance.player1 = gameObject;
    }

    /// <summary>
    /// Called by the server to set this player as player 2.
    /// </summary>
    [ClientRpc]
    public void RpcSetPlayer2()
    {
        GameManager.instance.player2 = gameObject;
    }

    public void SendSpawnToCamera()
    {
        Vector3 spawncenter = new Vector3(0, 0, 0);
        foreach (Vector3Int point in spawnList)
        {
            spawncenter += point;
        }

        cameraScript.SpawnCenter = spawncenter / spawnList.Count;
        cameraScript.Init();
    }

    public void spawnCharacters()
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<UIManager>().SpawnUI();

            Player localPlayer = GameManager.instance.GetLocalPlayer();
            Player enemyPlayer = GameManager.instance.GetOtherPlayer(localPlayer.gameObject).GetComponent<Player>();

            List<int> localPlayerCharacters = localPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters;
            List<int> enemyPlayerCharacters = enemyPlayer.gameObject.GetComponent<UIManager>().CurrentCharacters;

            if (GameManager.instance.Player1 == localPlayer)    // Player need to always spawn in the same order for the NetID
            {
                for (int i = 0; i < localPlayerCharacters.Count; i++)
                {
                    GameManager.instance.CreateCharacter(localPlayer.gameObject, localPlayer.spawnList[i], localPlayerCharacters[i]);
                }

                for (int i = 0; i < enemyPlayerCharacters.Count; i++)
                {
                    GameManager.instance.CreateCharacter(enemyPlayer.gameObject, enemyPlayer.spawnList[i], enemyPlayerCharacters[i]);
                }
            }
            else
            {

                for (int i = 0; i < enemyPlayerCharacters.Count; i++)
                {
                    GameManager.instance.CreateCharacter(enemyPlayer.gameObject, enemyPlayer.spawnList[i], enemyPlayerCharacters[i]);
                }

                for (int i = 0; i < localPlayerCharacters.Count; i++)
                {
                    GameManager.instance.CreateCharacter(localPlayer.gameObject, localPlayer.spawnList[i], localPlayerCharacters[i]);
                }
            }

            GameManager.instance.ResetAllBatches();
            gameObject.GetComponent<UIManager>().SpawnUI();
        }
    }

    [Command]
    public void CmdTeamReady(int[] characterChoices)
    {
        Isready = true;
        List<int> numbers = new List<int>(characterChoices);
        gameObject.GetComponent<UIManager>().CurrentCharacters = numbers;

        //GameManager.instance.InitStartGame();

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

            StandardCube spawnpoint;
            // Spawn the characters
            for (int i = 0; i < Grid.instance.SpawnPlayer1.Count; i++)
            {
                spawnpoint = (StandardCube)Grid.instance.GetPlaceableFromVector(Grid.instance.SpawnPlayer1[i] + Vector3.down);
                spawnpoint.isSpawnPoint = true;
                spawnpoint.Destroyable = false;
                spawnpoint.movable = false;
                spawnpoint.gravityType = GravityType.NULL_GRAVITY;
                if (i < GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters.Count)
                {
                    GameManager.instance.CreateCharacter(GameManager.instance.player1, Grid.instance.SpawnPlayer1[i], GameManager.instance.player1.GetComponent<UIManager>().CurrentCharacters[i]);
                }
            }
            for (int i = 0; i < Grid.instance.SpawnPlayer2.Count; i++)
            {
                spawnpoint = (StandardCube)Grid.instance.GetPlaceableFromVector(Grid.instance.SpawnPlayer2[i] + Vector3.down);
                spawnpoint.isSpawnPoint = true;
                spawnpoint.Destroyable = false;
                spawnpoint.movable = false;
                spawnpoint.gravityType = GravityType.NULL_GRAVITY;
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
        Button[] buttons = gameObject.transform.Find("TeamCanvas").transform.GetComponentsInChildren<Button>();
        foreach (Button but in buttons)
        {
            Destroy(but.gameObject);
        }
        CmdTeamReady(characterChoices);
    }

    [ClientRpc]
    public void RpcStartSpawn(int[] otherPlayerChoices)
    {
        List<int> numbers = new List<int>(otherPlayerChoices);
        GameManager.instance.GetOtherPlayer(gameObject).GetComponent<UIManager>().CurrentCharacters = numbers;
        GameManager.instance.State = States.Spawn;
        spawnCharacters();
    }

    public void GameReady()
    {
        Isready = true;
        Vector3[] positions = new Vector3[Characters.Count];
        int[] ids = new int[Characters.Count];
        for (int i = 0; i < Characters.Count; i++)
        {
            positions[i] = Characters[i].transform.position;
            ids[i] = Characters[i].GetComponent<LivingPlaceable>().netId;
        }
        CmdGameReady(positions, ids);
    }

    [ClientRpc]
    public void RpcEndSwapSpawn(Vector3[] positions, int[] ids)
    {
        if (GameManager.instance.player2.GetComponent<Player>() != this && !this.isLocalPlayer)
        {
            foreach (LivingPlaceable c in GameManager.instance.player1.GetComponent<Player>().characters)
            {
                c.gameObject.SetActive(false);
            }
        }
        if (GameManager.instance.player1.GetComponent<Player>() != this && !this.isLocalPlayer)
        {
            foreach (LivingPlaceable c in GameManager.instance.player2.GetComponent<Player>().characters)
            {
                c.gameObject.SetActive(false);
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
            GameManager.instance.InitStartGameServer();
            RpcEndSpawnAndStartGame();
            GameManager.instance.isGameStarted = true;
            GameManager.instance.TransitBetweenTurn();
        }
    }

    public void Respawn(LivingPlaceable character)
    {
        foreach (Vector3Int spawns in spawnList)
        {
            Placeable spawn = Grid.instance.GridMatrix[spawns.x, spawns.y, spawns.z];
            if (spawn == null)
            {
                Grid.instance.MovePlaceable(character, spawns);
                character.gameObject.SetActive(true);
                Vector3 transmit = new Vector3(spawns.x, spawns.y, spawns.z);
                CmdRespawn(transmit, character.netId);
            }
        }
        GameManager.instance.PlayingPlaceable.CurrentHP = GameManager.instance.PlayingPlaceable.MaxHP;
    }

    [Command]
    public void CmdRespawn(Vector3 position, int netID)
    {
        Placeable placeable = GameManager.instance.FindLocalObject(netID);
        Vector3Int pos = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        Grid.instance.MovePlaceable(placeable, pos, true);
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
            Grid.instance.MovePlaceable(placeable, pos, true);
            placeable.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// To call to update the speed with the bias to avoid conflicting speed (the same)
    /// </summary>
    /// <param name="biases"></param>
    [ClientRpc]
    public void RpcBiasSpeed(float[] biases)
    {
        BiasSpeed(biases);
    }

    /// <summary>
    /// To call to update the speed with the bias to avoid conflicting speed (the same)
    /// </summary>
    /// <param name="biases"></param>
    public void BiasSpeed(float[] biases)
    {
        int i = 0;
        foreach (LivingPlaceable chara in Characters)
        {
            chara.SpeedStack += biases[i];
            i++;
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
                Grid.instance.MovePlaceable(GameManager.instance.FindLocalObject(ids[i]), space, true);
            }
            else
            {
                Grid.instance.SwitchPlaceable(GameManager.instance.FindLocalObject(ids[i]), oldPlac);
            }

        }
    }

    public void ChangeUi()
    {
        Debug.Log("Try to change UI");
        // Make the other player's characters visible again
        foreach (LivingPlaceable c in GameManager.instance.player1.GetComponent<Player>().characters)
        {
            c.gameObject.SetActive(true);
        }
        foreach (LivingPlaceable c in GameManager.instance.player2.GetComponent<Player>().characters)
        {
            c.gameObject.SetActive(true);
        }
        
        // Remove the material from the spawning points
        for (int i = 0; i < Grid.instance.SpawnPlayer1.Count; i++)
        {
            Placeable bloc = Grid.instance.GridMatrix[Grid.instance.SpawnPlayer1[i].x, Grid.instance.SpawnPlayer1[i].y - 1,
                Grid.instance.SpawnPlayer1[i].z];
            bloc.GetComponent<MeshRenderer>().material = bloc.oldMaterial;
        }
        for (int i = 0; i < Grid.instance.SpawnPlayer2.Count; i++)
        {
            Placeable bloc = Grid.instance.GridMatrix[Grid.instance.SpawnPlayer2[i].x, Grid.instance.SpawnPlayer2[i].y - 1,
                Grid.instance.SpawnPlayer2[i].z];
            bloc.GetComponent<MeshRenderer>().material = bloc.oldMaterial;
        }

        GameManager.instance.ResetAllBatches();

        // Find the local player and activate the UI;
        GameObject localPlayer;
        if (isLocalPlayer)
        {
            localPlayer = gameObject;
        }
        else
        {
            localPlayer = GameManager.instance.GetOtherPlayer(gameObject);
        }
        localPlayer.GetComponent<UIManager>().InitAbilities(GameManager.instance.GetLocalPlayer());
        localPlayer.GetComponent<UIManager>().spawnCanvas.SetActive(false);
        localPlayer.GetComponent<UIManager>().teamCanvas.SetActive(false); //just making sure and useful for reco
        localPlayer.GetComponent<UIManager>().gameCanvas.SetActive(true);

        GameManager.instance.isGameStarted = true;
    }


    [ClientRpc]
    public void RpcEndSpawnAndStartGame()
    {
        ChangeUi();
        GameManager.instance.TransitBetweenTurn();
    }

    [ClientRpc]
    public void RpcGivePlayingPlaceable(int netId)
    {
        if (isLocalPlayer)
        {
            GameManager.instance.PlayingPlaceable = (LivingPlaceable)GameManager.instance.idPlaceable[netId];
            cameraScript.BackToMovement();
        }
    }



    [Command]
    public void CmdReconnectMe()
    {
        Reforge();
        if (GameManager.instance.PlayingPlaceable)
        {
            RpcGivePlayingPlaceable(GameManager.instance.PlayingPlaceable.netId);
        }
        RpcEndReco();
        //Reforge server links
        //Reforge on the good player on both clients

    }
    [ClientRpc]
    public void RpcEndReco()
    {
        ChangeUi();
        GameManager.instance.UpdateTimeline();
        GameManager.instance.SetCamera();
        GameManager.instance.PlayingPlaceable.Player.cameraScript.BackToMovement();

    }

    private void Update()
    {
        if (isServer && GameManager.instance.player1 != null && GameManager.instance.player2 != null)
        {
            if (clock.IsStarted && clock.IsFinished && GameManager.instance.PlayingPlaceable && GameManager.instance.PlayingPlaceable.Player == this)
            {
                RpcEndTurn(GameManager.instance.PlayingPlaceable.netId); //permet une resynchronisation au rythme server
                clock.IsStarted = false;
                GameManager.instance.EndOFTurn();
            }
        }
    }

    [ClientRpc]
    public void RpcEndTurn(int netIdAskingChar)
    {
        if (null == GameManager.instance.PlayingPlaceable)
            return;
        LivingPlaceable askingPlaceable = CheckAskingTurn(netIdAskingChar);
        if (null == askingPlaceable)
            return;
        GameManager.instance.EndOFTurn();
    }

    //launcher for end of turn
    public void EndTurn()
    {
        Debug.Log("EndTurn asked");
        CmdEndTurn(GameManager.instance.PlayingPlaceable.netId);
    }

    [Command]
    private void CmdEndTurn(int netIdAskingChar)
    {
        if (null == GameManager.instance.PlayingPlaceable)
            return;
        LivingPlaceable askingPlaceable = CheckAskingTurn(netIdAskingChar);
        if (null == askingPlaceable)
            return;

        GameManager.instance.EndOFTurn();
        RpcEndTurn(netIdAskingChar);
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
        Debug.LogError("Skill number not found");
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
    /*
    public void ShowSkillEffectTarget(LivingPlaceable playingPlaceable, Skill skill)
    {
        
        if (skill.TargetType == TargetType.ALREADYTARGETED)
        {
            Vector3 Playerpos = playingPlaceable.GetPosition();
            if (skill.SkillEffect == SkillEffect.UP)
            {
                if (Playerpos.y + 1 < Grid.instance.sizeY && Grid.instance.GridMatrix[(int)Playerpos.x, (int)Playerpos.y + 1, (int)Playerpos.z] == null)
                {
                    OnUseSkill(SkillToNumber(playingPlaceable, skill), playingPlaceable.netId, new int[0], 0);
                }
            }
            else
            {
                OnUseSkill(SkillToNumber(playingPlaceable, skill), playingPlaceable.netId, new int[0], 0); //whatever, auto targeted do not go through dispatch
            }
            return;
        }

        RaycastSelector rayselector = GetComponentInChildren<RaycastSelector>();
        rayselector.Pattern = SkillArea.NONE;
        rayselector.EffectArea = 0;
        playingPlaceable.ResetHighlightSkill();
        playingPlaceable.ResetAreaOfMovement();
        playingPlaceable.ResetTargets();
        if (skill.TargetType == TargetType.BLOCK || skill.TargetType == TargetType.AREA)
        {
            List<Vector3Int> vect = Grid.instance.HighlightTargetableBlocks(playingPlaceable.GetPosition(), skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS, skill.Minrange > 0);

            if (skill.SkillArea == SkillArea.CROSS)
            {
                vect = Grid.instance.DrawCrossPattern(vect, playingPlaceable.GetPosition());
            }
            else if (skill.TargetType == TargetType.AREA || skill.SkillArea == SkillArea.THROUGHBLOCKS || skill.SkillArea == SkillArea.TOPBLOCK)
            {
                vect = Grid.instance.TopBlockPattern(vect);
            }

            if (skill.SkillEffect == SkillEffect.DESTROY)
            {
                vect = Grid.instance.DestroyBlockPattern(vect);
            }
            else if (skill.SkillEffect == SkillEffect.CREATE)
            {
                vect = Grid.instance.CreateBlockPattern(vect);
            }
            else if (skill.SkillEffect == SkillEffect.MOVE)
            {
                vect = Grid.instance.PushPattern(vect, playingPlaceable.GetPosition());
            }

            foreach (Vector3Int v3 in vect)
            {
                playingPlaceable.TargetArea.Add(Grid.instance.GridMatrix[v3.x, v3.y, v3.z]);
            }

            playingPlaceable.ChangeMaterialAreaOfTarget(GameManager.instance.targetMaterial);
            rayselector.layerMask = LayerMask.GetMask("Placeable");
            rayselector.EffectArea = skill.EffectArea;
            if (skill.SkillArea == SkillArea.LINE || skill.SkillArea == SkillArea.MIXEDAREA)
            {
                rayselector.Pattern = skill.SkillArea;
            }

        }
        else if (skill.TargetType == TargetType.HURTABLE)
        {
            List<LivingPlaceable> targetableunits = new List<LivingPlaceable>();
            if (skill.SkillEffect == SkillEffect.SWORDRANGE)
            {
                targetableunits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS, skill.Minrange > 0);
                targetableunits = Grid.instance.SwordRangePattern(targetableunits, playingPlaceable.GetPosition());
            }
            else if (skill.SkillEffect == SkillEffect.SPINNING)
            {
                targetableunits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS, false);
                targetableunits = Grid.instance.SpinningPattern(targetableunits, playingPlaceable.GetPosition());
            }
            else
            {
                targetableunits = Grid.instance.HighlightTargetableLiving(playingPlaceable.GetPosition(), skill.Minrange, skill.Maxrange, skill.SkillArea == SkillArea.THROUGHBLOCKS, false);
            }

            playingPlaceable.TargetableUnits = targetableunits;
            GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("LivingPlaceable");
        }
        else
        {
            GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("Everything");
            playingPlaceable.GetComponent<Renderer>().material.color = Color.red;
        }

    }
    */
    public LivingPlaceable CheckAskingTurn(int netIdAskingChar)
    {
        LivingPlaceable askingPlaceable = (LivingPlaceable)GameManager.instance.FindLocalIdeable(netIdAskingChar);
        if (null == askingPlaceable || askingPlaceable != GameManager.instance.PlayingPlaceable)
        {
            Debug.LogError(netIdAskingChar + "wasn't found");
            return null;
        }
        return askingPlaceable;
    }
    /// <summary>
    /// Command to move on server. Checks path and whether it's the turn of the asking character
    /// </summary>
    /// <param name="path"></param>
    /// <param name="netIdAskingChar"></param>
    [Command]
    public void CmdMoveTo(Vector3[] path, int netIdAskingChar)
    {
        if (path.Length <= 1)
        {
            Debug.LogError("path is too short");
            return;
        }
        LivingPlaceable askingPlaceable = CheckAskingTurn(netIdAskingChar);
        if (null == askingPlaceable)
            return;
        if (Grid.instance.CheckPath(path, askingPlaceable))
        {
            List<Vector3> finalPath = Grid.instance.CheckPathForEffect(path, askingPlaceable);

            Move(finalPath.ToArray(), askingPlaceable, true);
            EffectManager.instance.EffectsOnBlock((StandardCube)Grid.instance.GetPlaceableFromVector(path[path.Length - 1]), askingPlaceable);

            RpcMoveTo(finalPath.ToArray(), netIdAskingChar);
        }
        else
        {
            Debug.LogError("Path is invalid");
        }
    }

    // TODO: rename et voir si besoin de refactor
    public void Move(Vector3[] path, LivingPlaceable askingPlaceable,bool mustUpdateTransform)
    {
        Grid.instance.MovePlaceable(askingPlaceable, Vector3Int.FloorToInt(path[path.Length - 1]) + Vector3Int.up, mustUpdateTransform);

        if(mustUpdateTransform)
            askingPlaceable.transform.position = path[path.Length - 1] + new Vector3(0, 1, 0);
        
        askingPlaceable.CurrentPM -= path.Length - 1;
    }
    /// <summary>
    /// Moves and manages the coroutine on client
    /// </summary>
    /// <param name="path"></param>
    /// <param name="netIdAskingChar"></param>
    [ClientRpc]
    public void RpcMoveTo(Vector3[] path, int netIdAskingChar)
    {
        LivingPlaceable askingPlaceable = (LivingPlaceable)GameManager.instance.FindLocalIdeable(netIdAskingChar);
        if (null == askingPlaceable || askingPlaceable != GameManager.instance.PlayingPlaceable)
        {
            Debug.LogError(netIdAskingChar + "wasn't found in rpcMoveTo");
            return;
        }
        List<Vector3> bezierPath = new List<Vector3>(path);
        
        Move(path,askingPlaceable,false);

        EffectManager.instance.EffectsOnBlock((StandardCube)Grid.instance.GetPlaceableFromVector(askingPlaceable.GetPosition() + Vector3.down), askingPlaceable);

        for (int i = 0; i < bezierPath.Count; i++)
        {
            bezierPath[i] += Vector3.up;
        }

        FollowPathAnimation(bezierPath, askingPlaceable, animator: askingPlaceable.GetComponent<Animator>(), 3f);
        /*
        if (askingPlaceable.moveCoroutine != null)
        {
            askingPlaceable.StopCoroutine(askingPlaceable.moveCoroutine);
            askingPlaceable.moveCoroutine = null;

        }
        askingPlaceable.moveCoroutine = StartCoroutine(Player.MoveAlongBezier(bezierPath, askingPlaceable, askingPlaceable.AnimationSpeed));
        */
        GameManager.instance.MoveLogic(bezierPath);
    }


    public override void OnStartLocalPlayer()
    {
        transform.Find("Main Camera").gameObject.SetActive(true);
        transform.Find("TeamCanvas").gameObject.SetActive(true);
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
    public static float CalculateDistance(Vector3 start, Vector3 nextNode, bool useCurve, out bool isBezier, ref Vector3 controlPoint)
    {
        isBezier = start.y != nextNode.y;

        if (isBezier && useCurve)// if difference of height
        {
            controlPoint = (nextNode + start + 2 * new Vector3(0, Mathf.Abs(nextNode.y - start.y), 0)) / 2;

            return LengthQuadraticBezier(start, nextNode, controlPoint);
        }
        else
        {
            return Vector3.Distance(start, nextNode);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="placeable"></param>
    /// <param name="animator"></param>
    /// <param name="speed"></param>
    /// <param name="useCurve"></param>
    [Client]
    public void FollowPathAnimation(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = true)
    {
        if (path.Count > 1)
        {
            if (placeable.moveCoroutine != null)
            {
                placeable.StopCoroutine(placeable.moveCoroutine);
                placeable.moveCoroutine = null;

            }
            placeable.moveCoroutine = StartCoroutine(MoveAlongBezier(path, placeable, speed: speed, animator: animator, useCurve: useCurve));

        }
    }

    /// <summary> // TODO: use the transform of the 3D model rather than the placeable transform
    /// Make the transform follow the path given.
    /// </summary>
    /// <param name="path">Path to follow</param>
    /// <param name="placeable">Transform to move</param>
    /// <param name="animator">To set if animation should be played</param>
    /// <param name="speed"></param>
    /// <param name="useBezier">If false, use bezier curve</param>
    /// <returns></returns>
    [Client]
    private static IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, Animator animator = null, float speed = 4f, bool useCurve = false)
    {
        if (path.Count < 2)
        {
            yield break;
        }

        placeable.isMoving = true;
        
        float timeBezier = 0f;
        Vector3 previousCheckpoint = path[0];
        Vector3 controlPoint = new Vector3();

        bool isJumping = false;

        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;

        bool useBezier;

        int stepInPath = 1;
        int stepRef = 1;
        float distance = CalculateDistance(previousCheckpoint, path[stepInPath], useCurve, out useBezier, ref controlPoint);
        float distanceParcourue = 0;
        SoundHandler.Instance.StartWalkSound();
        while (timeBezier < 1)
        {
            distanceParcourue += (speed * Time.deltaTime);
            timeBezier = distanceParcourue / distance;

            while (timeBezier > 1 && stepInPath < path.Count - 1) // If we overstep the next checkpoint, compute to the following which is not overstep
            {


                distanceParcourue -= distance;
                previousCheckpoint = path[stepInPath];
                stepInPath++; 

                distance = CalculateDistance(previousCheckpoint, path[stepInPath], useCurve, out useBezier, ref controlPoint);//On calcule la distance au noeud suivant
                timeBezier = distanceParcourue / distance; //on recalcule

            }

            if (stepInPath == path.Count - 1 && timeBezier > 1)
            {
                // arrived to the last node of path, in precedent loop
                placeable.transform.position = path[stepInPath];
                // exit yield loop
                break;
            }

            if (placeable.IsLiving())
            {
                targetDir = path[stepInPath] - placeable.transform.position;//next one
                
                targetDir.y = 0;// don't move up 
                Vector3 vectorRotation = Vector3.RotateTowards(placeable.transform.forward, targetDir, 0.2f, 0);
                placeable.transform.rotation = Quaternion.LookRotation(vectorRotation);
            }

            //change what we look at

            if (useBezier && useCurve)
            {
                if (animator != null)
                {
                    if (!isJumping)
                    {
                        isJumping = true;
                        stepRef = stepInPath;
                        animator.Play("jump");
                        SoundHandler.Instance.PauseWalkSound();

                    }
                    else
                    {
                        if (stepInPath != stepRef)
                        {
                            stepRef = stepInPath;
                            animator.Play("land 0");
                            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

                        }
                    }
                }

                placeable.transform.position = Mathf.Pow(1 - timeBezier, 2) * (previousCheckpoint) + 2 * (1 - timeBezier) * timeBezier * (controlPoint) + Mathf.Pow(timeBezier, 2) * (path[stepInPath]);

            }
            else
            {
                if (animator != null)
                {
                    if (isJumping)
                    {
                        isJumping = false;
                        animator.Play("land 1");
                        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                        SoundHandler.Instance.StartWalkSound();
                    }
                    else
                    {
                        animator.Play("walking");

                    }
                }

                Vector3 next = Vector3.Lerp(previousCheckpoint, path[stepInPath], timeBezier);
                

                placeable.transform.position = next;

            }




            yield return null;

        }
        if (animator != null)
        {
            if (isJumping)
            {
                animator.Play("land 2");
            }
            else
            {
                animator.SetTrigger("idle");
            }

        }
        SoundHandler.Instance.StopWalkSound();
        placeable.isMoving = false;

        Debug.Log("End" + placeable.GetPosition());
    }

    /*
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
        placeable.destination = new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z);

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
                        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

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
                    yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
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
        //GameManager.instance.PlayingPlaceable.destination = new Vector3Int();

        Debug.Log("End" + placeable.GetPosition());
        //Debug.Log("End transform" + placeable.transform);
    }

    // ONLY FOR OTHER PLACEABLE
    public static IEnumerator MoveAlongBezier(List<Vector3> path, Placeable placeable, float speed, bool justLerp = false)
    {
        if (path.Count < 2)
        {
            yield break;
        }
        if (placeable.isMoving)
        {
            placeable.transform.position = new Vector3(placeable.destination.x, placeable.destination.y, placeable.destination.z);
        }
        placeable.isMoving = true;
        placeable.destination = new Vector3Int((int)path[path.Count - 1].x, (int)path[path.Count - 1].y, (int)path[path.Count - 1].z);

        float timeBezier = 0f;
        Vector3 delta = placeable.transform.position - path[0];
        Vector3 startPosition = path[0];
        Vector3 controlPoint = new Vector3();
        bool isBezier = true;//TODO FIX NAN


        //For visual rotation
        Vector3 targetDir = path[1] - placeable.transform.position;
        targetDir.y = 0;
        float distance = 0;
        int i = 1;
        if (!justLerp)
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
    */
    
    /// <summary>
    /// Check if use is possible (server side) and send rpc
    /// </summary>
    /// <param name="numSkill"></param>
    /// <param name="infoSelection"></param>
    /// <param name="netIdAskingChar"></param>
    [Command]
    public void CmdUseSkill(int numSkill, int[] infoSelection, int netIdAskingChar)
    {
        LivingPlaceable askingPlaceable = CheckAskingTurn(netIdAskingChar);
        if (null == askingPlaceable)
            return;
        Skill skill = NumberToSkill(askingPlaceable, numSkill);
        if (null == skill)
            return;
        if (askingPlaceable.CurrentPA < skill.Cost)
            return;

        SelectionInfo newSelection = new SelectionInfo();
        newSelection.FillFromNetwork(infoSelection);

        GameManager.instance.currentSelection = newSelection;
        
        bool wasSuccessfulyCast = skill.Use(askingPlaceable, newSelection.placeable);

        if (wasSuccessfulyCast)
        {
            RpcUseSkill(numSkill, infoSelection, netIdAskingChar);
        }
         
    }

    /// <summary>
    /// Local check of PA to notify player
    /// </summary>
    /// <param name="numSkill"></param>
    /// <param name="netidTarget"></param>
    /// <param name="netidArea"></param>
    /// <param name="state"></param>
    public void OnUseSkill(int numSkill, int netidTarget)
    {
        Skill skill = NumberToSkill(GameManager.instance.PlayingPlaceable, numSkill);
        if (GameManager.instance.PlayingPlaceable.CurrentPA >= skill.Cost)
        {
            int[] selectionToTransmit = GameManager.instance.RaycastSelector.CurrentHovered.ConvertForNetwork();
            CmdUseSkill(numSkill, selectionToTransmit, GameManager.instance.PlayingPlaceable.netId);
        }
        else
        {
            GameManager.instance.ActiveSkill = null;
            GameManager.instance.State = States.Move;
            FloatingTextController.CreateFloatingText("Not enough PA", GameManager.instance.PlayingPlaceable.transform, Color.red);
            cameraScript.BackToMovement();
        }
    }

    [ClientRpc]
    public void RpcUseSkill(int numSkill, int[] infoSelection, int netIdAskingChar)
    {
        LivingPlaceable askingPlaceable = (LivingPlaceable)GameManager.instance.FindLocalIdeable(netIdAskingChar);
        if (null == askingPlaceable)
        {
            Debug.LogError("AskingChar not found on client");
            return;
        }

        Skill skill = NumberToSkill(askingPlaceable, numSkill);

        SelectionInfo newSelectionInfo = new SelectionInfo();
        newSelectionInfo.FillFromNetwork(infoSelection);

        

        GameManager.instance.currentSelection = newSelectionInfo;

        skill.Use(askingPlaceable, newSelectionInfo.placeable);
        if (isLocalPlayer)
        {
            gameObject.GetComponent<UIManager>().UpdateAvailability();
        }

        if (askingPlaceable.Player.isLocalPlayer)
        {
            cameraScript.BackToMovement();
        }
    }
}
