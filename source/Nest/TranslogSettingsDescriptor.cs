// Decompiled with JetBrains decompiler
// Type: Nest.TranslogSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TranslogSettingsDescriptor : 
    DescriptorBase<TranslogSettingsDescriptor, ITranslogSettings>,
    ITranslogSettings
  {
    TranslogDurability? ITranslogSettings.Durability { get; set; }

    ITranslogFlushSettings ITranslogSettings.Flush { get; set; }

    Time ITranslogSettings.SyncInterval { get; set; }

    public TranslogSettingsDescriptor Flush(
      Func<TranslogFlushSettingsDescriptor, ITranslogFlushSettings> selector)
    {
      return this.Assign<Func<TranslogFlushSettingsDescriptor, ITranslogFlushSettings>>(selector, (Action<ITranslogSettings, Func<TranslogFlushSettingsDescriptor, ITranslogFlushSettings>>) ((a, v) => a.Flush = v != null ? v(new TranslogFlushSettingsDescriptor()) : (ITranslogFlushSettings) null));
    }

    public TranslogSettingsDescriptor Durability(TranslogDurability? durability) => this.Assign<TranslogDurability?>(durability, (Action<ITranslogSettings, TranslogDurability?>) ((a, v) => a.Durability = v));

    public TranslogSettingsDescriptor SyncInterval(Time time) => this.Assign<Time>(time, (Action<ITranslogSettings, Time>) ((a, v) => a.SyncInterval = v));
  }
}
