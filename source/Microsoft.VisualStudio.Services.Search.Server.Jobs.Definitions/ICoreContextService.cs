// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.ICoreContextService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  [DefaultServiceImplementation(typeof (CoreContextService))]
  public interface ICoreContextService : IVssFrameworkService
  {
    CoreIndexingExecutionContext CreateIndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler);
  }
}
