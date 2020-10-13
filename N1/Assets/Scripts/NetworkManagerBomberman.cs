using UnityEngine;
using Mirror;

public class NetworkManagerBomberman : NetworkManager
{
    public GameObject playerPrefab2;

    public GameObject m_bombPrefab;
    public GameObject m_explosionPrefab;
    public Transform m_leftSpawn;
    public Transform m_rightSpawn;

    public override void OnStartClient()
    {
        ClientScene.RegisterPrefab(playerPrefab2);
        ClientScene.RegisterPrefab(m_bombPrefab);
        ClientScene.RegisterPrefab(m_explosionPrefab);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = numPlayers == 0 ? m_leftSpawn : m_rightSpawn;
        GameObject player = Instantiate(numPlayers == 0 ? playerPrefab : playerPrefab2, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }
}
