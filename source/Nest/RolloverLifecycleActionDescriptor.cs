// Decompiled with JetBrains decompiler
// Type: Nest.RolloverLifecycleActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RolloverLifecycleActionDescriptor : 
    DescriptorBase<RolloverLifecycleActionDescriptor, IRolloverLifecycleAction>,
    IRolloverLifecycleAction,
    ILifecycleAction
  {
    Time IRolloverLifecycleAction.MaximumAge { get; set; }

    long? IRolloverLifecycleAction.MaximumDocuments { get; set; }

    string IRolloverLifecycleAction.MaximumSize { get; set; }

    public RolloverLifecycleActionDescriptor MaximumSize(string maximumSize) => this.Assign<string>(maximumSize, (Action<IRolloverLifecycleAction, string>) ((a, v) => a.MaximumSize = maximumSize));

    public RolloverLifecycleActionDescriptor MaximumAge(Time maximumAge) => this.Assign<Time>(maximumAge, (Action<IRolloverLifecycleAction, Time>) ((a, v) => a.MaximumAge = maximumAge));

    public RolloverLifecycleActionDescriptor MaximumDocuments(long? maximumDocuments) => this.Assign<long?>(maximumDocuments, (Action<IRolloverLifecycleAction, long?>) ((a, v) => a.MaximumDocuments = maximumDocuments));
  }
}
