// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.CollectionExtensionFinalizeUninstallOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Extension;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal class CollectionExtensionFinalizeUninstallOperation : IndexingUnitDeleteOperation
  {
    public CollectionExtensionFinalizeUninstallOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080697, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
        this.DeleteChangeEvents(executionContext);
        this.DeleteIndexingUnit(executionContext);
        this.MarkExtensionUninstallComplete(coreIndexingExecutionContext.RequestContext);
        operationResult.Message = FormattableString.Invariant(FormattableStringFactory.Create("CollectionExtensionFinalizeUninstallOperation successful for {0}", (object) this.IndexingUnit));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080697, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    private void MarkExtensionUninstallComplete(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string path = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}/{2}", (object) "/Service/ALMSearch/Settings/IsExtensionOperationInProgress", (object) this.IndexingUnit.EntityType.Name, (object) InstalledExtensionMessageChangeType.Uninstalled));
      service.SetValue<bool>(requestContext, path, false);
    }
  }
}
