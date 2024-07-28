// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.DynamicAccessor
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Model.Accessors
{
  internal abstract class DynamicAccessor
  {
    protected DynamicAccessor(
      DynamicModelReadContext context,
      Type targetType,
      ReflectionObjectKind kind)
    {
      this.Context = context;
      this.TargetType = targetType;
      this.Kind = kind;
    }

    public ReflectionObjectKind Kind { get; }

    public DynamicModelReadContext Context { get; }

    public Type TargetType { get; }

    public override string ToString() => this.GetType().Name + " Type: " + this.TargetType.FullName;
  }
}
