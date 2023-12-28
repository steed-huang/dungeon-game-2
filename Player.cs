using Godot;
using System;

public partial class Player : CharacterBody2D
{
  public const float MoveSpeed = 300.0f;

  [Export]
  public PackedScene Bullet;

  private Vector2 syncPos = new Vector2(0, 0);
  private float syncRotation = 0;

  public override void _Ready()
  {
    // set current player as multiplayer authority
    // Name == player id
    GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
  }

  public override void _PhysicsProcess(double delta)
  {
    // only move player if this is not the local player
    if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
    {

      Vector2 velocity = Velocity;

      // gun
      GetNode<Node2D>("GunRotation").LookAt(GetViewport().GetMousePosition());

      if (Input.IsActionJustPressed("fire"))
      {
        Rpc(nameof(fire));
      }

      // Get the input direction and handle the movement/deceleration.
      // As good practice, you should replace UI actions with custom gameplay actions.
      Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
      if (direction != Vector2.Zero)
      {
        velocity.X = direction.X * MoveSpeed;
        velocity.Y = direction.Y * MoveSpeed;
      }
      else
      {
        velocity.X = Mathf.MoveToward(Velocity.X, 0, MoveSpeed);
        velocity.Y = Mathf.MoveToward(Velocity.Y, 0, MoveSpeed);
      }

      Velocity = velocity.Normalized() * MoveSpeed;
      MoveAndSlide();

      syncPos = GlobalPosition;
      syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
    }
    else
    {
      // linear interpolation of network player information
      GlobalPosition = GlobalPosition.Lerp(syncPos, .2f);
      GetNode<Node2D>("GunRotation").RotationDegrees = Mathf.Lerp(GetNode<Node2D>("GunRotation").RotationDegrees, syncRotation, .2f);
    }
  }

  [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
  private void fire()
  {
    Node2D b = Bullet.Instantiate<Node2D>();
    b.RotationDegrees = GetNode<Node2D>("GunRotation").RotationDegrees;
    b.GlobalPosition = GetNode<Node2D>("GunRotation/BulletSpawn").GlobalPosition;
    GetTree().Root.AddChild(b);
  }


  public void SetUpPlayer(string name)
  {
    GetNode<Label>("Name Label").Text = name;
  }
}
