// Decompiled with JetBrains decompiler
// Type: Nest.FileSystemRepositorySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FileSystemRepositorySettingsDescriptor : 
    DescriptorBase<FileSystemRepositorySettingsDescriptor, IFileSystemRepositorySettings>,
    IFileSystemRepositorySettings,
    IRepositorySettings
  {
    string IFileSystemRepositorySettings.ChunkSize { get; set; }

    bool? IFileSystemRepositorySettings.Compress { get; set; }

    int? IFileSystemRepositorySettings.ConcurrentStreams { get; set; }

    string IFileSystemRepositorySettings.Location { get; set; }

    bool? IFileSystemRepositorySettings.ReadOnly { get; set; }

    string IFileSystemRepositorySettings.RestoreBytesPerSecondMaximum { get; set; }

    string IFileSystemRepositorySettings.SnapshotBytesPerSecondMaximum { get; set; }

    public FileSystemRepositorySettingsDescriptor Location(string location) => this.Assign<string>(location, (Action<IFileSystemRepositorySettings, string>) ((a, v) => a.Location = v));

    public FileSystemRepositorySettingsDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<IFileSystemRepositorySettings, bool?>) ((a, v) => a.Compress = v));

    public FileSystemRepositorySettingsDescriptor ConcurrentStreams(int? concurrentStreams) => this.Assign<int?>(concurrentStreams, (Action<IFileSystemRepositorySettings, int?>) ((a, v) => a.ConcurrentStreams = v));

    public FileSystemRepositorySettingsDescriptor ChunkSize(string chunkSize) => this.Assign<string>(chunkSize, (Action<IFileSystemRepositorySettings, string>) ((a, v) => a.ChunkSize = v));

    public FileSystemRepositorySettingsDescriptor ReadOnly(bool? readOnly = true) => this.Assign<bool?>(readOnly, (Action<IFileSystemRepositorySettings, bool?>) ((a, v) => a.ReadOnly = v));

    public FileSystemRepositorySettingsDescriptor RestoreBytesPerSecondMaximum(
      string maximumBytesPerSecond)
    {
      return this.Assign<string>(maximumBytesPerSecond, (Action<IFileSystemRepositorySettings, string>) ((a, v) => a.RestoreBytesPerSecondMaximum = v));
    }

    public FileSystemRepositorySettingsDescriptor SnapshotBytesPerSecondMaximum(
      string maximumBytesPerSecond)
    {
      return this.Assign<string>(maximumBytesPerSecond, (Action<IFileSystemRepositorySettings, string>) ((a, v) => a.SnapshotBytesPerSecondMaximum = v));
    }
  }
}
