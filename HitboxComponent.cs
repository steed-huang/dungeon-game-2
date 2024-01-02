using Godot;
using System;

public partial class HitboxComponent : Area2D
{

  [Export]
  private Healthbar healthBar;

  public override void _Ready()
  {
  }

  public override void _Process(double delta)
  {
  }

  public void Damage(Attack attack)
  {
	healthBar?.Damage(attack);
  }
}
