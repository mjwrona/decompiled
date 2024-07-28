// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer.TfvcCodeIndexingStalenessAnalyzer
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
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer
{
  public class TfvcCodeIndexingStalenessAnalyzer : CodeIndexingStalenessAnalyzer
  {
    private TraceMetaData m_traceMetaData;
    private TfvcRepositoryStalenessAnalyzer m_tfvcRepositoryStalenessAnalyzer;

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    protected override string IndexingUnitType => "TFVC_Repository";

    public TfvcCodeIndexingStalenessAnalyzer(
      IIndexingUnitDataAccess indexingUnitDataAccess,
      TfvcRepositoryStalenessAnalyzer tfvcRepositoryStalenessAnalyzer,
      TraceMetaData traceMetaData)
      : base(indexingUnitDataAccess)
    {
      this.m_tfvcRepositoryStalenessAnalyzer = tfvcRepositoryStalenessAnalyzer ?? throw new ArgumentNullException(nameof (tfvcRepositoryStalenessAnalyzer));
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
        string str = empty + FormattableString.Invariant(FormattableStringFactory.Create("Total number of Tfvc repositories detected = {0}.", (object) indexingUnits.Count));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepository in indexingUnits)
        {
          try
          {
            Dictionary<string, CodeIndexingStalenessData> scopePathToStalenessData = this.FetchStalenessForTfvcRepository(executionContext, tfvcRepository, utcNow);
            if (scopePathToStalenessData != null)
            {
              List<CustomerIntelligenceData> indexingStalenessCiData = this.GetCodeIndexingStalenessCIData(tfvcRepository, scopePathToStalenessData);
              this.PublishCITelemetry(executionContext, indexingStalenessCiData);
              ++num1;
            }
            else
              ++num2;
          }
          catch (Exception ex)
          {
            ++num2;
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("AnalyzeCodeIndexingStaleness failed for tfvc repository IndexingUnitId = {0} with exception = {1}.", (object) tfvcRepository.IndexingUnitId, (object) ex.ToString())));
          }
        }
        message = str + FormattableString.Invariant(FormattableStringFactory.Create("No. of Tfvc Repositories successfully processed = {0}.", (object) num1)) + FormattableString.Invariant(FormattableStringFactory.Create("No. of Tfvc Repositories unsuccessfully processed = {0}.", (object) num2));
      }
      else
      {
        message = empty + FormattableString.Invariant(FormattableStringFactory.Create("AnalyzeCodeIndexingStaleness exiting since fetched allTfvcRepositoriesInCollection is null."));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, message);
      }
      return message;
    }

    internal virtual Dictionary<string, CodeIndexingStalenessData> FetchStalenessForTfvcRepository(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepository,
      DateTime referenceTime)
    {
      if (tfvcRepository == null)
        throw new ArgumentNullException(nameof (tfvcRepository));
      if (executionContext == null)
        throw new ArgumentNullException(nameof (executionContext));
      Dictionary<string, CodeIndexingStalenessData> dictionary = new Dictionary<string, CodeIndexingStalenessData>();
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) tfvcRepository.Properties;
      TfvcCodeRepoTFSAttributes entityAttributes = (TfvcCodeRepoTFSAttributes) tfvcRepository.TFSEntityAttributes;
      if (properties == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, "tfvcRepositoryIndexingProperties found null.");
      }
      else
      {
        string repositoryName = entityAttributes.RepositoryName;
        CodeIndexingStalenessData stalenessData = this.m_tfvcRepositoryStalenessAnalyzer.GetStalenessData(tfvcRepository.TFSEntityId, repositoryName, properties.LastIndexedChangeSetId, referenceTime);
        if (stalenessData == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.m_traceMetaData, "FetchStalenessForTfvcRepository: codeIndexingStalenessData found null from TfvcRepositoryStalenessAnalyzer.GetStalenessData.");
          return (Dictionary<string, CodeIndexingStalenessData>) null;
        }
        dictionary.Add(repositoryName, stalenessData);
      }
      return dictionary;
    }

    internal virtual List<CustomerIntelligenceData> GetCodeIndexingStalenessCIData(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepository,
      Dictionary<string, CodeIndexingStalenessData> scopePathToStalenessData)
    {
      List<CustomerIntelligenceData> indexingStalenessCiData = new List<CustomerIntelligenceData>();
      foreach (KeyValuePair<string, CodeIndexingStalenessData> keyValuePair in scopePathToStalenessData)
      {
        string key = keyValuePair.Key;
        CodeIndexingStalenessData indexingStalenessData = keyValuePair.Value;
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("CIProcessingDelaySource", nameof (TfvcCodeIndexingStalenessAnalyzer));
        intelligenceData.Add("IndexingUnitType", this.IndexingUnitType);
        intelligenceData.Add("IsShadow", tfvcRepository.IsShadow.ToString());
        intelligenceData.Add("RepositoryId", tfvcRepository.TFSEntityId.ToString());
        intelligenceData.Add("ScopePath", key);
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
