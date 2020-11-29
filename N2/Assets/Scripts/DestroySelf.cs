using UnityEngine;
using System.Collections;
using Mirror;

public class DestroySelf : NetworkBehaviour
{
    void Start()
    {
        Invoke("DisableCollider", .05f);
        Invoke("DestroyServerObject", 1);
    }

    private void DisableCollider()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private void DestroyServerObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}
