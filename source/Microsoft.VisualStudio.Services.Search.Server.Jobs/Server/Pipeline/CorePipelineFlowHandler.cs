// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CorePipelineFlowHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class CorePipelineFlowHandler
  {
    public CorePipelineFlowHandler()
      : this((Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null, new TraceMetaData(1080675, "Indexing Pipeline", "Pipeline"))
    {
    }

    public CorePipelineFlowHandler(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, TraceMetaData traceMetaData)
    {
      this.IndexingUnit = indexingUnit;
      this.TraceMetaData = traceMetaData;
    }

    public virtual void Prepare(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
    }

    public virtual void PrePipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
    }

    public virtual void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
    }

    [StaticSafe("Grandfathered")]
    public static CorePipelineFlowHandler DefaultNoOpInstance => new CorePipelineFlowHandler();

    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    public TraceMetaData TraceMetaData { get; }

    public virtual void PublishIndexingCompletionSLA(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      ExecutionTracerContext executionTracerContext = coreIndexingExecutionContext.ExecutionTracerContext;
      TimeSpan timeSpan = DateTime.UtcNow.Subtract(executionTracerContext.TracerCICorrelationDetails.TriggerTimeUtc);
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
      {
        ["E2ETimeInSeconds"] = (object) timeSpan.TotalSeconds,
        ["IndexingUnitId"] = (object) coreIndexingExecutionContext.IndexingUnit.IndexingUnitId,
        ["ItemsProcessedAcrossJobYields"] = (object) crawlSpec.ItemsProcessedAcrossYields,
        ["JobYieldCount"] = (object) crawlSpec.JobYieldCount
      };
      executionTracerContext.PublishClientTrace(this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, (IDictionary<string, object>) properties, true);
    }

    public void UpdateRepoPropertiesWithLastIndexedInfo(
      TfvcCodeRepoIndexingProperties repoIndexingProperties,
      string lastIndexedChangeId,
      TfvcHttpClientWrapper tfvcHttpClientWrapper,
      string repositoryId,
      string flowHandlerName)
    {
      int changeSetId = -1;
      DateTime dateTime = repoIndexingProperties.RepositoryLastProcessedTime;
      if (!string.IsNullOrWhiteSpace(lastIndexedChangeId))
      {
        changeSetId = int.Parse(lastIndexedChangeId, (IFormatProvider) CultureInfo.InvariantCulture);
        if (tfvcHttpClientWrapper != null)
        {
          TfvcChangesetRef changeset = (TfvcChangesetRef) tfvcHttpClientWrapper.GetChangeset(repositoryId, changeSetId);
          if (changeset != null)
            dateTime = changeset.CreatedDate.ToUniversalTime();
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("{0}: lastIndexedChangeset found to be null for Repository Id: {1}, Repository Name: {2}, LastIndexedChangeId: {3}", (object) flowHandlerName, (object) repositoryId, (object) repoIndexingProperties.Name, (object) lastIndexedChangeId)));
        }
      }
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("{0}: LastIndexedChangeId found to be null/empty for Repository Id: {1}, Repository Name: {2}", (object) flowHandlerName, (object) repositoryId, (object) repoIndexingProperties.Name)));
      repoIndexingProperties.LastIndexedChangeSetId = changeSetId;
      repoIndexingProperties.RepositoryLastProcessedTime = dateTime;
    }
  }
}
