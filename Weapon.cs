using Godot;
using System;

public partial class Weapon : Node2D
{
  [Export]
  private Timer fireTimer;

  [Export]
  public PackedScene bullet;

  private float fireRate = 0.2f;
  private bool canFire = true;

  public override void _Ready()
  {
    fireTimer.WaitTime = fireRate;
    fireTimer.OneShot = true;
  }

  public override void _Process(double delta)
  {
  }

  private void _on_fire_timer_timeout()
  {
    canFire = true;
  }

  public void handleFire(float GunRotationDegrees)
  {
    if (!canFire) return;
    Rpc(nameof(Fire), GunRotationDegrees);
    canFire = false;
    fireTimer.Start();
  }

  [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
  private void Fire(float GunRotationDegrees)
  {
    Node2D b = bullet.Instantiate<Node2D>();
    b.RotationDegrees = GunRotationDegrees;
    b.GlobalPosition = GetNode<Node2D>("Bullet Spawn").GlobalPosition;
    GetTree().Root.AddChild(b);
  }
}
