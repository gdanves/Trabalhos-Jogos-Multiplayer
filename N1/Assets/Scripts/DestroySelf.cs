using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3f);
    }
}
