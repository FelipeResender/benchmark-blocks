namespace Quantum.PerfTests  {
  using System.Collections;
  using NUnit.Framework;
  using Photon.Deterministic;
  using Quantum;
  using Unity.PerformanceTesting;
  using UnityEngine.TestTools;
  using Assert = NUnit.Framework.Assert;

  public unsafe class PerfTestFallingSpheres : QuantumPerfTestBase {
    public static string[] DefaultTestCases = new[] {
      "QuantumUser/PerfTests/FallingBodiesTestSettings"
    };

    [UnityTest, Performance]
    public IEnumerator Test([ValueSource(nameof(DefaultTestCases))] string assetPath) {
      Physics3DFallingBodiesTestSettings t = null;
      return new PerfTestWorker() {
        OnInit = f => {
          Assert.IsTrue(f.TryFindAsset(assetPath, out t));
        },
        OnUpdate = f => {
          if (f.Number % t.InstantiationEventEveryFrame == 0) {
            for (var i = 0; i < t.EntitiesPerInstantiationEvent && f.Global->FallingSpheresBenchmarkInstantiatedBodies < t.MaxBodies; i++) {
              FPVector3 dir;
              dir.X = f.RNG->NextInclusive(FP.Minus_1, FP._1);
              dir.Y = f.RNG->NextInclusive(FP.Minus_1, FP._1);
              dir.Z = f.RNG->NextInclusive(FP.Minus_1, FP._1);
              dir   = dir.Normalized;

              var radius = f.RNG->NextInclusive(FP._0, t.SpreadRadius);
              var pos    = t.InitialPosition + dir * radius;

              var e = f.Create();
              f.Add(e, Transform3D.Create(pos));
              t.ColliderProto.AddToEntity(f, e);
              t.BodyProto.AddToEntity(f, e);
              t.ViewProto.AddToEntity(f, e);

              if (f.Unsafe.TryGetPointer(e, out PhysicsBody3D* body)) {
                body->AddForce(t.RestartForce);
                body->AddTorque(t.RestartToque);
              }

              f.Physics3D.SetCallbacks(e, t.CallbackFlags);

              ++f.Global->FallingSpheresBenchmarkInstantiatedBodies;
            }
          }

          return f.Global->FallingSpheresBenchmarkInstantiatedBodies;
        },
        Signals = {
          new ISignalOnTrigger3DDelegate((f, info) => {
            f.Unsafe.GetPointer<Transform3D>(info.Entity)->Position.Z = t.InitialPosition.Z;
            if (f.Unsafe.TryGetPointer(info.Entity, out PhysicsBody3D* body)) {
              body->Velocity        = default;
              body->AngularVelocity = default;

              body->AddForce(t.RestartForce);
              body->AddTorque(t.RestartToque);
            }
          })
        },
        ProfileMarkers = {
          "Physics3D.BroadPhase",
          "QuantumGame.OnSimulate",
        },
      }.Run();
    }
  }
}