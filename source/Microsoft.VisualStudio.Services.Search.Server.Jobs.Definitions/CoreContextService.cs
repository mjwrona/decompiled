// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CoreContextService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class CoreContextService : ICoreContextService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<ICoreIndexingExecutionContext> m_indexingExecutionContexts;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_indexingExecutionContexts = SearchPlatformHelper.GetExtensions<ICoreIndexingExecutionContext>(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_indexingExecutionContexts == null)
        return;
      this.m_indexingExecutionContexts.Dispose();
      this.m_indexingExecutionContexts = (IDisposableReadOnlyList<ICoreIndexingExecutionContext>) null;
    }

    public CoreIndexingExecutionContext CreateIndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      CoreIndexingExecutionContext executionContext1 = (CoreIndexingExecutionContext) null;
      foreach (ICoreIndexingExecutionContext executionContext2 in (IEnumerable<ICoreIndexingExecutionContext>) this.m_indexingExecutionContexts)
      {
        if (((IEnumerable<IEntityType>) executionContext2.SupportedEntityTypes).Contains<IEntityType>(indexingUnit.EntityType, (IEqualityComparer<IEntityType>) new EntityTypeComparer()))
        {
          executionContext1 = (CoreIndexingExecutionContext) Activator.CreateInstance(executionContext2.GetType(), (object) requestContext, (object) indexingUnit, (object) tracerCICorrelationDetails, (object) indexingUnitChangeEventHandler);
          break;
        }
      }
      if (executionContext1 == null)
        executionContext1 = new CoreIndexingExecutionContext(requestContext, indexingUnit, tracerCICorrelationDetails, indexingUnitChangeEventHandler);
      return executionContext1;
    }
  }
}
