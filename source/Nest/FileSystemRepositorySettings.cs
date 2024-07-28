// Decompiled with JetBrains decompiler
// Type: Nest.FileSystemRepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class FileSystemRepositorySettings : IFileSystemRepositorySettings, IRepositorySettings
  {
    internal FileSystemRepositorySettings()
    {
    }

    public FileSystemRepositorySettings(string location) => this.Location = location;

    public string ChunkSize { get; set; }

    public bool? Compress { get; set; }

    public int? ConcurrentStreams { get; set; }

    public string Location { get; set; }

    public bool? ReadOnly { get; set; }

    public string RestoreBytesPerSecondMaximum { get; set; }

    public string SnapshotBytesPerSecondMaximum { get; set; }
  }
}
