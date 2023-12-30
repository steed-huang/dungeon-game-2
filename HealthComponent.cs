using Godot;
using System;

public partial class HealthComponent : Node2D
{

  [Export]
  private float maxHealth = 10.0f;

  private float health;

  public override void _Ready()
  {
    health = maxHealth;
  }

  public override void _Process(double delta)
  {
  }

  public void Damage(Attack attack)
  {
    health -= attack.damage;

    if (health <= 0)
    {
      GetParent().QueueFree();
    }
  }
}
