using Godot;
using System;
using System.Linq;

public partial class MultiplayerController : Control
{
  [Export]
  private const int PORT = 8910;
  [Export]
  private const string ADDRESS = "127.0.0.1";

  private ENetMultiplayerPeer peer;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    Multiplayer.PeerConnected += PeerConnected;
    Multiplayer.PeerDisconnected += PeerDisconnected;
    Multiplayer.ConnectedToServer += ConnectedToServer;
    Multiplayer.ConnectionFailed += ConnectionFailed;
  }

  /// <summary>
  /// Runs when player connects (runs on all peers)
  /// </summary>
  /// <param name="id">id of player that connected</param>
  /// <exception cref="NotImplementedException"></exception>
  private void PeerConnected(long id)
  {
    GD.Print("Player Connected: " + id.ToString());
  }

  /// <summary>
  /// Runs when player disconnects (runs on all peers)
  /// </summary>
  /// <param name="id">id of player that disconnected</param>
  /// <exception cref="NotImplementedException"></exception>
  private void PeerDisconnected(long id)
  {
    GD.Print("Player Disconnected: " + id.ToString());
    GameManager.Players.Remove(GameManager.Players.Where(i => i.Id == id).First<PlayerInfo>());
    var players = GetTree().GetNodesInGroup("Player");

    foreach (var item in players)
    {
      if (item.Name == id.ToString())
      {
        item.QueueFree();
      }
    }
  }

  /// <summary>
  /// Runs when the connection is successful (runs only on the client)
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  private void ConnectedToServer()
  {
    GD.Print("CONNECTED TO SERVER");

    // host handles sending information
    RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("Name Input").Text, Multiplayer.GetUniqueId());
  }

  /// <summary>
  /// Runs when the connection fails (runs only on the client)
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  private void ConnectionFailed()
  {
    GD.Print("CONNECTION FAILED");
  }

  public override void _Process(double delta)
  {
  }

  public void _on_host_button_down()
  {
    peer = new ENetMultiplayerPeer();
    var error = peer.CreateServer(PORT, 2);

    if (error != Error.Ok)
    {
      GD.Print("ERROR CANNOT HOST! :" + error.ToString());
      return;
    }

    peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
    Multiplayer.MultiplayerPeer = peer;

    GD.Print("Waiting For Players...");

    // register host as player
    SendPlayerInformation(GetNode<LineEdit>("Name Input").Text, 1);
  }

  public void _on_join_button_down()
  {
    peer = new ENetMultiplayerPeer();
    peer.CreateClient(ADDRESS, PORT);

    peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
    Multiplayer.MultiplayerPeer = peer;

    GD.Print("Joining Game...");
  }

  public void _on_start_game_button_down()
  {
    Rpc(nameof(startGame));
  }

  [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
  private void startGame()
  {
    foreach (var item in GameManager.Players)
    {
      GD.Print(item.Name + " is playing");
    }

    var scene = ResourceLoader.Load<PackedScene>("res://Tiles.tscn").Instantiate<Node2D>();
    GetTree().Root.AddChild(scene);
    this.Hide();
  }

  [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
  private void SendPlayerInformation(string name, int id)
  {
    PlayerInfo playerInfo = new PlayerInfo()
    {
      Name = name,
      Id = id
    };

    if (!GameManager.Players.Contains(playerInfo))
    {
      GameManager.Players.Add(playerInfo);
    }

    if (Multiplayer.IsServer())
    {
      foreach (var item in GameManager.Players)
      {
        Rpc(nameof(SendPlayerInformation), item.Name, item.Id);
      }
    }
  }

}
