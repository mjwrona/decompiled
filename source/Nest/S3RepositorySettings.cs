// Decompiled with JetBrains decompiler
// Type: Nest.S3RepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class S3RepositorySettings : IS3RepositorySettings, IRepositorySettings
  {
    internal S3RepositorySettings()
    {
    }

    public S3RepositorySettings(string bucket) => this.Bucket = bucket;

    public string BasePath { get; set; }

    public string Bucket { get; set; }

    public string BufferSize { get; set; }

    public string CannedAcl { get; set; }

    public string ChunkSize { get; set; }

    public string Client { get; set; }

    public bool? Compress { get; set; }

    public bool? ServerSideEncryption { get; set; }

    public string StorageClass { get; set; }

    public bool? PathStyleAccess { get; set; }

    public bool? DisableChunkedEncoding { get; set; }

    public bool? ReadOnly { get; set; }

    public string MaxRestoreBytesPerSecond { get; set; }

    public string MaxSnapshotBytesPerSecond { get; set; }
  }
}
