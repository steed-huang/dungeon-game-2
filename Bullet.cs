using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
  public const float Speed = 500.0f;
  private Vector2 direction = new Vector2();

  private const float attackDamage = 10.0f;
  private const float knockbackForce = 100.0f;

  public override void _Ready()
  {
    direction = new Vector2(1, 0).Rotated(Rotation);
  }

  public override void _PhysicsProcess(double delta)
  {
    Vector2 velocity = Speed * direction;
    Velocity = velocity;

    KinematicCollision2D collisionResult = MoveAndCollide(Velocity * (float)delta);

    // tile collisions
    if (collisionResult is not null)
    {
      QueueFree();
    }
  }

  private void _on_area_2d_area_entered(Area2D area)
  {
    if (area is HitboxComponent component)
    {
      Attack a = new Attack
      {
        damage = attackDamage,
        knockbackForce = knockbackForce,
        position = GlobalPosition
      };

      component.Damage(a);
      QueueFree();
    }
  }

  private void _on_timer_timeout()
  {
    QueueFree();
  }
}