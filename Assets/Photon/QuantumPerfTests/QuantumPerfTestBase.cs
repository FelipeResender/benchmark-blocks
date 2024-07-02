namespace Quantum.PerfTests {
  using System;
  using System.Collections;
  using NUnit.Framework;
  using Photon.Deterministic;
  using Quantum;
  using Unity.PerformanceTesting;
  using UnityEngine;
  using UnityEngine.TestTools;

  public abstract partial class QuantumPerfTestBase {
    [SetUp]
    protected virtual void SetUp() {
      DelegatingSystem.Clear();
      ISignalDelegates.Clear();
    }

    [TearDown]
    protected virtual void TearDown() {
      DelegatingSystem.Clear();
      ISignalDelegates.Clear();
    }

    [UnityTest]
    [Performance]
    public IEnumerator __WarmupAndOverhead() => new PerfTestWorker().Run();

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
      CreateEntities(f, t.EntityCount, typeof(Transform3D), specs);
      if (t.ShuffleEntities) {
        for (int i = 0; i < 5; i++) {
          int count = DestroyEntities<Transform3D>(f, FP._0_20);
          CreateEntities(f, count, typeof(Transform3D), specs);
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