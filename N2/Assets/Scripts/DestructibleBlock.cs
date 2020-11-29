using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestructibleBlock : NetworkBehaviour
{
    public List<GameObject> m_powerups;

    public void OnTriggerEnter(Collider other)
    {
        if(!NetworkServer.active)
            return;

        if(other.CompareTag("Explosion")) {
            NetworkServer.Spawn(Instantiate(m_powerups[Random.Range(0, m_powerups.Count)], transform.position, Quaternion.identity));
            NetworkServer.Destroy(gameObject);
        }
    }
}
