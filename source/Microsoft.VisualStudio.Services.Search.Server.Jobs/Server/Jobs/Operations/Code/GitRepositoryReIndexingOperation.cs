// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitRepositoryReIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitRepositoryReIndexingOperation : RepositoryDeleteOperation
  {
    private GitHttpClientWrapper m_gitClientWrapper;
    private IndexingExecutionContext m_executionContext;
    private const string s_traceArea = "Indexing Pipeline";
    private const string s_traceLayer = "Job";
    [StaticSafe]
    private static TraceMetaData s_traceMetaData = new TraceMetaData(1080637, "Indexing Pipeline", "Job");

    public GitRepositoryReIndexingOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_gitClientWrapper = new GitHttpClientWrapper((ExecutionContext) executionContext, GitRepositoryReIndexingOperation.s_traceMetaData);
      this.m_executionContext = (IndexingExecutionContext) executionContext;
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal GitRepositoryReIndexingOperation(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      GitHttpClientWrapper clientWrapper,
      IIndexingUnitDataAccess indexingUnitDataAccess)
      : base((CoreIndexingExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_gitClientWrapper = clientWrapper;
      this.m_executionContext = executionContext;
      this.IndexingUnitDataAccess = indexingUnitDataAccess;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080625, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult1 = new OperationResult();
      OperationResult operationResult2 = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult2.Status = OperationStatus.Succeeded;
          return operationResult2;
        }
        OperationResult operationResult3 = base.RunOperation(coreIndexingExecutionContext);
        resultMessage.AppendLine(operationResult3.Message);
        if (operationResult3.Status == OperationStatus.Succeeded)
        {
          operationResult2.Status = OperationStatus.Succeeded;
          if (this.m_innerOperationStatus == OperationStatus.Succeeded)
          {
            try
            {
              this.CreateAssignRoutingEvent(this.GetOrCreateIndexingUnit(executionContext, resultMessage), resultMessage);
            }
            catch (Exception ex)
            {
              operationResult2.Status = OperationStatus.Failed;
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("GitRepositoryReIndexingOperation for Indexing Unit {0} failed with error {1}", (object) this.IndexingUnit.TFSEntityId, (object) ex)));
            }
          }
        }
        else
        {
          if (operationResult3.Status != OperationStatus.FailedAndRetry)
          {
            if (operationResult3.Status != OperationStatus.Failed)
              goto label_11;
          }
          operationResult2.Status = operationResult3.Status;
        }
      }
      finally
      {
        operationResult2.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080625, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
label_11:
      return operationResult2;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetOrCreateIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = this.IndexingUnitDataAccess.GetIndexingUnit(this.m_executionContext.RequestContext, this.IndexingUnit.TFSEntityId, "Git_Repository", (IEntityType) CodeEntityType.GetInstance());
      if (indexingUnit1 == null)
      {
        GitRepository repositoryAsync = this.m_gitClientWrapper.GetRepositoryAsync(this.IndexingUnit.TFSEntityId);
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(repositoryAsync.Id, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), this.IndexingUnit.ParentUnitId);
        GitCodeRepoTFSAttributes repoTfsAttributes = new GitCodeRepoTFSAttributes();
        repoTfsAttributes.RepositoryName = repositoryAsync.Name;
        repoTfsAttributes.DefaultBranch = repositoryAsync.DefaultBranch;
        repoTfsAttributes.RemoteUrl = repositoryAsync.RemoteUrl;
        indexingUnit2.TFSEntityAttributes = (TFSEntityAttributes) repoTfsAttributes;
        GitCodeRepoIndexingProperties indexingProperties = new GitCodeRepoIndexingProperties();
        indexingProperties.Name = repositoryAsync.Name;
        indexingUnit2.Properties = (IndexingProperties) indexingProperties;
        indexingUnit1 = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(this.m_executionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
        {
          indexingUnit2
        }, true).First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        string message = FormattableString.Invariant(FormattableStringFactory.Create("Created IndexingUnit with IndexingUnitId - {0} for Repository {1}", (object) indexingUnit1.IndexingUnitId, (object) indexingUnit1.TFSEntityId));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(GitRepositoryReIndexingOperation.s_traceMetaData, message);
        resultMessage.Append(message);
      }
      else
      {
        string message = FormattableString.Invariant(FormattableStringFactory.Create("Erasing Indexing watermarks for {0}", (object) this.IndexingUnit));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(GitRepositoryReIndexingOperation.s_traceMetaData, message);
        resultMessage.Append(message);
        this.IndexingUnit.EraseIndexingWatermarksOfTree(indexingExecutionContext, this.IndexingUnitDataAccess);
      }
      return indexingUnit1;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateAssignRoutingEvent(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      StringBuilder resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent assignRoutingEvent = this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.m_executionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = gitRepoIndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) this.m_executionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
      string message = FormattableString.Invariant(FormattableStringFactory.Create("Created AssignRouting event [Id = {0} Repository Id = {1}]", (object) assignRoutingEvent.Id, (object) gitRepoIndexingUnit.TFSEntityId));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(GitRepositoryReIndexingOperation.s_traceMetaData, message);
      resultMessage.Append(message);
      return assignRoutingEvent;
    }
  }
}
