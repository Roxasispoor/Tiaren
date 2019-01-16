using Barebones.MasterServer;
using Barebones.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptedMsfMatchMakerClient : MsfMatchmakerClient
{
    public delegate void FindGamesCallback(GameInfoPacket game);
    public AdaptedMsfMatchMakerClient(IClientSocket connection) : base(connection)
    {
    }

    public virtual void FindAppropriateMatch(Dictionary<string, string> filter, FindGamesCallback callback, IClientSocket connection)
    {
        if (!connection.IsConnected)

        {

            Logs.Error("Not connected");
            callback.Invoke(null);
            return;
        }

        connection.SendMessage((short)MsfOpCodes.FindAppropriateGame, filter.ToBytes(), (status, response) =>
        {
            if (status != ResponseStatus.Success)
            {
                Logs.Error(response.AsString("Unknown error while requesting a list of games"));
                callback.Invoke(new GameInfoPacket());
                return;
            }

            var game = response.Deserialize(new GameInfoPacket());

            callback.Invoke(game);
        });

    }
    public void FindAppropriateMatch(FindGamesCallback callback)
    {
        FindAppropriateMatch(new Dictionary<string, string>(), callback, Connection);
    }
    public void FindAppropriateMatch(Dictionary<string,string> settings, FindGamesCallback callback)
    {
        FindAppropriateMatch(settings, callback, Connection);
    }

}
