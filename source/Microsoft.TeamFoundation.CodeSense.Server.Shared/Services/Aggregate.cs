// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.Aggregate
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  [CollectSizeInformation("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.BytesPerAggregateWritten", "Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.AggregatesWrittenBase")]
  public sealed class Aggregate
  {
    [JsonConstructor]
    public Aggregate()
    {
      this.Metadata = new Aggregate.AggregateMetadata();
      this.Details = new FileDetailsResult();
      this.Summary = new FileSummaryResult();
    }

    public Aggregate(
      Aggregate.AggregateMetadata metadata,
      FileDetailsResult details,
      FileSummaryResult summary)
    {
      this.Metadata = metadata;
      this.Details = details;
      this.Summary = summary;
    }

    [JsonProperty]
    [JsonConverter(typeof (EscapedStringJsonConverter))]
    public FileSummaryResult Summary { get; private set; }

    [JsonProperty]
    [JsonConverter(typeof (EscapedStringJsonConverter))]
    public FileDetailsResult Details { get; private set; }

    [JsonProperty]
    public Aggregate.AggregateMetadata Metadata { get; set; }

    public sealed class AggregateMetadata
    {
      private IDictionary<string, HashSet<int>> includedChanges;

      [JsonConstructor]
      public AggregateMetadata()
        : this(new Dictionary<string, HashSet<int>>(), (IEnumerable<BranchLinkData>) new List<BranchLinkData>())
      {
      }

      public AggregateMetadata(
        Dictionary<string, HashSet<int>> includedChanges,
        IEnumerable<BranchLinkData> branchLinks)
      {
        this.includedChanges = (IDictionary<string, HashSet<int>>) includedChanges;
        this.BranchLinks = branchLinks;
        this.AggregateVersion = 5;
      }

      [JsonProperty]
      public IDictionary<string, HashSet<int>> IncludedChanges
      {
        get => this.includedChanges;
        private set => this.includedChanges = value != null ? value : (IDictionary<string, HashSet<int>>) new Dictionary<string, HashSet<int>>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      }

      [JsonProperty]
      public IEnumerable<BranchLinkData> BranchLinks { get; private set; }

      [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
      public int AggregateVersion { get; private set; }
    }
  }
}
