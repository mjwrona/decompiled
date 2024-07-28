// Decompiled with JetBrains decompiler
// Type: Nest.AzureRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AzureRepositoryDescriptor : 
    DescriptorBase<AzureRepositoryDescriptor, IAzureRepository>,
    IAzureRepository,
    IRepository<IAzureRepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    IAzureRepositorySettings IRepository<IAzureRepositorySettings>.Settings { get; set; }

    string ISnapshotRepository.Type { get; } = "azure";

    object IRepositoryWithSettings.DelegateSettings => (object) this.Self.Settings;

    public AzureRepositoryDescriptor Settings(
      Func<AzureRepositorySettingsDescriptor, IAzureRepositorySettings> settingsSelector)
    {
      return this.Assign<Func<AzureRepositorySettingsDescriptor, IAzureRepositorySettings>>(settingsSelector, (Action<IAzureRepository, Func<AzureRepositorySettingsDescriptor, IAzureRepositorySettings>>) ((a, v) => a.Settings = v != null ? v(new AzureRepositorySettingsDescriptor()) : (IAzureRepositorySettings) null));
    }
  }
}
