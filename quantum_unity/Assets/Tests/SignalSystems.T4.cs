using Quantum;

namespace Tests
{
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollision2D))]
    public delegate void ISignalOnCollision2DDelegate(Frame f, CollisionInfo2D info);
    
    class SystemISignalOnCollision2D : SystemSignalsOnly, ISignalOnCollision2D {
        public static ISignalOnCollision2DDelegate Callback;
        void ISignalOnCollision2D.OnCollision2D(Frame f, CollisionInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollisionEnter2D))]
    public delegate void ISignalOnCollisionEnter2DDelegate(Frame f, CollisionInfo2D info);
    
    class SystemISignalOnCollisionEnter2D : SystemSignalsOnly, ISignalOnCollisionEnter2D {
        public static ISignalOnCollisionEnter2DDelegate Callback;
        void ISignalOnCollisionEnter2D.OnCollisionEnter2D(Frame f, CollisionInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollisionExit2D))]
    public delegate void ISignalOnCollisionExit2DDelegate(Frame f, ExitInfo2D info);
    
    class SystemISignalOnCollisionExit2D : SystemSignalsOnly, ISignalOnCollisionExit2D {
        public static ISignalOnCollisionExit2DDelegate Callback;
        void ISignalOnCollisionExit2D.OnCollisionExit2D(Frame f, ExitInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTrigger2D))]
    public delegate void ISignalOnTrigger2DDelegate(Frame f, TriggerInfo2D info);
    
    class SystemISignalOnTrigger2D : SystemSignalsOnly, ISignalOnTrigger2D {
        public static ISignalOnTrigger2DDelegate Callback;
        void ISignalOnTrigger2D.OnTrigger2D(Frame f, TriggerInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTriggerEnter2D))]
    public delegate void ISignalOnTriggerEnter2DDelegate(Frame f, TriggerInfo2D info);
    
    class SystemISignalOnTriggerEnter2D : SystemSignalsOnly, ISignalOnTriggerEnter2D {
        public static ISignalOnTriggerEnter2DDelegate Callback;
        void ISignalOnTriggerEnter2D.OnTriggerEnter2D(Frame f, TriggerInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTriggerExit2D))]
    public delegate void ISignalOnTriggerExit2DDelegate(Frame f, ExitInfo2D info);
    
    class SystemISignalOnTriggerExit2D : SystemSignalsOnly, ISignalOnTriggerExit2D {
        public static ISignalOnTriggerExit2DDelegate Callback;
        void ISignalOnTriggerExit2D.OnTriggerExit2D(Frame f, ExitInfo2D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollision3D))]
    public delegate void ISignalOnCollision3DDelegate(Frame f, CollisionInfo3D info);
    
    class SystemISignalOnCollision3D : SystemSignalsOnly, ISignalOnCollision3D {
        public static ISignalOnCollision3DDelegate Callback;
        void ISignalOnCollision3D.OnCollision3D(Frame f, CollisionInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollisionEnter3D))]
    public delegate void ISignalOnCollisionEnter3DDelegate(Frame f, CollisionInfo3D info);
    
    class SystemISignalOnCollisionEnter3D : SystemSignalsOnly, ISignalOnCollisionEnter3D {
        public static ISignalOnCollisionEnter3DDelegate Callback;
        void ISignalOnCollisionEnter3D.OnCollisionEnter3D(Frame f, CollisionInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnCollisionExit3D))]
    public delegate void ISignalOnCollisionExit3DDelegate(Frame f, ExitInfo3D info);
    
    class SystemISignalOnCollisionExit3D : SystemSignalsOnly, ISignalOnCollisionExit3D {
        public static ISignalOnCollisionExit3DDelegate Callback;
        void ISignalOnCollisionExit3D.OnCollisionExit3D(Frame f, ExitInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTrigger3D))]
    public delegate void ISignalOnTrigger3DDelegate(Frame f, TriggerInfo3D info);
    
    class SystemISignalOnTrigger3D : SystemSignalsOnly, ISignalOnTrigger3D {
        public static ISignalOnTrigger3DDelegate Callback;
        void ISignalOnTrigger3D.OnTrigger3D(Frame f, TriggerInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTriggerEnter3D))]
    public delegate void ISignalOnTriggerEnter3DDelegate(Frame f, TriggerInfo3D info);
    
    class SystemISignalOnTriggerEnter3D : SystemSignalsOnly, ISignalOnTriggerEnter3D {
        public static ISignalOnTriggerEnter3DDelegate Callback;
        void ISignalOnTriggerEnter3D.OnTriggerEnter3D(Frame f, TriggerInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }
    [SystemForSignalDelegateAttribute(typeof(SystemISignalOnTriggerExit3D))]
    public delegate void ISignalOnTriggerExit3DDelegate(Frame f, ExitInfo3D info);
    
    class SystemISignalOnTriggerExit3D : SystemSignalsOnly, ISignalOnTriggerExit3D {
        public static ISignalOnTriggerExit3DDelegate Callback;
        void ISignalOnTriggerExit3D.OnTriggerExit3D(Frame f, ExitInfo3D info) {
            Callback?.Invoke(f, info);
        }
    }

    partial class PerfTestBase {

        public static void ClearSignals() {
            SystemISignalOnCollision2D.Callback = null;
            SystemISignalOnCollisionEnter2D.Callback = null;
            SystemISignalOnCollisionExit2D.Callback = null;
            SystemISignalOnTrigger2D.Callback = null;
            SystemISignalOnTriggerEnter2D.Callback = null;
            SystemISignalOnTriggerExit2D.Callback = null;
            SystemISignalOnCollision3D.Callback = null;
            SystemISignalOnCollisionEnter3D.Callback = null;
            SystemISignalOnCollisionExit3D.Callback = null;
            SystemISignalOnTrigger3D.Callback = null;
            SystemISignalOnTriggerEnter3D.Callback = null;
            SystemISignalOnTriggerExit3D.Callback = null;
        }
    }
}