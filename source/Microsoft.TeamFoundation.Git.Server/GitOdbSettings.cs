// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitOdbSettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitOdbSettings
  {
    private readonly int? m_maxRenameDetectionFileSize;
    private readonly int? m_stablePackfileCapSize;
    private readonly int? m_unstablePackfileCapSize;
    private readonly int? m_maxMemoryStreamBytes;
    private readonly int? m_maxMemoryMappedFileBytes;
    private readonly int? m_indexTransactionLeaseRenewCount;
    private readonly long? m_tipIndexSize;
    private readonly long? m_stagingIndexSize;
    private readonly bool? m_useShardedHashsetForROR;

    public GitOdbSettings(
      int? maxRenameDetectionFileSize = null,
      int? stablePackfileCapSize = null,
      int? unstablePackfileCapSize = null,
      int? maxMemoryStreamBytes = null,
      int? maxMemoryMappedFileBytes = null,
      int? indexTransactionLeaseRenewCount = null,
      long? tipIndexSize = null,
      long? stagingIndexSize = null,
      bool? useShardedHashsetForROR = null)
    {
      this.m_maxRenameDetectionFileSize = maxRenameDetectionFileSize;
      this.m_stablePackfileCapSize = stablePackfileCapSize;
      this.m_unstablePackfileCapSize = unstablePackfileCapSize;
      this.m_maxMemoryStreamBytes = maxMemoryStreamBytes;
      this.m_maxMemoryMappedFileBytes = maxMemoryMappedFileBytes;
      this.m_indexTransactionLeaseRenewCount = indexTransactionLeaseRenewCount;
      this.m_tipIndexSize = tipIndexSize;
      this.m_stagingIndexSize = stagingIndexSize;
      this.m_useShardedHashsetForROR = useShardedHashsetForROR;
    }

    public int MaxRenameDetectionFileSize => this.m_maxRenameDetectionFileSize.GetValueOrDefault(5242880);

    public int StablePackfileCapSize => this.m_stablePackfileCapSize.GetValueOrDefault(4194304);

    public int UnstablePackfileCapSize => this.m_unstablePackfileCapSize.GetValueOrDefault(1048576);

    public int MaxMemoryStreamBytes => this.m_maxMemoryStreamBytes.GetValueOrDefault(262144);

    public int MaxMemoryMappedFileBytes => this.m_maxMemoryMappedFileBytes.GetValueOrDefault(16777216);

    public int IndexTransactionLeaseRenewCount => this.m_indexTransactionLeaseRenewCount.GetValueOrDefault(10);

    public long TipIndexSize => this.m_tipIndexSize.GetValueOrDefault(65536L);

    public long StagingIndexSize => this.m_stagingIndexSize.GetValueOrDefault(8388608L);

    public bool UseShardedHashsetForROR => this.m_useShardedHashsetForROR.GetValueOrDefault(false);
  }
}
