namespace Quantum {
  using Photon.Deterministic;

  public partial class Physics3DFallingBodiesTestSettings : AssetObject {
    public int                                   EntitiesPerInstantiationEvent = 1;
    public int                                   InstantiationEventEveryFrame  = 1;
    public int                                   MaxBodies                     = 1024;
    public FPVector3                             InitialPosition;
    public FP                                    SpreadRadius;
    public Prototypes.PhysicsCollider3DPrototype ColliderProto;
    public Prototypes.PhysicsBody3DPrototype     BodyProto;
    public Prototypes.ViewPrototype              ViewProto;
    public CallbackFlags                         CallbackFlags;

    public FPVector3 RestartForce;
    public FPVector3 RestartToque;
  }
}