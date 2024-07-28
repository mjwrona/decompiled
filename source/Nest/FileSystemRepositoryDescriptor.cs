// Decompiled with JetBrains decompiler
// Type: Nest.FileSystemRepositoryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FileSystemRepositoryDescriptor : 
    DescriptorBase<FileSystemRepositoryDescriptor, IFileSystemRepository>,
    IFileSystemRepository,
    IRepository<IFileSystemRepositorySettings>,
    IRepositoryWithSettings,
    ISnapshotRepository
  {
    object IRepositoryWithSettings.DelegateSettings => (object) this.Self.Settings;

    IFileSystemRepositorySettings IRepository<IFileSystemRepositorySettings>.Settings { get; set; }

    string ISnapshotRepository.Type { get; } = "fs";

    public FileSystemRepositoryDescriptor Settings(
      string location,
      Func<FileSystemRepositorySettingsDescriptor, IFileSystemRepositorySettings> settingsSelector = null)
    {
      return this.Assign<IFileSystemRepositorySettings>(settingsSelector.InvokeOrDefault<FileSystemRepositorySettingsDescriptor, IFileSystemRepositorySettings>(new FileSystemRepositorySettingsDescriptor().Location(location)), (Action<IFileSystemRepository, IFileSystemRepositorySettings>) ((a, v) => a.Settings = v));
    }
  }
}
