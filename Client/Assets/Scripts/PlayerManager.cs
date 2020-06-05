using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles state of all players in client instance
/// </summary>

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public MeshRenderer model;

    /// <summary>
    /// Sets values for newly spawned player
    /// </summary>
    /// <param name="_id">Player Id</param>
    /// <param name="_username">Player Username</param>

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    /// <summary>
    /// Updates player health and kills player if health at or below zero
    /// </summary>
    /// <param name="_health">New health value</param>

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Removes player model from scene when health hits zero
    /// </summary>

    public void Die()
    {
        model.enabled = false;
    }

    /// <summary>
    /// Respawns player
    /// </summary>

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }
}
