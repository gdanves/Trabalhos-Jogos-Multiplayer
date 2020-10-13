using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class GlobalStateManager : NetworkBehaviour
{
    public List<Transform> randomBombPositions;
    public GameObject m_bombPrefab;
    private Text m_timerText;
    private Text m_winnerText;
    private float m_timeLeft = 120;
    private float m_randomBombExhaust = 0;

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
                m_timerText = GameObject.Find("TimerText").GetComponent<Text>();
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
        } else if(playerNum == 1) {
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

    public void EndGame(int winner)
    {
        if(!m_winnerText)
            m_winnerText = GameObject.Find("WinnerText").GetComponent<Text>();
        m_winnerText.text = (winner == 1 ? "Vermelho" : "Azul") + " venceu!";
        Time.timeScale = .1f;
        Invoke("StopHost", .2f);
    }

    public void StopHost()
    {
        RestartTimer();
        Time.timeScale = 1;
        NetworkManager.singleton.StopHost();
    }
}
