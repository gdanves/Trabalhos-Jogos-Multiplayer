using UnityEngine;
using System.Collections;
using System;
using Mirror;

public class Player : NetworkBehaviour
{
    public uint m_playerNum = 1;

    // power-ups
    private float m_speed;
    private uint m_power;
    private uint m_bombs;

    // states
    private bool m_dead = false;
    [SyncVar]
    private bool m_walking = false;

    // references
    public GameObject m_bombPrefab;
    public GlobalStateManager m_globalManager;
    public AudioClip m_sfxCollect;
    public AudioClip m_sfxDeath;
    public AudioClip m_sfxDefeat;
    public AudioClip m_sfxVictory;
    private Rigidbody m_rb;
    private Transform m_transform;
    private Animator m_animator;
    private AudioSource m_audioSource;

    void Start()
    {
        if(m_playerNum == 1)
            m_globalManager.SetGameRunning(false);

        m_rb = GetComponent<Rigidbody>();
        m_transform = transform;
        m_animator = m_transform.Find("PlayerModel").GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();

        m_globalManager.RestartTimer();

        // setup power-ups
        m_speed = 5;
        m_power = 3;
        m_bombs = 2;
    }

    void FixedUpdate()
    {
        if(!m_globalManager.IsGameRunning()) {
            if(m_playerNum == 2)
                m_globalManager.SetGameRunning(true);
            else
                return;
        }

        m_animator.SetBool("Walking", m_walking);
        if(!isLocalPlayer)
            return;

        m_globalManager.PollTimer(m_playerNum);
        Move();
        if(Input.GetKeyDown(KeyCode.Space) && CanDropBombs()) {
            m_bombs--;
            DropBomb(m_power);
            Invoke("AddBomb", 3f);
        }
    }

    private void Move()
    {
        bool walking = true;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, m_speed);
            m_transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            m_rb.velocity = new Vector3(-m_speed, m_rb.velocity.y, m_rb.velocity.z);
            m_transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, -m_speed);
            m_transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            m_rb.velocity = new Vector3(m_speed, m_rb.velocity.y, m_rb.velocity.z);
            m_transform.rotation = Quaternion.Euler(0, 90, 0);
        } else
            walking = false;

        if(walking != m_walking) {
            m_walking = walking;
            ChangeWalking(walking);
        }
    }

    private void AddBomb()
    {
        m_bombs++;
    }

    private bool CanDropBombs()
    {
        if(m_bombs <= 0)
            return false;
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach(GameObject bomb in bombs) {
            if(Vector3.Distance(transform.position, bomb.transform.position) < 1)
                return false;
        }
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(m_dead)
            return;

        // this should be server-side only to avoid d-sync problems, but for now it works
        if(other.CompareTag("Explosion")) {
            m_audioSource.PlayOneShot(m_sfxDeath);
            m_audioSource.clip = isLocalPlayer ? m_sfxDefeat : m_sfxVictory;
            m_audioSource.PlayDelayed(0.1f);
            m_dead = true;
            int winner = m_playerNum == 1 ? 2 : 1;
            m_globalManager.EndGame(winner);
        } else if(other.CompareTag("Collectable")) {
            int collectableId = other.GetComponent<Collectable>().GetId();
            Destroy(other.gameObject);
            m_audioSource.PlayOneShot(m_sfxCollect);
            switch(collectableId) {
                case 1:
                    m_power++;
                    break;
                case 2:
                    m_speed += 2.5f;
                    break;
                default:
                    m_bombs++;
                    break;
            }
        }

    }

    // server
    [Command]
    private void DropBomb(uint power)
    {
        GameObject bomb = Instantiate(m_bombPrefab,
            new Vector3(Mathf.RoundToInt(m_transform.position.x), m_bombPrefab.transform.position.y,
            Mathf.RoundToInt(m_transform.position.z)), m_bombPrefab.transform.rotation);
        bomb.GetComponent<Bomb>().SetPower(power);
        NetworkServer.Spawn(bomb);
    }

    [Command]
    private void ChangeWalking(bool walking)
    {
        if(isLocalPlayer)
            return;
        m_walking = walking;
    }
}
