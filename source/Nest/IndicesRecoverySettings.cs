// Decompiled with JetBrains decompiler
// Type: Nest.IndicesRecoverySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IndicesRecoverySettings : IIndicesRecoverySettings
  {
    public bool? Compress { get; set; }

    public int? ConcurrentSmallFileStreams { get; set; }

    public int? ConcurrentStreams { get; set; }

    public string FileChunkSize { get; set; }

    public string MaxBytesPerSecond { get; set; }

    public int? TranslogOperations { get; set; }

    public string TranslogSize { get; set; }
  }
}
