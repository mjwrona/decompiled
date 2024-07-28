// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.SourceNoDedupeFileContractBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.QueryBuilders;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public abstract class SourceNoDedupeFileContractBase : CodeFileContract
  {
    public string OriginalContent { get; set; }

    public string OrganizationName { get; set; }

    public string OrganizationNameOriginal { get; set; }

    public string OrganizationId { get; set; }

    public string ChangeId { get; set; }

    public string BranchNameOriginal { get; set; }

    public string BranchName { get; set; }

    protected SourceNoDedupeFileContractBase()
    {
    }

    protected SourceNoDedupeFileContractBase(ISearchQueryClient elasticClient)
      : base(elasticClient)
    {
    }

    public override void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      base.PopulateFileContractDetails(requestContext, indexingUnit, data, metaDataStore, parsedData, settings, originalContent);
      this.CollectionName = metaDataStore["NormalizedCollectionName"];
      this.CollectionNameOriginal = metaDataStore["CollectionName"];
      if (data is IMetaDataStoreItem metaDataStoreItem && metaDataStoreItem.BranchesInfo != null && metaDataStoreItem.BranchesInfo.Count == 1)
      {
        string branchName = metaDataStoreItem.BranchesInfo[0].BranchName;
        this.BranchName = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/".NormalizePathWithoutTrimming(), branchName.NormalizePath());
        this.BranchNameOriginal = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/", branchName);
        this.SetIsDefaultBranch(branchName.Equals(metaDataStore["DefaultBranchName"], StringComparison.Ordinal));
        this.ChangeId = metaDataStoreItem.BranchesInfo[0].ChangeId.NormalizeString();
      }
      else
      {
        string nameWithoutPrefix1 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/".NormalizePathWithoutTrimming(), metaDataStore["NormalizedBranchName"]);
        string nameWithoutPrefix2 = CodeFileContract.GetBranchNameWithoutPrefix("refs/heads/", metaDataStore["BranchName"]);
        this.BranchName = nameWithoutPrefix1;
        this.BranchNameOriginal = nameWithoutPrefix2;
        this.ChangeId = metaDataStore["NormalizedLatestCommitIdKey"];
        bool result;
        if (!bool.TryParse(metaDataStore["IsDefaultBranch"], out result))
          result = true;
        this.SetIsDefaultBranch(result);
      }
      if (originalContent == null)
        return;
      this.OriginalContent = this.GetOriginalContent(originalContent);
    }

    internal override string ConvertToAdvancedPhraseQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      bool enableRanking,
      string requestId)
    {
      return ElasticsearchQueryBuilder.BuildMatchPhraseQuery(this.GetSearchFieldForAdvancedQuery(requestContext).ElasticsearchFieldName, termExpression.Value);
    }

    protected virtual string GetOriginalContent(byte[] originalContent) => new TextEncoding().GetString(originalContent).ToLowerInvariant();

    protected abstract void SetIsDefaultBranch(bool isDefaultBranch);
  }
}
