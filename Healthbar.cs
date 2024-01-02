using Godot;
using System;

public partial class Healthbar : ProgressBar
{

  [Export]
  private float maxHealth = 10.0f;

  private float health = 0.0f;

  [Export]
  private ProgressBar damageBar;

  [Export]
  private Timer damageTimer;

  public override void _Ready()
  {
	health = maxHealth;

	// health progress bar
	MaxValue = maxHealth;
	Value = health;

	// damage progress bar
	damageBar.MaxValue = health;
	damageBar.Value = health;
  }

  public override void _Process(double delta)
  {

  }

  public void Damage(Attack attack)
  {
	float prevHealth = health;

	health -= attack.damage;
	Value = health;

	if (health <= 0)
	{
	  GetParent().QueueFree();
	}

	if (health < prevHealth)
	{
	  damageTimer.Start();
	}
	else
	{
	  damageBar.Value = health; // for healing
	}
  }

  private void _on_damage_timer_timeout()
  {
	damageBar.Value = health;
  }
}
