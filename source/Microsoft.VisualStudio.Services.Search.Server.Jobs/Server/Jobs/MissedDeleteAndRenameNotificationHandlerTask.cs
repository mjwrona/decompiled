// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.MissedDeleteAndRenameNotificationHandlerTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class MissedDeleteAndRenameNotificationHandlerTask : IIndexingPatchTask
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080485, "Indexing Pipeline", "IndexingOperation");
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly bool m_entityCrudOperationsFeatureEnabled;
    private readonly string m_indexingUnitType;
    private readonly IEntityType m_entityType;
    private readonly Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool> m_entityModifiedDecider;
    private readonly IDictionary<Guid, TeamProjectReference> m_nameMapForWellFormedTfsEntities;
    private readonly ISet<Guid> m_nonDeletedTfsEntities;
    private readonly IDataAccessFactory m_dataAccessFactory;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private IndexMetadataStateAnalyser m_indexMetadataStateAnalyser;

    public string Name { get; } = nameof (MissedDeleteAndRenameNotificationHandlerTask);

    public MissedDeleteAndRenameNotificationHandlerTask(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      string indexingUnitType,
      IEntityType entityType,
      Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool> entityChangeDecider,
      IDictionary<Guid, TeamProjectReference> nameMapForWellFormedTfsEntities,
      ISet<Guid> nonDeletedTfsEntities,
      bool entityCrudOperationsFeatureEnabled)
    {
      this.m_dataAccessFactory = dataAccessFactory;
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_indexingUnitType = indexingUnitType;
      this.m_entityType = entityType;
      this.m_entityModifiedDecider = entityChangeDecider;
      this.m_nameMapForWellFormedTfsEntities = nameMapForWellFormedTfsEntities;
      this.m_nonDeletedTfsEntities = nonDeletedTfsEntities;
      this.m_entityCrudOperationsFeatureEnabled = entityCrudOperationsFeatureEnabled;
    }

    internal virtual IndexMetadataStateAnalyser IndexMetadataStateAnalyser
    {
      get
      {
        if (this.m_indexMetadataStateAnalyser == null)
          this.m_indexMetadataStateAnalyser = new IndexMetadataStateAnalyserFactory().GetIndexMetadataStateAnalyser(this.m_dataAccessFactory, this.m_indexingUnitChangeEventHandler, this.m_entityType);
        return this.m_indexMetadataStateAnalyser;
      }
    }

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      if (!this.m_entityCrudOperationsFeatureEnabled)
      {
        resultMessageBuilder.Append("Entity CRUD operations feature is disabled.");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(MissedDeleteAndRenameNotificationHandlerTask.s_traceMetadata, "Entity CRUD operations feature is disabled.");
      }
      else
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, this.m_indexingUnitType, this.m_entityType, -1);
        int num = 0;
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit in indexingUnits)
        {
          if (this.m_nonDeletedTfsEntities.Contains(entityIndexingUnit.TFSEntityId))
          {
            TeamProjectReference tfsProject;
            if (this.m_entityCrudOperationsFeatureEnabled && this.m_nameMapForWellFormedTfsEntities.TryGetValue(entityIndexingUnit.TFSEntityId, out tfsProject))
              num += this.IndexMetadataStateAnalyser.CreateEntityRenameOperationIfRequired((ExecutionContext) indexingExecutionContext, entityIndexingUnit, tfsProject, this.m_entityModifiedDecider);
          }
          else if (!indexingExecutionContext.RequestContext.IsProjectSoftDeleted(entityIndexingUnit.TFSEntityId.ToString()) && this.m_entityCrudOperationsFeatureEnabled)
            num += this.IndexMetadataStateAnalyser.CreateEntityDeleteOperationIfRequired((ExecutionContext) indexingExecutionContext, entityIndexingUnit);
        }
        resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("[{0}] rename/delete events were created. ", (object) num)));
      }
    }
  }
}
