using Godot;
using System;

public partial class Player : CharacterBody2D
{
  public const float moveSpeed = 200.0f;
  public const float maxMoveSpeed = 300.0f;
  private const float acceleration = 0.25f;
  private const float friction = 0.2f;

  // firing logic will be moved into weapon eventually
  private float fireRate = 0.2f;
  private bool canFire = true;

  [Export]
  private Timer fireTimer;

  [Export]
  public PackedScene Bullet;

  private Vector2 syncPos = new Vector2(0, 0);
  private float syncRotation = 0;

  public override void _Ready()
  {
    //  current player as multiplayer authority
    // Name == player id
    GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));

    fireTimer.WaitTime = fireRate;
    fireTimer.OneShot = true;
  }

  public override void _PhysicsProcess(double delta)
  {
    // only move player if this is the local player
    if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
    {
      GetNode<Camera2D>("Camera").MakeCurrent(); // attatch camera to local player

      Vector2 velocity = Velocity;

      // gun rotation
      Vector2 localMousePos = ToLocal(GetGlobalMousePosition()); // local position of the mouse relative to the player
      GetNode<Node2D>("GunRotation").LookAt(localMousePos + Position);

      if (Input.IsActionPressed("fire") && canFire)
      {
        Rpc(nameof(Fire));
        canFire = false;
        fireTimer.Start();
      }

      // Get the input direction and handle the movement/deceleration.
      Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
      if (direction != Vector2.Zero)
      {
        velocity.X = Mathf.Lerp(velocity.X, direction.X * moveSpeed, acceleration);
        velocity.Y = Mathf.Lerp(velocity.Y, direction.Y * moveSpeed, acceleration);
      }
      else
      {
        velocity.X = Mathf.Lerp(Velocity.X, 0, friction);
        velocity.Y = Mathf.Lerp(Velocity.Y, 0, friction);
      }

      Velocity = velocity.LimitLength(maxMoveSpeed);
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

  private void _on_fire_timer_timeout()
  {
    canFire = true;
  }

  [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
  private void Fire()
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
