using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Collectable : NetworkBehaviour
{
    public int m_id = 0;
    public int GetId()
    {
        return m_id;
    }
}
