// Decompiled with JetBrains decompiler
// Type: Nest.TriggerEventContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TriggerEventContainer : ITriggerEventContainer, IDescriptor
  {
    public TriggerEventContainer()
    {
    }

    public TriggerEventContainer(TriggerEventBase trigger)
    {
      trigger.ThrowIfNull<TriggerEventBase>(nameof (trigger));
      trigger.WrapInContainer((ITriggerEventContainer) this);
    }

    IScheduleTriggerEvent ITriggerEventContainer.Schedule { get; set; }
  }
}
