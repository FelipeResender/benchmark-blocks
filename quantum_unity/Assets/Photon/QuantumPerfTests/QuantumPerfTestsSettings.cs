#if UNITY_EDITOR
namespace Quantum.PerfTests {
  using UnityEditor;

  public static class QuantumPerfTestsSettings  {
    
    public static bool ForceInteractive {
      get => EditorPrefs.GetBool("QuantumPrefTests_ForceInteractive", false);
      set => EditorPrefs.SetBool("QuantumPrefTests_ForceInteractive", value);
    }
    
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider() {
      return new SettingsProvider("Preferences/Quantum/PerfTests", SettingsScope.User) {
        guiHandler = searchContext => {
          ForceInteractive = EditorGUILayout.Toggle("Force Interactive", ForceInteractive);
        },
        keywords = new[] { "Quantum", "Perf", "Test", "Photon" }
      };
    }
  }
}
#endif