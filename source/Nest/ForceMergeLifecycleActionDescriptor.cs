// Decompiled with JetBrains decompiler
// Type: Nest.ForceMergeLifecycleActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ForceMergeLifecycleActionDescriptor : 
    DescriptorBase<ForceMergeLifecycleActionDescriptor, IForceMergeLifecycleAction>,
    IForceMergeLifecycleAction,
    ILifecycleAction
  {
    int? IForceMergeLifecycleAction.MaximumNumberOfSegments { get; set; }

    public ForceMergeLifecycleActionDescriptor MaximumNumberOfSegments(int? maximumNumberOfSegments) => this.Assign<int?>(maximumNumberOfSegments, (Action<IForceMergeLifecycleAction, int?>) ((a, v) => a.MaximumNumberOfSegments = maximumNumberOfSegments));
  }
}
