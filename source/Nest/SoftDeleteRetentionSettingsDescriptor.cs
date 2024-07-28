// Decompiled with JetBrains decompiler
// Type: Nest.SoftDeleteRetentionSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SoftDeleteRetentionSettingsDescriptor : 
    DescriptorBase<SoftDeleteRetentionSettingsDescriptor, ISoftDeleteRetentionSettings>,
    ISoftDeleteRetentionSettings
  {
    long? ISoftDeleteRetentionSettings.Operations { get; set; }

    public SoftDeleteRetentionSettingsDescriptor Operations(long? operations) => this.Assign<long?>(operations, (Action<ISoftDeleteRetentionSettings, long?>) ((a, v) => a.Operations = v));
  }
}
