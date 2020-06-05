using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representation of player object in server scene
/// </summary>

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float health;
    public float maxhealth = 100f;

    private bool[] inputs;
    private float yVelocity = 0;

    /// <summary>
    /// Initialize uniform player values
    /// </summary>

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    /// <summary>
    /// Initializes new player and assigns default initial values
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_username"></param>

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxhealth;

        inputs = new bool[5];
    }

    /// <summary>
    /// handles player movement input in server authoritative way
    /// </summary>

    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    /// <summary>
    /// Takes player input and moves player accordingly
    /// </summary>
    /// <param name="_inputDirection">Vector2 representing new position of player</param>

    private void Move(Vector2 _inputDirection)
    {

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>
    /// Sets inputs and rotation of player
    /// </summary>
    /// <param name="_inputs">Array of bools representing input keys pressed this tick</param>
    /// <param name="_rotation">Quaternion representing current player rotation</param>

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    /// <summary>
    /// Deals damage to other player
    /// </summary>
    /// <param name="_viewDirection">Direction player is facing used to determine if shot hits</param>

    public void Shoot(Vector3 _viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    /// <summary>
    /// handles player taking damage when being shot
    /// </summary>
    /// <param name="_damage">Amount of health removed from target</param>

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    /// <summary>
    /// Respawns player
    /// </summary>
    /// <returns>Waits for respawn time before sending player back into game world</returns>

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxhealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
}
