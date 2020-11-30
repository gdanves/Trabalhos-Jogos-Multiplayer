using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Mirror;

public class Bomb : NetworkBehaviour
{
    public AudioClip m_explosionSound;
    public GameObject m_explosionPrefab;
    public LayerMask m_levelMask;
    private AudioSource m_audioSource;
    private bool m_exploded = false;
    private uint m_power = 3;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        Invoke("Explode", 3f);
    }

    void Explode()
    {
        m_audioSource.clip = m_explosionSound;
        m_audioSource.Play();
        if(!NetworkServer.active)
            return;

        NetworkServer.Spawn(Instantiate(m_explosionPrefab, transform.position, Quaternion.identity));

        CreateExplosions(Vector3.forward);
        CreateExplosions(Vector3.right);
        CreateExplosions(Vector3.back);
        CreateExplosions(Vector3.left);

        GetComponent<MeshRenderer>().enabled = false;
        m_exploded = true;
        transform.Find("Collider").gameObject.SetActive(false);
        Invoke("DestroyServerObject", .3f);
    }

    public void SetPower(uint power)
    {
        m_power = power;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!m_exploded && other.CompareTag("Explosion")) {
            CancelInvoke("Explode");
            Explode();
        }
    }

    private void CreateExplosions(Vector3 direction)
    {
        for(int i = 1; i < m_power; i++) {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, .5f, 0), direction, out hit, i, m_levelMask);
            if(!hit.collider)
                NetworkServer.Spawn(Instantiate(m_explosionPrefab, transform.position + (i * direction), m_explosionPrefab.transform.rotation));
            else
                break;
        }
    }

    private void DestroyServerObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}
