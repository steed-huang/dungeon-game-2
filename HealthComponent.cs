using Godot;
using System;

public partial class HealthComponent : Node2D
{

  [Export]
  private float maxHealth = 10.0f;

  private float health;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    health = maxHealth;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
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
