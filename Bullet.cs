using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
  public const float Speed = 500.0f;
  private Vector2 direction = new Vector2();

  public override void _Ready()
  {
    direction = new Vector2(1, 0).Rotated(Rotation);
  }

  public override void _PhysicsProcess(double delta)
  {
    Vector2 velocity = Speed * direction;
    Velocity = velocity;

    KinematicCollision2D collisionResult = MoveAndCollide(Velocity * (float)delta);

    if (collisionResult is not null)
    {
      QueueFree();
    }
  }

  private void _on_timer_timeout()
  {
    QueueFree();
  }
}