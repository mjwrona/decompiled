// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPluginMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPluginMetadataAggregationAccessor : 
    IMavenPluginMetadataStoreAggregationAccessor,
    IAggregationAccessor<MavenPluginMetadataAggregation>,
    IAggregationAccessor,
    IMavenPluginMetadataStore
  {
    private readonly IWritableMavenPluginMetadataStore pluginStore;

    public MavenPluginMetadataAggregationAccessor(
      IAggregation aggregation,
      IWritableMavenPluginMetadataStore pluginStore)
    {
      this.Aggregation = aggregation;
      this.pluginStore = pluginStore;
    }

    public IAggregation Aggregation { get; }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      foreach (ICommitOperationData batchCommitOperation in (IEnumerable<ICommitOperationData>) this.ExpandBatchCommitOperations(commitLogEntries))
      {
        // ISSUE: explicit non-virtual call
        if (batchCommitOperation is MavenCommitOperationData addOp && __nonvirtual (addOp.Pom)?.Packaging != null && addOp.Pom.Packaging.Equals("maven-plugin", StringComparison.OrdinalIgnoreCase))
          await this.pluginStore.AppendPluginDataAsync(feedRequest.Feed, addOp.Pom, addOp.Identity, addOp.GetIngestionDirection());
      }
    }

    private IReadOnlyList<ICommitOperationData> ExpandBatchCommitOperations(
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      List<ICommitOperationData> commitOperationDataList = new List<ICommitOperationData>();
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
      {
        if (commitLogEntry.CommitOperationData is BatchCommitOperationData commitOperationData)
          commitOperationDataList.AddRange(commitOperationData.Operations);
        else
          commitOperationDataList.Add(commitLogEntry.CommitOperationData);
      }
      return (IReadOnlyList<ICommitOperationData>) commitOperationDataList;
    }

    public Task<MavenPluginList> GetPluginListAsync(FeedCore feed, string groupId) => this.pluginStore.GetPluginListAsync(feed, groupId);
  }
}
