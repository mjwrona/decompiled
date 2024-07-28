// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.AggregateV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  [CollectSizeInformation("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.BytesPerAggregateWritten", "Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.AggregatesWrittenBase")]
  public class AggregateV3
  {
    internal static readonly DateTime DefaultTimeStamp = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public AggregateV3()
    {
      this.Metadata = new AggregateV3.AggregateMetadataV3();
      this.Details = new FileDetailsResultV3();
      this.SourceControlData = new SourceControlDataV3();
      this.CodeElements = new CodeElementIdentityCollectionV3();
    }

    public AggregateV3(
      AggregateV3.AggregateMetadataV3 metadata,
      FileDetailsResultV3 details,
      SourceControlDataV3 sourceControlData,
      CodeElementIdentityCollectionV3 codeElements)
    {
      this.Metadata = metadata;
      this.Details = details;
      this.SourceControlData = sourceControlData;
      this.CodeElements = codeElements;
    }

    public AggregateV3(Aggregate oldAggregate, int retentionPeriod)
      : this()
    {
      this.Metadata = new AggregateV3.AggregateMetadataV3(oldAggregate.Metadata);
      this.FetchSourceControlData(oldAggregate.Metadata.BranchLinks);
      this.FetchDetails(oldAggregate.Details);
    }

    [JsonProperty]
    [JsonConverter(typeof (EscapedStringJsonConverter))]
    public FileDetailsResultV3 Details { get; private set; }

    [JsonProperty]
    public CodeElementIdentityCollectionV3 CodeElements { get; private set; }

    [JsonProperty]
    public SourceControlDataV3 SourceControlData { get; private set; }

    [JsonIgnore]
    public IEnumerable<string> ContributingServerPaths => (IEnumerable<string>) this.Metadata.IncludedChanges.Keys;

    [JsonProperty]
    public AggregateV3.AggregateMetadataV3 Metadata { get; set; }

    public static AggregateV3 FromContributor(string contributingPath) => new AggregateV3()
    {
      Metadata = {
        IncludedChanges = {
          [contributingPath] = new HashSet<int>()
        }
      }
    };

    public void Merge(AggregateV3 otherAggregate)
    {
      if (otherAggregate == null)
        return;
      this.MergeCodeElementIdentities(otherAggregate.CodeElements, otherAggregate.Details.BranchList);
      this.Details.Merge(otherAggregate.Details);
      this.Metadata.Merge(otherAggregate.Metadata);
      this.SourceControlData.Merge(otherAggregate.SourceControlData);
    }

    private void MergeCodeElementIdentities(
      CodeElementIdentityCollectionV3 otherIdentities,
      List<BranchDetailsResultV3> otherBranchList)
    {
      Dictionary<int, int> dictionary = this.CodeElements.Merge(otherIdentities);
      foreach (BranchDetailsResultV3 otherBranch in otherBranchList)
      {
        foreach (CodeElementDetailsResultV3 detail in otherBranch.Details)
          detail.Id = dictionary[detail.Id];
      }
    }

    private bool ContainsChange(string serverPath, int change) => this.Metadata.IncludedChanges.ContainsKey(serverPath) && this.Metadata.IncludedChanges[serverPath].Contains(change);

    public void AddChange(FileChangeSliceResultV3 addedSlice)
    {
      if (!this.ContainsChange(addedSlice.AggregatePath, addedSlice.ChangesId))
      {
        this.Metadata.AddChange(addedSlice.AggregatePath, addedSlice.ChangesId);
        this.Metadata.AddBranchLinks(addedSlice.BranchLinks);
        this.MergeCodeElements(addedSlice);
        if (addedSlice.Details.Any<CodeElementDetailsResultV3>())
          this.Details.MergeCodeElements(addedSlice.AggregatePath, addedSlice.Details);
      }
      this.SourceControlData.Merge(addedSlice.SourceControlData);
    }

    public int Filter(int retentionPeriod) => FilterUtilities.ByRetentionV3(retentionPeriod, this.SourceControlData, this.CodeElements, this.Details.BranchList, this.Metadata.BranchLinks, this.Metadata.IncludedChanges);

    private void MergeCodeElements(FileChangeSliceResultV3 addedSlice)
    {
      Dictionary<int, int> dictionary = this.CodeElements.Merge(addedSlice.CodeElementIdentities);
      foreach (CodeElementDetailsResultV3 detail in addedSlice.Details)
        detail.Id = dictionary[detail.Id];
    }

    private void FetchSourceControlData(IEnumerable<BranchLinkData> branchLinks)
    {
      foreach (BranchLinkData branchLink in branchLinks)
      {
        if (!string.IsNullOrEmpty(branchLink.TargetChangesId))
          this.SourceControlData.UpdateChangeset(new CommitDataV3(branchLink.TargetChangesId, branchLink.ChangesComment, branchLink.AuthorUniqueName, branchLink.Date, (IEnumerable<int>) null));
        if (!string.IsNullOrEmpty(branchLink.AuthorUniqueName))
          this.SourceControlData.UpdateUser(new UserData(branchLink.AuthorUniqueName, branchLink.AuthorDisplayName, branchLink.AuthorEmail));
      }
    }

    private void FetchDetails(FileDetailsResult oldDetails)
    {
      List<BranchDetailsResultV3> branchList = new List<BranchDetailsResultV3>();
      foreach (BranchDetailsResult branch in oldDetails.BranchList)
      {
        Dictionary<int, CodeElementChangeResultV3> source = new Dictionary<int, CodeElementChangeResultV3>();
        foreach (FileChangeAggregateResult detail in branch.Details)
        {
          foreach (CodeElementDetailsResult codeElement in detail.CodeElements)
          {
            int key = this.CodeElements.Add(new CodeElementIdentityV3(codeElement.Id, codeElement.Kind));
            foreach (CollectorResult collectorResult in codeElement.ElementDetails.Where<CollectorResult>((Func<CollectorResult, bool>) (e => e.Id == "Microsoft.Changes")))
            {
              CodeElementChangeResult data = collectorResult.GetData<CodeElementChangeResult>();
              if (data != null)
              {
                this.SourceControlData.FetchData(data);
                if (!source.ContainsKey(key))
                  source.Add(key, new CodeElementChangeResultV3());
                source[key].Add(data.ChangeKind, data.ChangesId);
              }
            }
          }
        }
        List<CodeElementDetailsResultV3> list = source.Select<KeyValuePair<int, CodeElementChangeResultV3>, CodeElementDetailsResultV3>((Func<KeyValuePair<int, CodeElementChangeResultV3>, CodeElementDetailsResultV3>) (c => new CodeElementDetailsResultV3(c.Key, (IEnumerable<CollectorResult>) new CollectorResult[1]
        {
          new CollectorResult("Microsoft.Changes", (object) c.Value, "3.0")
        }))).ToList<CodeElementDetailsResultV3>();
        branchList.Add(new BranchDetailsResultV3(branch.ServerPath, list));
      }
      this.Details = new FileDetailsResultV3(branchList);
    }

    public class AggregateMetadataV3
    {
      private IDictionary<string, HashSet<int>> includedChanges = (IDictionary<string, HashSet<int>>) new Dictionary<string, HashSet<int>>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);

      [JsonConstructor]
      public AggregateMetadataV3()
        : this(new List<BranchLinkDataV3>())
      {
      }

      public AggregateMetadataV3(List<BranchLinkDataV3> branchList)
      {
        this.BranchLinks = branchList;
        this.AggregateVersion = 7;
        this.TimeStamp = AggregateV3.DefaultTimeStamp;
      }

      public AggregateMetadataV3(Aggregate.AggregateMetadata oldMetadata)
        : this(oldMetadata.BranchLinks.Select<BranchLinkData, BranchLinkDataV3>((Func<BranchLinkData, BranchLinkDataV3>) (l => new BranchLinkDataV3(l))).ToList<BranchLinkDataV3>())
      {
        this.includedChanges = oldMetadata.IncludedChanges;
      }

      [JsonProperty]
      public DateTime TimeStamp { get; private set; }

      [JsonProperty]
      public IDictionary<string, HashSet<int>> IncludedChanges
      {
        get => this.includedChanges;
        set => this.includedChanges = value != null ? value : (IDictionary<string, HashSet<int>>) new Dictionary<string, HashSet<int>>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      }

      [JsonProperty]
      public List<BranchLinkDataV3> BranchLinks { get; private set; }

      [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
      public int AggregateVersion { get; set; }

      public void AddChange(string serverPath, int addedChange)
      {
        if (this.includedChanges.ContainsKey(serverPath))
          this.includedChanges[serverPath].Add(addedChange);
        else
          this.includedChanges.Add(serverPath, new HashSet<int>((IEnumerable<int>) new int[1]
          {
            addedChange
          }));
      }

      public void AddChanges(IDictionary<string, HashSet<int>> addedChanges)
      {
        foreach (string key in (IEnumerable<string>) addedChanges.Keys)
        {
          if (this.includedChanges.ContainsKey(key))
            this.includedChanges[key].UnionWith((IEnumerable<int>) addedChanges[key]);
          else
            this.includedChanges.Add(key, addedChanges[key]);
        }
      }

      public void AddBranchLinks(IEnumerable<BranchLinkDataV3> branchLinks) => this.BranchLinks.AddRange(branchLinks);

      public void Merge(AggregateV3.AggregateMetadataV3 otherMetadata)
      {
        this.AddChanges(otherMetadata.IncludedChanges);
        this.AddBranchLinks((IEnumerable<BranchLinkDataV3>) otherMetadata.BranchLinks);
      }

      internal void UpdateTimeStamp()
      {
        DateTime utcNow = DateTime.UtcNow;
        this.TimeStamp = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);
      }
    }
  }
}
