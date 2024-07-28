// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.UpdateSearchUrlInIndexingPropertiesOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class UpdateSearchUrlInIndexingPropertiesOperation : AbstractIndexingOperation
  {
    public UpdateSearchUrlInIndexingPropertiesOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080644, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      try
      {
        if (coreIndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          this.UpdateSearchUrl(coreIndexingExecutionContext.RequestContext, resultMessage);
          operationResult.Status = OperationStatus.Succeeded;
        }
        else
        {
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Operation supported only in On-Premises environment.")));
          operationResult.Status = OperationStatus.Failed;
        }
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080644, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    private void UpdateSearchUrl(IVssRequestContext requestContext, StringBuilder resultMessage)
    {
      string b = requestContext.To(TeamFoundationHostType.Deployment).GetService<CachedRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/ALMSearch/Settings/ATSearchPlatformConnectionString", false, (string) null);
      CollectionIndexingProperties properties = this.IndexingUnit.Properties as CollectionIndexingProperties;
      string connectionString = properties.IndexESConnectionString;
      if (!string.Equals(connectionString, b, StringComparison.OrdinalIgnoreCase))
      {
        properties.IndexESConnectionString = properties.QueryESConnectionString = b;
        this.IndexingUnit = this.IndexingUnitDataAccess.UpdateIndexingUnit(requestContext, this.IndexingUnit);
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated SearchUrl from {0} to {1} in {2}", (object) connectionString, (object) b, (object) this.IndexingUnit.ToString())));
      }
      else
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("SearchUrl in {0} already set to {1}. No updates done.", (object) this.IndexingUnit.ToString(), (object) b)));
    }
  }
}
