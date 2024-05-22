using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    public int _Health { get => currentHealth; set => currentHealth = value; }

    private void Start()
    {
        Init();
    }
    public void Init() => currentHealth = maxHealth;

    public void TakeDamage(int damage) => _Health -= damage;
    public void Heal(int heal) => _Health += heal;
    public void ResetHealth() => _Health = maxHealth;
    public bool IsDead() => _Health <= 0;
}
