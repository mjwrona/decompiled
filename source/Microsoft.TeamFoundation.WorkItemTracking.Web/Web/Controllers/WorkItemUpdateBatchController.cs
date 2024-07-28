// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemUpdateBatchController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Batch;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ClientPreproduction]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "batch", ResourceVersion = 1)]
  public class WorkItemUpdateBatchController : 
    WorkItemTrackingApiController,
    IOverrideLoggingMethodNames
  {
    protected virtual bool IsPartialUpdateEnabled => false;

    public override string ActivityLogArea => "WorkItem Tracking";

    [HttpPost]
    [HttpPatch]
    [MethodInformation(EstimatedCost = EstimatedMethodCost.Moderate, TimeoutSeconds = 720)]
    public virtual Task<HttpResponseMessage> Batch(CancellationToken cancellationToken) => new WitHttpBatchHandler(WebApiConfiguration.GetHttpServer(this.TfsRequestContext), this.IsPartialUpdateEnabled).ProcessBatchAsync(this.Request, cancellationToken);

    string IOverrideLoggingMethodNames.GetLoggingMethodName(
      string methodName,
      HttpActionContext actionContext)
    {
      return methodName.Equals(actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + ".Batch", StringComparison.Ordinal) ? "Batch" : methodName;
    }
  }
}
