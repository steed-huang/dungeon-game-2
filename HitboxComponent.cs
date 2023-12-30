using Godot;
using System;

public partial class HitboxComponent : Area2D
{

  [Export]
  private HealthComponent healthComponent;

  public override void _Ready()
  {
  }

  public override void _Process(double delta)
  {
  }

  public void Damage(Attack attack)
  {
    healthComponent?.Damage(attack);
  }
}
