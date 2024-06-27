namespace Tests {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using NUnit.Framework;
  using Photon.Deterministic;
  using Quantum;
  using Unity.PerformanceTesting;
  using UnityEngine;
  using UnityEngine.TestTools;
  using Assert = NUnit.Framework.Assert;
  using Input = Quantum.Input;
  
  using ComponentAlwaysAdded = Quantum.Transform3D;

  public abstract partial class PerfTestBase {


    [UnityTest]
    [Performance]
    public IEnumerator __WarmupAndOverhead() {
      var setup = new TestSetup() {
        OnUpdate = f => 0
      };
      return RunTest(setup);
    }

    public Type AddSignalDelegate(Delegate del) {
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
    
    protected QuantumRunner CreateRunner(TestSetup setup) {

      var systemsConfig = QuantumDefaultConfigs.Global.SystemsConfig;
      systemsConfig.Reset();
      systemsConfig.AddSystem<DelegatingSystem>();

      foreach (var signalHandler in setup.Signals) {
        var systemType = AddSignalDelegate(signalHandler);
        systemsConfig.AddSystem(systemType);
      }
      
      RuntimeConfig runtimeConfig = new() {
        SimulationConfig = QuantumDefaultConfigs.Global.SimulationConfig,
        SystemsConfig    = QuantumDefaultConfigs.Global.SystemsConfig
      };

      SessionRunner.Arguments arguments = new() {
        RunnerFactory         = QuantumRunnerUnityFactory.DefaultFactory,
        GameParameters        = QuantumRunnerUnityFactory.CreateGameParameters,
        RuntimeConfig         = runtimeConfig,
        SessionConfig         = QuantumDeterministicSessionConfigAsset.DefaultConfig,
        GameMode              = DeterministicGameMode.Local,
        RunnerId              = "LOCALDEBUG",
        PlayerCount           = Input.MAX_COUNT,
        InstantReplaySettings = default,
        InitialDynamicAssets  = default,
        DeltaTimeType         = SimulationUpdateTime.EngineDeltaTime,
      };

      Debug.Log("Creating runner");
      return QuantumRunner.StartGame(arguments);
    }

    protected static unsafe void CreateEntities(Frame f, int count, Type alwaysAdd, params ComponentSpec[] components) {
      if (alwaysAdd != null) {
        var newComponents = new ComponentSpec[components.Length + 1];
        Array.Copy(components, newComponents, components.Length);
        
        for (int i = 0; i < newComponents.Length; ++i) {
          newComponents[i].Components.Add(ComponentTypeId.GetComponentIndex(alwaysAdd));
        }

        newComponents[^1].Probability = 1;
        
        CreateEntities(f, count, newComponents);
      } else {
        CreateEntities(f, count, components);
      }
    }

    protected static unsafe void CreateEntities(Frame f, int count, params ComponentSpec[] components) {
      for (int i = 0; i < count; ++i) {
        EntityRef entity = f.Create();

        var p = f.RNG->Next();

        foreach (var spec in components) {
          var componentSet = spec.Components;
          var probability  = spec.Probability;
          p -= probability;
          if (p > 0) {
            continue;
          }
          
          for (int c = 0; c < ComponentSet.MAX_COMPONENTS; ++c) {
            if (!componentSet.IsSet(c)) {
              continue;
            }
            f.Add(entity, c, null);
          }
          break;
        }
      }
    }

    public static ComponentSpec[] WithComponents(params ComponentSpec[] components) {
      return components;
    }

    public class TestSetup {
      public Func<Frame, int> OnUpdate;
      public Action<Frame>    OnInit;
      public Action<Frame>    OnBeforeUpdate;
      public int              FrameCount = 50;
      //public List<Type>       Systems    = new();
      public List<Delegate>   Signals    = new();
    }

    public IEnumerator RunTest(TestSetup setup) {
      Assert.IsNull(DelegatingSystem._Update);
      Assert.IsNull(DelegatingSystem._OnInit);

      try {
        DelegatingSystem._OnInit = f => { setup.OnInit?.Invoke(f); };

        using QuantumRunner runner = CreateRunner(setup);
        // spin everything up
        runner.Service(1.0);

        double      delta       = 1.0 / runner.Game.Session.SimulationRate;
        
        SampleGroup sampleGroup = new("UpdateTime", SampleUnit.Microsecond);
        SampleGroup update      = new SampleGroup("Quantum.TaskContext.EndFrame", SampleUnit.Microsecond);
        
        int         lastValue     = -1;

        DelegatingSystem._Update = f => {
          setup.OnBeforeUpdate?.Invoke(f);
          setup.OnUpdate(f);
        };

        runner.Service(delta);
        
        DelegatingSystem._Update = f => {
          setup.OnBeforeUpdate?.Invoke(f);
          int value;
          using (Measure.Scope(sampleGroup)) {
            value = setup.OnUpdate(f);
          }
          lastValue = value;
        };

        for (int i = 0; i < setup.FrameCount; i++) {
          using (Measure.ProfilerMarkers(update)) {
            runner.Service(delta);
          }
          
          yield return null;
        }
        Debug.Log($"Last value: {lastValue}");
      } finally {
        DelegatingSystem._Update = null;
        DelegatingSystem._OnInit = null;
        ClearSignals();
      }
      
      yield break;
    }

    //
    // public IEnumerator RunTest(Func<Frame, int> test, 
    //   Action<Frame> oneTimeSetUp = null, 
    //   Action<Frame> setUp = null, 
    //   int frameCount = 50,
    //   params Delegate[] signalHandlers
    //   ) {
    //   Assert.IsNull(DelegatingSystem._Update);
    //   Assert.IsNull(DelegatingSystem._OnInit);
    //
    //   try {
    //     DelegatingSystem._OnInit = f => { oneTimeSetUp?.Invoke(f); };
    //
    //     using QuantumRunner runner = CreateRunner();
    //     // spin everything up
    //     runner.Service(1.0);
    //
    //     double      delta       = 1.0 / runner.Game.Session.SimulationRate;
    //     
    //     SampleGroup sampleGroup = new("UpdateTime", SampleUnit.Microsecond);
    //     SampleGroup update      = new SampleGroup("Quantum.TaskContext.EndFrame", SampleUnit.Microsecond);
    //     
    //     int         lastValue     = -1;
    //
    //     DelegatingSystem._Update = f => {
    //       setUp?.Invoke(f);
    //       test(f);
    //     };
    //
    //     runner.Service(delta);
    //     
    //     DelegatingSystem._Update = f => {
    //       setUp?.Invoke(f);
    //       int value;
    //       using (Measure.Scope(sampleGroup)) {
    //         value = test(f);
    //       }
    //       lastValue = value;
    //     };
    //
    //     for (int i = 0; i < frameCount; i++) {
    //       using (Measure.ProfilerMarkers(update)) {
    //         runner.Service(delta);
    //       }
    //     }
    //     Debug.Log($"Last value: {lastValue}");
    //   } finally {
    //     DelegatingSystem._Update = null;
    //     DelegatingSystem._OnInit = null;
    //   }
    //   
    //   yield break;
    // }

    protected unsafe int DestroyEntities<T>(Frame f, FP percent) where T : unmanaged, IComponent {
      int destroyedEntities = 0;
      foreach (var pair in f.Unsafe.GetComponentBlockIterator<T>())
        if (f.RNG->Next() <= percent) {
          destroyedEntities++;
          f.Destroy(pair.Entity);
        }

      f.Unsafe.CommitAllCommands();
      return destroyedEntities;
    }

    protected void SimpleSetUp(Frame f, TestParams t, params ComponentSpec[] specs) {
      CreateEntities(f, t.EntityCount, typeof(ComponentAlwaysAdded), specs);
      if (t.ShuffleEntities) {
        for (int i = 0; i < 5; i++) {
          int count = DestroyEntities<ComponentAlwaysAdded>(f, FP._0_20);
          CreateEntities(f, count, typeof(ComponentAlwaysAdded), specs);
        }
      }
    }
    
    [Serializable]
    public partial struct TestParams {
      public int  EntityCount;
      public bool ShuffleEntities;

      public override string ToString() {
        return JsonUtility.ToJson(this);
      }
    }
    
    public struct ComponentSpec {
      public ComponentSet Components;
      public FP           Probability;
      
      public static implicit operator ComponentSpec(Type type) {
        var set = new ComponentSet();
        set.Add(ComponentTypeId.GetComponentIndex(type));
        return new ComponentSpec {
          Components  = set,
          Probability = 1
        };
      }

      public static implicit operator ComponentSpec((Type, float) tuple) {
        var set = new ComponentSet();
        set.Add(ComponentTypeId.GetComponentIndex(tuple.Item1));
        return new ComponentSpec {
          Components  = set,
          Probability = FP.FromFloat_UNSAFE(tuple.Item2)
        };
      }

      public static implicit operator ComponentSpec((ComponentSet set, float probability) tuple) {
        return new ComponentSpec {
          Components  = tuple.set,
          Probability = FP.FromFloat_UNSAFE(tuple.probability)
        };
      }
      
      public static implicit operator ComponentSpec(int typeId) {
        var set = new ComponentSet();
        set.Add(typeId);
        return new ComponentSpec {
          Components  = set,
          Probability = 1
        };
      }
    }
  }
}