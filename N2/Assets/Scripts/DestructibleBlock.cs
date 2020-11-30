using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestructibleBlock : NetworkBehaviour
{
    public List<GameObject> m_powerups;
    // save locally to avoid calling for server destroy twice (fixes multiple power-up spawns)
    private bool m_destroyed = false;

    public void OnTriggerEnter(Collider other)
    {
        if(m_destroyed || !NetworkServer.active)
            return;

        if(other.CompareTag("Explosion")) {
            m_destroyed = true;
            NetworkServer.Spawn(Instantiate(m_powerups[Random.Range(0, m_powerups.Count)], transform.position, Quaternion.identity));
            NetworkServer.Destroy(gameObject);
        }
    }
}
