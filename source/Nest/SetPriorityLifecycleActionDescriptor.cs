// Decompiled with JetBrains decompiler
// Type: Nest.SetPriorityLifecycleActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SetPriorityLifecycleActionDescriptor : 
    DescriptorBase<SetPriorityLifecycleActionDescriptor, ISetPriorityLifecycleAction>,
    ISetPriorityLifecycleAction,
    ILifecycleAction
  {
    int? ISetPriorityLifecycleAction.Priority { get; set; }

    public SetPriorityLifecycleActionDescriptor Priority(int? priority) => this.Assign<int?>(priority, (Action<ISetPriorityLifecycleAction, int?>) ((a, v) => a.Priority = priority));
  }
}
