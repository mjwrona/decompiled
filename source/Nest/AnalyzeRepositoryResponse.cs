// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeRepositoryResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AnalyzeRepositoryResponse : ResponseBase
  {
    [DataMember(Name = "blob_count")]
    public int BlobCount { get; internal set; }

    [DataMember(Name = "blob_path")]
    public string BlobPath { get; internal set; }

    [DataMember(Name = "concurrency")]
    public int Concurrency { get; internal set; }

    [DataMember(Name = "coordinating_node")]
    public NodeIdentity CoordinatingNode { get; internal set; }

    [DataMember(Name = "delete_elapsed")]
    public string DeleteElapsed { get; internal set; }

    [DataMember(Name = "delete_elapsed_nanos")]
    public long DeleteElapsedNanos { get; internal set; }

    [DataMember(Name = "details")]
    public IEnumerable<AnalysisDetails> Details { get; internal set; }

    [DataMember(Name = "early_read_node_count")]
    public int EarlyReadNodeCount { get; internal set; }

    [DataMember(Name = "issues_detected")]
    public IEnumerable<string> IssuesDetected { get; internal set; }

    [DataMember(Name = "listing_elapsed")]
    public string ListingElapsed { get; internal set; }

    [DataMember(Name = "listing_elapsed_nanos")]
    public long ListingElapsedNanos { get; internal set; }

    [DataMember(Name = "max_blob_size")]
    public string MaxBlobSize { get; internal set; }

    [DataMember(Name = "max_blob_size_bytes")]
    public long MaxBlobSizeBytes { get; internal set; }

    [DataMember(Name = "max_total_data_size")]
    public string MaxTotalDataSize { get; internal set; }

    [DataMember(Name = "max_total_data_size_bytes")]
    public long MaxTotalDataSizeBytes { get; internal set; }

    [DataMember(Name = "rare_action_probability")]
    public double RareActionProbability { get; internal set; }

    [DataMember(Name = "read_node_count")]
    public int ReadNodeCount { get; internal set; }

    [DataMember(Name = "repository")]
    public string Repository { get; internal set; }

    [DataMember(Name = "seed")]
    public long Seed { get; internal set; }

    [DataMember(Name = "summary")]
    public Summary Summary { get; internal set; }
  }
}
