using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;

public class GlobalStateManager : NetworkBehaviour
{
    public List<Transform> randomBombPositions;
    public GameObject m_bombPrefab;
    private TextMeshProUGUI m_timerText;
    private TextMeshProUGUI m_centerText;
    private float m_timeLeft = 120;
    private float m_randomBombExhaust = 0;
    private bool m_gameRunning = false;

    private static GlobalStateManager _instance;
    public static GlobalStateManager Instance { get { return _instance; } }

    void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PollTimer(uint playerNum)
    {
        if(!m_timerText) {
            if(GameObject.Find("TimerText"))
                m_timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
            else
                return;
        }

        if(m_timeLeft > 0) {
            m_timeLeft -= Time.deltaTime;

            if(m_timerText) {
                if(m_timeLeft > 0) {
                    float minutes = Mathf.FloorToInt(m_timeLeft / 60);  
                    float seconds = Mathf.FloorToInt(m_timeLeft % 60);
                    m_timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                } else
                    m_timerText.text = "";
            }
        }

        if(m_timeLeft <= 30 && NetworkServer.active) {
            // sudden death mode
            m_randomBombExhaust -= Time.deltaTime;
            if(m_randomBombExhaust <= 0) {
                m_randomBombExhaust = 1.5f;
                NetworkServer.Spawn(Instantiate(m_bombPrefab, randomBombPositions[Random.Range(0, randomBombPositions.Count)].position, m_bombPrefab.transform.rotation));
            }
        }
    }

    public void RestartTimer()
    {
        m_timeLeft = 120;
    }

    public void SetGameRunning(bool running)
    {
        if(!m_centerText)
            m_centerText = GameObject.Find("CenterText").GetComponent<TextMeshProUGUI>();
        if(!running)
            m_centerText.text = "Esperando o Player 2...";
        else if(running != m_gameRunning)
            m_centerText.text = "";
        m_gameRunning = running;
    }

    public bool IsGameRunning()
    {
        return m_gameRunning;
    }

    public void EndGame(int winner)
    {
        // temp fix to avoid calling it twice
        if(Time.timeScale < 1)
            return;
        m_centerText.text = (winner == 1 ? "Vermelho" : "Azul") + " venceu!";
        Time.timeScale = .1f;
        Invoke("StopHost", .35f);
    }

    public void StopHost()
    {
        RestartTimer();
        NetworkManager.singleton.StopHost();
        Time.timeScale = 1;
    }
}
