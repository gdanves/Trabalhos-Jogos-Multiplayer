using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerBomberman : NetworkManager
{
    public GameObject playerPrefab2;

    public GameObject m_bombPrefab;
    public GameObject m_explosionPrefab;
    public Transform m_leftSpawn;
    public Transform m_rightSpawn;
    public GlobalStateManager m_globalManager;

    public override void Awake()
    {
        SceneManager.LoadScene("Offline", LoadSceneMode.Additive);
        base.Awake();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

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
        m_globalManager.EndGame(1);
        base.OnServerDisconnect(conn);
    }
}
