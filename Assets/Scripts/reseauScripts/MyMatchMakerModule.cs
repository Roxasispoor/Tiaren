using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyMatchMakerModule : MatchmakerModule
{
    protected virtual void Awake()
    {
        AddOptionalDependency<RoomsModule>();
        AddOptionalDependency<LobbiesModule>();
    }
    public override void Initialize(IServer server)
    {
        base.Initialize(server);

        // Add handlers
        server.SetHandler((short)MsfOpCodes.FindAppropriateGame, HandleFindAppropriateGame);

    }

    public void HandleFindAppropriateGame(IIncommingMessage message)
    {
        var list = new List<GameInfoPacket>();
        GameInfoPacket toreturn=null;
        var filters = new Dictionary<string, string>().FromBytes(message.AsBytes());

        foreach (var provider in GameProviders)
        {
            list.AddRange(provider.GetPublicGames(message.Peer, filters));
            
        }
        foreach (var actualGame in list)
        {
            if(actualGame.OnlinePlayers<2)//si il n'y a pas deux joueurs
            {
                toreturn = actualGame;
                break;
            }
        }
        if(toreturn != null)
        {
            message.Respond(toreturn.ToBytes(), ResponseStatus.Success); 
        }
        else
        {
            message.Respond("", ResponseStatus.Failed);
        }
      
            // Convert to generic list and serialize to bytes
            

    }
}
