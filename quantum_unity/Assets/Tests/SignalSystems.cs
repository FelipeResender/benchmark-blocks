namespace Tests {
  using System;

  class SystemForSignalDelegateAttribute : Attribute {
    public SystemForSignalDelegateAttribute(Type systemType) {
      Type = systemType;
    }
    public Type Type { get; }
  }
}