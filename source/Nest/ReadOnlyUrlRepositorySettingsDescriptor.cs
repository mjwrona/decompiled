// Decompiled with JetBrains decompiler
// Type: Nest.ReadOnlyUrlRepositorySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ReadOnlyUrlRepositorySettingsDescriptor : 
    DescriptorBase<ReadOnlyUrlRepositorySettingsDescriptor, IReadOnlyUrlRepositorySettings>,
    IReadOnlyUrlRepositorySettings,
    IRepositorySettings
  {
    int? IReadOnlyUrlRepositorySettings.ConcurrentStreams { get; set; }

    string IReadOnlyUrlRepositorySettings.Location { get; set; }

    public ReadOnlyUrlRepositorySettingsDescriptor Location(string location) => this.Assign<string>(location, (Action<IReadOnlyUrlRepositorySettings, string>) ((a, v) => a.Location = v));

    public ReadOnlyUrlRepositorySettingsDescriptor ConcurrentStreams(int? concurrentStreams) => this.Assign<int?>(concurrentStreams, (Action<IReadOnlyUrlRepositorySettings, int?>) ((a, v) => a.ConcurrentStreams = v));
  }
}
