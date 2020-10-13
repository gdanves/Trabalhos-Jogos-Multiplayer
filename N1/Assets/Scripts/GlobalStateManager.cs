using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class GlobalStateManager : NetworkBehaviour
{
    public void onPlayerDeath()
    {
        // TODO
        Debug.Log("Game over!");
    }
}
