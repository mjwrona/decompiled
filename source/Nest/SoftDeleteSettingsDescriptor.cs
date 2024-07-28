// Decompiled with JetBrains decompiler
// Type: Nest.SoftDeleteSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SoftDeleteSettingsDescriptor : 
    DescriptorBase<SoftDeleteSettingsDescriptor, ISoftDeleteSettings>,
    ISoftDeleteSettings
  {
    bool? ISoftDeleteSettings.Enabled { get; set; }

    ISoftDeleteRetentionSettings ISoftDeleteSettings.Retention { get; set; }

    public SoftDeleteSettingsDescriptor Retention(
      Func<SoftDeleteRetentionSettingsDescriptor, ISoftDeleteRetentionSettings> selector)
    {
      return this.Assign<ISoftDeleteRetentionSettings>(selector(new SoftDeleteRetentionSettingsDescriptor()), (Action<ISoftDeleteSettings, ISoftDeleteRetentionSettings>) ((a, v) => a.Retention = v));
    }

    [Obsolete("Creating indices with soft-deletes disabled is deprecated and will be removed in future Elasticsearch versions. Do not set a value of 'false'")]
    public SoftDeleteSettingsDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ISoftDeleteSettings, bool?>) ((a, v) => a.Enabled = v));
  }
}
