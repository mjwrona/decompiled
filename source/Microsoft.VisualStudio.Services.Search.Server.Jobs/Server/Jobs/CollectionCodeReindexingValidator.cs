// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionCodeReindexingValidator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class CollectionCodeReindexingValidator : ReindexingValidatorBase
  {
    public CollectionCodeReindexingValidator(IndexingExecutionContext executionContext)
      : base(executionContext)
    {
      this.CollectionIndexingUnit = executionContext.CollectionIndexingUnit;
      this.CollectionIndexingProperties = this.CollectionIndexingUnit.Properties as CollectionIndexingProperties;
    }

    protected Microsoft.VisualStudio.Services.Search.Common.IndexingUnit CollectionIndexingUnit { get; set; }

    protected CollectionIndexingProperties CollectionIndexingProperties { get; set; }

    public override bool ValidateReindexingCompleteness(StringBuilder resultMessage)
    {
      if (this.IndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        resultMessage.Append("Skipping the validation since it is OnPrem environment.");
        return true;
      }
      if (!this.CollectionIndexingProperties.IndexESConnectionString.Equals(this.IndexingExecutionContext.ServiceSettings.JobAgentSearchPlatformConnectionString))
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("IndexESConnectionString : '{0}' is not pointing to new cluster. ", (object) this.CollectionIndexingProperties.IndexESConnectionString)));
        return false;
      }
      bool flag1 = false;
      bool flag2 = false;
      if (this.IndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/ValidateDocumentCounts", true))
      {
        if (this.ValidateDocumentCounts(resultMessage))
        {
          resultMessage.Append("Doc counts are matching on both the clusters. ");
          flag1 = true;
        }
        else
          resultMessage.Append("Doc counts are not matching on both the clusters. ");
      }
      else
      {
        flag1 = true;
        resultMessage.Append("Skipping doc count validation.");
      }
      if (this.ValidateChangeEventStatus())
      {
        resultMessage.Append("Indexing events completed successfully. ");
        flag2 = true;
      }
      else
        resultMessage.Append("Indexing events failed. ");
      return flag1 & flag2;
    }

    internal virtual bool ValidateChangeEventStatus() => true;

    internal virtual bool ValidateDocumentCounts(StringBuilder resultMessage)
    {
      if (this.CollectionIndexingProperties.QueryIndices == null || this.CollectionIndexingProperties.QueryIndices.Count <= 0)
        return true;
      List<IndexInfo> indexInfoList = new List<IndexInfo>();
      indexInfoList.Add(new IndexInfo()
      {
        EntityName = this.CollectionIndexingProperties.Name,
        IndexName = this.CollectionIndexingProperties.IndexIndices[0].IndexName
      });
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> largeRepositories = this.GetLargeRepositories();
      if (largeRepositories != null)
      {
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in largeRepositories)
        {
          if (indexingUnit != null)
            indexInfoList.Add(new IndexInfo()
            {
              IndexName = indexingUnit.GetIndexInfo().IndexName,
              EntityName = indexingUnit.Properties.Name
            });
        }
      }
      int currentHostConfigValue1 = this.IndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/MaxRetriesForDocumentCountValidation", true, 5);
      int currentHostConfigValue2 = this.IndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/DocumentCountValidationRetryIntervalInSecs", true, 30);
      foreach (IndexInfo queryIndex in this.CollectionIndexingProperties.QueryIndices)
      {
        IndexInfo queryIndexInfo = queryIndex;
        string oldIndexName = queryIndexInfo?.IndexName;
        string newIndexName = indexInfoList.Find((Predicate<IndexInfo>) (x => !string.IsNullOrWhiteSpace(x.EntityName) && x.EntityName.Equals(queryIndexInfo?.EntityName)))?.IndexName;
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("OldIndexName: {0}, NewIndexName: {1}. ", (object) oldIndexName, (object) newIndexName)));
        if (!ExponentialBackoffRetryInvoker.Instance.Invoke<bool>((Func<object>) (() => (object) this.MatchQueryResultsOnBothClusters(oldIndexName, newIndexName, resultMessage)), currentHostConfigValue1, currentHostConfigValue2 * 1000, false, this.TraceMetadata))
          return false;
      }
      return true;
    }

    internal virtual bool MatchQueryResultsOnBothClusters(
      string indexNameOnOldCluster,
      string indexNameOnNewCluster,
      StringBuilder resultMessage)
    {
      if (string.IsNullOrWhiteSpace(indexNameOnOldCluster) || string.IsNullOrWhiteSpace(indexNameOnNewCluster))
        return true;
      ISearchPlatform searchPlatform = SearchPlatformFactory.GetInstance().Create(this.CollectionIndexingProperties.QueryESConnectionString, this.IndexingExecutionContext.ProvisioningContext.SearchPlatformSettings, this.IndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      IExpression queryExpression = (IExpression) new TermExpression("collectionId", Operator.Equals, this.IndexingExecutionContext.RequestContext.GetCollectionIdAsNormalizedString());
      long docCount1 = this.GetDocCount(searchPlatform, queryExpression, this.CollectionIndexingProperties.QueryContractType, indexNameOnOldCluster);
      long configValue1 = this.IndexingExecutionContext.RequestContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MinDocCountForReindexingValidation");
      if (docCount1 > configValue1)
      {
        long docCount2 = this.GetDocCount(this.IndexingExecutionContext.ProvisioningContext.SearchPlatform, queryExpression, this.CollectionIndexingProperties.IndexContractType, indexNameOnNewCluster);
        int configValue2 = this.IndexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxDocCountDiffPercentageForReindexingValidation");
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Doc count on old cluster is {0} and doc count on new cluster is {1}. ", (object) docCount1, (object) docCount2)));
        Tracer.PublishKpi("DocumentCountOnOldCluster", "Indexing Pipeline", (double) docCount1);
        Tracer.PublishKpi("DocumentCountOnNewCluster", "Indexing Pipeline", (double) docCount2);
        if (docCount2 < docCount1)
        {
          if ((double) (docCount1 - docCount2) * 100.0 / (double) docCount1 > (double) configValue2)
          {
            resultMessage.Append("Diff of indexed document count on old and new cluster is not under acceptable limit. ");
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Document Count validation failed.")));
          }
          resultMessage.Append("Diff of indexed document count on old and new cluster is under acceptable limit. ");
          return true;
        }
        resultMessage.Append("Indexed document count on new cluster is more than that in old cluster. ");
        return true;
      }
      resultMessage.Append("Number of indexed documents is less than MinDocCountForValidation, hence not validating. ");
      return true;
    }

    internal virtual IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetLargeRepositories() => new CollectionCodeFinalizeHelper().GetLargeChildrenIndexingUnitsToFinalize(this.IndexingExecutionContext, this.IndexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled());

    internal virtual long GetDocCount(
      ISearchPlatform searchPlatform,
      IExpression queryExpression,
      DocumentContractType contractType,
      string indexName)
    {
      return searchPlatform.GetNumberOfHitsCount(queryExpression, contractType, (IEnumerable<IndexInfo>) new List<IndexInfo>()
      {
        new IndexInfo() { IndexName = indexName }
      });
    }
  }
}
