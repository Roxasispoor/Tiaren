using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine;
using UnityEngine.UI;
public class DisplayConnection : MonoBehaviour {
    private IClientSocket _connection;
    public Text Text;
    // Use this for initialization
    void Start () {
        Text = gameObject.GetComponent<Text>();
        _connection = GetConnection();
        _connection.StatusChanged += UpdateStatusView;
        UpdateStatusView(_connection.Status);
    }
    protected IClientSocket GetConnection()
    {
        return Msf.Connection;
    }
    protected void OnDestroy()
    {
        _connection.StatusChanged -= UpdateStatusView;
    }
    protected void UpdateStatusView(ConnectionStatus status)
    {
        switch (status)
        {
            case ConnectionStatus.Connected:
                Text.text = "Connected";
                break;
            case ConnectionStatus.Disconnected:
                Text.text = "Offline";
                break;
            case ConnectionStatus.Connecting:
                Text.text = "Connecting";
                break;
            default:
                Text.text = "Unknown";
                break;
        }
    }

        // Update is called once per frame
        void Update () {
		
	}
}
