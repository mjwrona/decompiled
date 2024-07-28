// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer.GitCodeIndexingStalenessAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Crawler;
using Microsoft.VisualStudio.Services.Search.Crawler.StalenessAnalyzer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer
{
  public class GitCodeIndexingStalenessAnalyzer : CodeIndexingStalenessAnalyzer
  {
    private TraceMetaData m_traceMetaData;
    private GitRepositoryStalenessAnalyzer m_gitRepositoryStalenessAnalyzer;

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    protected override string IndexingUnitType => "Git_Repository";

    public GitCodeIndexingStalenessAnalyzer(
      IIndexingUnitDataAccess indexingUnitDataAccess,
      GitRepositoryStalenessAnalyzer gitRepositoryStalenessAnalyzer,
      TraceMetaData traceMetaData)
      : base(indexingUnitDataAccess)
    {
      this.m_gitRepositoryStalenessAnalyzer = gitRepositoryStalenessAnalyzer ?? throw new ArgumentNullException(nameof (gitRepositoryStalenessAnalyzer));
      this.m_traceMetaData = traceMetaData ?? throw new ArgumentNullException(nameof (traceMetaData));
    }

    public override string AnalyzeCodeIndexingStaleness(ExecutionContext executionContext)
    {
      string empty = string.Empty;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.GetIndexingUnits(executionContext);
      DateTime utcNow = DateTime.UtcNow;
      string message;
      if (indexingUnits != null)
      {
        int num1 = 0;
        int num2 = 0;
        string str = empty + FormattableString.Invariant(FormattableStringFactory.Create("Total number of Git repositories detected = {0}.", (object) indexingUnits.Count)) + FormattableString.Invariant(FormattableStringFactory.Create("Total number of Large Git repositories detected = {0}.", (object) indexingUnits.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => x.IsLargeRepository(executionContext.RequestContext))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().Count));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepository in indexingUnits)
        {
          try
          {
            if (!gitRepository.Properties.IsDisabled)
            {
              Dictionary<string, CodeIndexingStalenessData> branchStalenessData = this.FetchStalenessForGitRepository(executionContext, gitRepository, utcNow);
              List<CustomerIntelligenceData> indexingStalenessCiData = this.GetCodeIndexingStalenessCIData(gitRepository, branchStalenessData);
              this.PublishCITelemetry(executionContext, indexingStalenessCiData);
              ++num1;
            }
          }
          catch (Exception ex)
          {
            ++num2;
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("AnalyzeCodeIndexingStaleness failed for git large repository IndexingUnitId = {0} with exception = {1}.", (object) gitRepository.IndexingUnitId, (object) ex.ToString())));
          }
        }
        message = str + FormattableString.Invariant(FormattableStringFactory.Create("No. of Git Repositories successfully processed = {0}.", (object) num1)) + FormattableString.Invariant(FormattableStringFactory.Create("No. of Git Repositories unsuccessfully processed = {0}.", (object) num2));
      }
      else
      {
        message = empty + FormattableString.Invariant(FormattableStringFactory.Create("AnalyzeCodeIndexingStaleness exiting since fetched allGitRepositoriesInCollection is null."));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, message);
      }
      return message;
    }

    internal virtual Dictionary<string, CodeIndexingStalenessData> FetchStalenessForGitRepository(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepository,
      DateTime referenceTime)
    {
      if (gitRepository == null)
        throw new ArgumentNullException(nameof (gitRepository));
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      Dictionary<string, CodeIndexingStalenessData> dictionary = new Dictionary<string, CodeIndexingStalenessData>();
      GitCodeRepoIndexingProperties properties = (GitCodeRepoIndexingProperties) gitRepository.Properties;
      if (properties == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, "gitRepositoryIndexingProperties found null.");
      }
      else
      {
        foreach (KeyValuePair<string, GitBranchIndexInfo> keyValuePair in properties.BranchIndexInfo)
        {
          string key = keyValuePair.Key;
          GitBranchIndexInfo gitBranchIndexInfo = keyValuePair.Value;
          CodeIndexingStalenessData stalenessData = this.m_gitRepositoryStalenessAnalyzer.GetStalenessData(gitRepository.TFSEntityId, key, gitBranchIndexInfo.LastIndexedCommitId, referenceTime);
          if (stalenessData != null)
            dictionary[key] = stalenessData;
        }
      }
      return dictionary;
    }

    internal virtual List<CustomerIntelligenceData> GetCodeIndexingStalenessCIData(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepository,
      Dictionary<string, CodeIndexingStalenessData> branchStalenessData)
    {
      List<CustomerIntelligenceData> indexingStalenessCiData = new List<CustomerIntelligenceData>();
      foreach (KeyValuePair<string, CodeIndexingStalenessData> keyValuePair in branchStalenessData)
      {
        string key = keyValuePair.Key;
        CodeIndexingStalenessData indexingStalenessData = keyValuePair.Value;
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("CIProcessingDelaySource", nameof (GitCodeIndexingStalenessAnalyzer));
        intelligenceData.Add("IndexingUnitType", this.IndexingUnitType);
        intelligenceData.Add("IsShadow", gitRepository.IsShadow.ToString());
        intelligenceData.Add("RepositoryId", gitRepository.TFSEntityId.ToString());
        intelligenceData.Add("BranchName", key);
        intelligenceData.Add("LatestPushId", (double) indexingStalenessData.LatestPushInfo.PushId);
        intelligenceData.Add("LatestPushTime", (object) indexingStalenessData.LatestPushInfo.PushTime);
        intelligenceData.Add("LastProcessedPushId", (double) indexingStalenessData.LastProcessedPushInfo.PushId);
        intelligenceData.Add("LastProcessedPushTime", (object) indexingStalenessData.LastProcessedPushInfo.PushTime);
        intelligenceData.Add("FirstUnprocessedPushId", (double) indexingStalenessData.FirstUnprocessedPushInfo.PushId);
        intelligenceData.Add("FirstUnprocessedPushTime", (object) indexingStalenessData.FirstUnprocessedPushInfo.PushTime);
        intelligenceData.Add("ReferenceTimeForStalenessMeasurement", (object) indexingStalenessData.ReferenceTimeForStaleness);
        intelligenceData.Add("CIProcessingDelayInMiliseconds", indexingStalenessData.CIProcessingDelayInMiliseconds);
        indexingStalenessCiData.Add(intelligenceData);
      }
      return indexingStalenessCiData;
    }

    internal virtual void PublishCITelemetry(
      ExecutionContext executionContext,
      List<CustomerIntelligenceData> ciDataList)
    {
      foreach (CustomerIntelligenceData ciData in ciDataList)
        executionContext.ExecutionTracerContext.PublishCi(this.m_traceMetaData.TraceArea, this.m_traceMetaData.TraceLayer, ciData);
    }
  }
}
