namespace Tests {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using Photon.Deterministic;
  using Quantum;
  using Unity.PerformanceTesting;
  using UnityEditor;
  using UnityEngine;
  using Assert = NUnit.Framework.Assert;
  using Input = Quantum.Input;

  public class QuantumTestRunner {
    public Func<Frame, int> OnUpdate = f => 0;
    public Action<Frame>    OnInit;
    public Action<Frame>    OnBeforeUpdate;
    public int              FrameCount    = 100;
    public bool             IsInteractive = true;

    //public List<Type>       Systems    = new();
    public List<Delegate> Signals = new();

    public IEnumerator Run() {

      DelegatingSystem._OnInit = f => {
        OnInit?.Invoke(f);
      };

      using QuantumRunner runner = CreateRunner();
      // spin everything up
      runner.Service(1.0);
#if UNITY_EDITOR
      Selection.activeObject = UnityEngine.Object.FindFirstObjectByType<QuantumRunnerBehaviour>();
#endif

      double delta = 1.0 / runner.Game.Session.SimulationRate;

      SampleGroup sampleGroup = new("UpdateTime", SampleUnit.Microsecond);
      SampleGroup update      = new SampleGroup("Quantum.TaskContext.EndFrame", SampleUnit.Microsecond);

      int lastValue = -1;

      DelegatingSystem._Update = f => {
        OnBeforeUpdate?.Invoke(f);
        OnUpdate(f);
      };

      runner.Service(delta);

      DelegatingSystem._Update = f => {
        OnBeforeUpdate?.Invoke(f);
        int value;
        using (Measure.Scope(sampleGroup)) {
          value = OnUpdate(f);
        }

        lastValue = value;
      };

      var sg = new SampleGroup("QuantumGame.OnSimulate", SampleUnit.Millisecond);
      using (Measure.ProfilerMarkers(sg)) {
        for (int i = 0; i < FrameCount; i++) {

          runner.Service(delta);

          if (IsInteractive) {
            yield return null;
          }
        }
      }

      Debug.Log($"Last value: {lastValue}");

      yield break;
    }

    private QuantumRunner CreateRunner() {

      var map = ScriptableObject.CreateInstance<Map>();
      map.Guid = AssetGuid.NewGuid();
      QuantumUnityDB.Global.AddAsset(map);
      
      var systemsConfig = ScriptableObject.CreateInstance<SystemsConfig>();
      systemsConfig.Guid = AssetGuid.NewGuid();
      systemsConfig.Reset();
      systemsConfig.AddSystem<DelegatingSystem>();
      foreach (var signalHandler in Signals) {
        var systemType = AddSignalDelegate(signalHandler);
        systemsConfig.AddSystem(systemType);
      }
      QuantumUnityDB.Global.AddAsset(systemsConfig);

      RuntimeConfig runtimeConfig = new() {
        SimulationConfig = QuantumDefaultConfigs.Global.SimulationConfig,
        SystemsConfig    = systemsConfig,
        Map              = map,
      };

      SessionRunner.Arguments arguments = new() {
        RunnerFactory         = QuantumRunnerUnityFactory.DefaultFactory,
        GameParameters        = new() {
          AssetSerializer    = new QuantumUnityJsonSerializer(),
          CallbackDispatcher = QuantumCallback.Dispatcher,
          EventDispatcher    = QuantumEvent.Dispatcher,
          ResourceManager    = QuantumUnityDB.Global,
        },
        RuntimeConfig         = runtimeConfig,
        SessionConfig         = QuantumDeterministicSessionConfigAsset.DefaultConfig,
        GameMode              = DeterministicGameMode.Local,
        RunnerId              = "LOCALDEBUG",
        PlayerCount           = Input.MAX_COUNT,
        InstantReplaySettings = default,
        InitialDynamicAssets  = default,
        DeltaTimeType         = SimulationUpdateTime.Default,
      };

      Debug.Log("Creating runner");
      var runner = QuantumRunner.StartGame(arguments);
      runner.IsSessionUpdateDisabled = true;
      return runner;
    }
    
    static Type AddSignalDelegate(Delegate del) {
      // check if delegate lives in 
      var delegateAttribute = del.GetType().GetAttribute<SystemForSignalDelegateAttribute>();
      if (delegateAttribute == null) {
        Assert.Fail($"Provided delegate {del} does not have a SystemForSignalDelegateAttribute");
      }
      
      var systemType = delegateAttribute.Type;
      Assert.IsTrue(systemType?.IsSubclassOf(typeof(SystemBase)) == true);
        
      // get static callback field
      var field = systemType.GetField("Callback", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
      Assert.NotNull(field);
        
      // set the callback
      field.SetValue(null, del);
      return systemType;
    }

  }
}