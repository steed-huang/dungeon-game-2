using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
  public const float Speed = 300.0f;

  public override void _PhysicsProcess(double delta)
  {
    Vector2 velocity = Velocity;
    Velocity = velocity;

    MoveAndSlide();
  }
}
