// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemAreaNodeSecurityAcesSyncOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemAreaNodeSecurityAcesSyncOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080668, "Indexing Pipeline", "IndexingOperation");
    private readonly ClassificationNodeSecurityAcesUpdater m_securityAcesUpdater;
    private readonly IClassificationNodeDataAccess m_classificationNodeDataAccess;

    public CollectionWorkItemAreaNodeSecurityAcesSyncOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new ClassificationNodeSecurityAcesUpdater(executionContext))
    {
    }

    protected CollectionWorkItemAreaNodeSecurityAcesSyncOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      ClassificationNodeSecurityAcesUpdater securityAcesUpdater)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
      this.m_securityAcesUpdater = securityAcesUpdater;
      this.m_classificationNodeDataAccess = dataAccessFactory.GetClassificationNodeDataAccess();
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionWorkItemAreaNodeSecurityAcesSyncOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        List<ClassificationNode> classificationNodes = this.m_classificationNodeDataAccess.GetClassificationNodes(coreIndexingExecutionContext.RequestContext, ClassificationNodeType.Area, -1);
        this.UpdateSecurityHashIfApplicableAndPersistToDb(coreIndexingExecutionContext.RequestContext, classificationNodes, (string) null);
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Synced security hash for [{0}] area paths. ", (object) classificationNodes.Count)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionWorkItemAreaNodeSecurityAcesSyncOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual void UpdateSecurityHashIfApplicableAndPersistToDb(
      IVssRequestContext requestContext,
      List<ClassificationNode> nodes,
      string rootAreaNodeToken)
    {
      if (nodes.Count == 0)
        return;
      if (nodes.Any<ClassificationNode>((Func<ClassificationNode, bool>) (n => n.NodeType.Equals((object) ClassificationNodeType.Area))))
        nodes = this.m_securityAcesUpdater.UpdateClassificationNodesWithSecurityHashAndToken(nodes, rootAreaNodeToken);
      this.m_classificationNodeDataAccess.AddOrUpdateClassificationNodes(requestContext, nodes, true);
    }
  }
}
