using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{

    public float moveSpeed = 5f;
    private bool dead = false;

    public GameObject bombPrefab;
    public GlobalStateManager globalManager;

    private Rigidbody rigidBody;
    private Transform myTransform;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        myTransform = transform;
        animator = myTransform.Find("PlayerModel").GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
        if(Input.GetKeyDown(KeyCode.Space) && canDropBombs())
            DropBomb();
    }

    private void Move()
    {
        bool walking = true;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("Walking", true);
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            rigidBody.velocity = new Vector3 (-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
            animator.SetBool("Walking", true);
        } else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("Walking", true);
        } else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            rigidBody.velocity = new Vector3 (moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        } else
            walking = false;
        animator.SetBool("Walking", walking);
    }

    private void DropBomb()
    {
        Instantiate(bombPrefab,
            new Vector3(Mathf.RoundToInt(myTransform.position.x), bombPrefab.transform.position.y,
            Mathf.RoundToInt(myTransform.position.z)), bombPrefab.transform.rotation);
    }

    private bool canDropBombs()
    {
        // TODO
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!dead && other.CompareTag("Explosion"))
        {
            dead = true;
            globalManager.onPlayerDeath();
        }
    }
}
