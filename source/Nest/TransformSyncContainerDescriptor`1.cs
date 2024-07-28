// Decompiled with JetBrains decompiler
// Type: Nest.TransformSyncContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TransformSyncContainerDescriptor<T> : 
    DescriptorBase<TransformSyncContainerDescriptor<T>, ITransformSyncContainer>,
    ITransformSyncContainer
  {
    ITransformTimeSync ITransformSyncContainer.Time { get; set; }

    public TransformSyncContainerDescriptor<T> Time(
      Func<TransformTimeSyncDescriptor<T>, ITransformTimeSync> selector)
    {
      return this.Assign<ITransformTimeSync>(selector != null ? selector(new TransformTimeSyncDescriptor<T>()) : (ITransformTimeSync) null, (Action<ITransformSyncContainer, ITransformTimeSync>) ((a, v) => a.Time = v));
    }
  }
}
