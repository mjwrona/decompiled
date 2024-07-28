// Decompiled with JetBrains decompiler
// Type: Nest.ReadOnlyUrlRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ReadOnlyUrlRepositoryDescriptor : 
    DescriptorBase<ReadOnlyUrlRepositoryDescriptor, IReadOnlyUrlRepository>,
    IReadOnlyUrlRepository,
    IRepository<IReadOnlyUrlRepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    IReadOnlyUrlRepositorySettings IRepository<IReadOnlyUrlRepositorySettings>.Settings { get; set; }

    object IRepositoryWithSettings.DelegateSettings => (object) this.Self.Settings;

    string ISnapshotRepository.Type => "url";

    public ReadOnlyUrlRepositoryDescriptor Settings(
      string location,
      Func<ReadOnlyUrlRepositorySettingsDescriptor, IReadOnlyUrlRepositorySettings> settingsSelector = null)
    {
      return this.Assign<IReadOnlyUrlRepositorySettings>(settingsSelector.InvokeOrDefault<ReadOnlyUrlRepositorySettingsDescriptor, IReadOnlyUrlRepositorySettings>(new ReadOnlyUrlRepositorySettingsDescriptor().Location(location)), (Action<IReadOnlyUrlRepository, IReadOnlyUrlRepositorySettings>) ((a, v) => a.Settings = v));
    }
  }
}
