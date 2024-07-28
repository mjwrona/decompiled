// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ISearchClusterManagementService
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public interface ISearchClusterManagementService
  {
    CloseIndexResponse CloseIndex(ExecutionContext executionContext, IndexIdentity indexIdentity);

    OpenIndexResponse OpenIndex(ExecutionContext executionContext, IndexIdentity indexIdentity);

    long GetIndexSizeInBytes(ExecutionContext executionContext, string indexName);

    long GetShardSizeInBytes(ExecutionContext executionContext, string indexName, string shard);

    string GetSmallestShard(ExecutionContext executionContext, string indexName);

    IEnumerable<KeyValuePair<string, double>> GetIndexShardsSizeInBytes(
      ExecutionContext executionContext,
      string indexName);

    Task<ForceMergeResponse> ForceMergeIndicesAsync(List<string> indices);

    List<KeyValuePair<string, long>> GetFieldWiseDocumentCount(
      ExecutionContext executionContext,
      string indexName,
      DocumentContractType contractType,
      string fieldName,
      string routing = null);

    NodesStatsResponse GetClusterResourceUsage();

    IDictionary<string, string> GetFieldValues(
      string termName,
      string termValue,
      string[] fields,
      string indexName,
      DocumentContractType contractType,
      string routing = null);

    List<EsShardDetails> GetShardsDetails(ExecutionContext executionContext, string indexName);

    string GetClusterName();
  }
}
