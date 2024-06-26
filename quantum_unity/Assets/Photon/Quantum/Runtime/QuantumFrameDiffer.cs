namespace Quantum {
  using Photon.Realtime;
  using UnityEngine;
  using static QuantumUnityExtensions;

  public class QuantumFrameDiffer : QuantumMonoBehaviour {
    public QuantumFrameDifferGUI.FrameDifferState State = new QuantumFrameDifferGUI.FrameDifferState();

    class QuantumFrameDifferGUIRuntime : QuantumFrameDifferGUI {
      public QuantumFrameDifferGUIRuntime(FrameDifferState state) : base(state) {
      }

      public override int TextLineHeight {
        get { return 20; }
      }

      public override Rect Position {
        get { return new Rect(0, 0, Screen.width, Screen.height); }
      }

      public override void DrawHeader() {
        GUILayout.Space(5);

        if (_hidden) {
          if (GUILayout.Button("Show Quantum Frame Differ", MiniButton, GUILayout.Height(16))) {
            _hidden = false;
          }
        } else {
          if (GUILayout.Button("Hide", MiniButton, GUILayout.Height(16))) {
            _hidden = true;
          }
        }
      }
    }

    // gui instance
    QuantumFrameDifferGUI _gui;

    // draw stuff... lol
    void OnGUI() {
      if (_gui == null) {
        _gui = new QuantumFrameDifferGUIRuntime(State);
      }

      GUILayout.BeginArea(_gui.Position);

      _gui.OnGUI();

      GUILayout.EndArea();
    }

    public static QuantumFrameDiffer Show() {
      var instance = FindFirstObjectByType<QuantumFrameDiffer>();
      if (instance) {
        instance._gui.Show();
        return instance;
      }

      GameObject gameObject;
      gameObject = new GameObject(typeof(QuantumFrameDiffer).Name);

      var differ = gameObject.AddComponent<QuantumFrameDiffer>();
      if (differ._gui == null) {
        differ._gui = new QuantumFrameDifferGUIRuntime(differ.State);
      }

      differ._gui.Show();

      return differ;
    }

    public static string TryGetPhotonNickname(RealtimeClient client, int actorId) {
      // Try to get Photon nickname
      if (client != null && client.CurrentRoom != null) {
        client.CurrentRoom.Players.TryGetValue(actorId, out var player);
        if (player != null) {
          return player.NickName;
        }
      }

      return null;
    }
  }
}