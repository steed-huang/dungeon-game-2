using Godot;
using System;

public partial class SceneManager : Node2D
{
  [Export]
  private PackedScene playerScene;

  public override void _Ready()
  {
    int index = 0;
    foreach (var item in GameManager.Players)
    {
      Player currentPlayer = playerScene.Instantiate<Player>();
      currentPlayer.Name = item.Id.ToString();
      currentPlayer.SetUpPlayer(item.Name);
      AddChild(currentPlayer);

      foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
      {
        if (int.Parse(spawnPoint.Name) == index)
        {
          currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
        }
      }
      index++;
    }
  }

  public override void _Process(double delta)
  {
  }
}
