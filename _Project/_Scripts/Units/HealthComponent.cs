using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthComponent", menuName = "ScriptableObjects/HealthComponent")]
[InlineEditor]
public class HealthComponent : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    public int Health { get => currentHealth; set => currentHealth = value; }

    public void Init() => currentHealth = maxHealth;

    public void TakeDamage(int damage) => Health -= damage;
    public void Heal(int heal) => Health += heal;
    public void ResetHealth() => Health = maxHealth;
    public bool IsDead() => Health <= 0;
}
