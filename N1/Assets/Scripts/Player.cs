using UnityEngine;
using System.Collections;
using System;
using Mirror;

public class Player : NetworkBehaviour
{
    public float m_mspd = 5f;
    private bool m_dead = false;
    [SyncVar]
    private bool m_walking = false;

    public GameObject m_bombPrefab;
    public GlobalStateManager m_globalManager;

    private Rigidbody m_rb;
    private Transform m_transform;
    private Animator m_animator;

    // Use this for initialization
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_transform = transform;
        m_animator = m_transform.Find("PlayerModel").GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        m_animator.SetBool("Walking", m_walking);
        if(!isLocalPlayer)
            return;

        Move();
        if(Input.GetKeyDown(KeyCode.Space) && CanDropBombs())
            DropBomb();
    }

    private void Move()
    {
        bool walking = true;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, m_mspd);
            m_transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            m_rb.velocity = new Vector3(-m_mspd, m_rb.velocity.y, m_rb.velocity.z);
            m_transform.rotation = Quaternion.Euler(0, 270, 0);
        } else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, -m_mspd);
            m_transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            m_rb.velocity = new Vector3(m_mspd, m_rb.velocity.y, m_rb.velocity.z);
            m_transform.rotation = Quaternion.Euler(0, 90, 0);
        } else
            walking = false;

        if(walking != m_walking) {
            m_walking = walking;
            ChangeWalking(walking);
        }
    }

    private bool CanDropBombs()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach(GameObject bomb in bombs) {
            if(Vector3.Distance(transform.position, bomb.transform.position) <= 1)
                return false;
        }
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!m_dead && other.CompareTag("Explosion")) {
            m_dead = true;
            m_globalManager.onPlayerDeath();
        }
    }

    // server
    [Command]
    private void DropBomb()
    {
        NetworkServer.Spawn(Instantiate(m_bombPrefab,
            new Vector3(Mathf.RoundToInt(m_transform.position.x), m_bombPrefab.transform.position.y,
            Mathf.RoundToInt(m_transform.position.z)), m_bombPrefab.transform.rotation));
    }

    [Command]
    private void ChangeWalking(bool walking)
    {
        if(isLocalPlayer)
            return;
        m_walking = walking;
    }
}
