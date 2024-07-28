// Decompiled with JetBrains decompiler
// Type: Nest.AzureRepositorySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AzureRepositorySettingsDescriptor : 
    DescriptorBase<AzureRepositorySettingsDescriptor, IAzureRepositorySettings>,
    IAzureRepositorySettings,
    IRepositorySettings
  {
    string IAzureRepositorySettings.BasePath { get; set; }

    string IAzureRepositorySettings.ChunkSize { get; set; }

    bool? IAzureRepositorySettings.Compress { get; set; }

    string IAzureRepositorySettings.Container { get; set; }

    public AzureRepositorySettingsDescriptor Container(string container) => this.Assign<string>(container, (Action<IAzureRepositorySettings, string>) ((a, v) => a.Container = v));

    public AzureRepositorySettingsDescriptor BasePath(string basePath) => this.Assign<string>(basePath, (Action<IAzureRepositorySettings, string>) ((a, v) => a.BasePath = v));

    public AzureRepositorySettingsDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<IAzureRepositorySettings, bool?>) ((a, v) => a.Compress = v));

    public AzureRepositorySettingsDescriptor ChunkSize(string chunkSize) => this.Assign<string>(chunkSize, (Action<IAzureRepositorySettings, string>) ((a, v) => a.ChunkSize = v));
  }
}
