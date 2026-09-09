using UnityEngine;

public struct HealthComponent : IComponent
{
    public float MaxHealth;
    public float CurrentHealth;

    public HealthComponent(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
}
