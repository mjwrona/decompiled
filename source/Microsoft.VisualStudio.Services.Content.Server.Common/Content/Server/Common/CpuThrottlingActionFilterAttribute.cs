// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.CpuThrottlingActionFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class CpuThrottlingActionFilterAttribute : ActionFilterAttribute
  {
    public static readonly Guid ArtifactServiceId = new Guid("00000016-0000-8888-8000-000000000000");
    public const int PotentialThrottleTracePoint = 5750100;
    public const int DefaultRetryDeltaInSeconds = 5;
    private readonly string registryRootPath;
    private readonly string controllerLabel;
    private readonly int cpuThresholdStartThrottling;
    private readonly int cpuThresholdFullyThrottling;
    private readonly TimeSpan retryDelta;
    private readonly int contentLengthThreshold;

    public CpuThrottlingActionFilterAttribute(
      string registryRootPath,
      string controllerLabel,
      int cpuThresholdFullyThrottling,
      int contentLengthThreshold = -1,
      int retryDeltaInSeconds = 5,
      int cpuThresholdStartThrottling = -1)
    {
      this.registryRootPath = registryRootPath;
      this.controllerLabel = controllerLabel;
      this.cpuThresholdFullyThrottling = cpuThresholdFullyThrottling;
      this.retryDelta = TimeSpan.FromSeconds((double) retryDeltaInSeconds);
      this.contentLengthThreshold = contentLengthThreshold;
      this.cpuThresholdStartThrottling = cpuThresholdStartThrottling;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (this.HandleAction(actionContext))
        return;
      base.OnActionExecuting(actionContext);
    }

    public override Task OnActionExecutingAsync(
      HttpActionContext actionContext,
      CancellationToken cancellationToken)
    {
      return !this.HandleAction(actionContext) ? base.OnActionExecutingAsync(actionContext, cancellationToken) : Task.CompletedTask;
    }

    private bool HandleAction(HttpActionContext actionContext)
    {
      TfsApiController controller = (TfsApiController) actionContext.ControllerContext.Controller;
      ThrottleInfo throttleInfo;
      if (!this.IsThrottled(controller, actionContext.Request, out throttleInfo))
        return false;
      actionContext.Response = actionContext.Request.CreateCpuThrottledResponseMsg(this.GetSuggestedRetry(controller.TfsRequestContext, throttleInfo));
      actionContext.Request.GetIVssRequestContext().Status = (Exception) new CpuThrottlingException(throttleInfo.Reason);
      return true;
    }

    private TimeSpan GetSuggestedRetry(IVssRequestContext context, ThrottleInfo throttleInfo) => this.retryDelta;

    private bool IsThrottled(
      TfsApiController controller,
      HttpRequestMessage requestMessage,
      out ThrottleInfo throttleInfo)
    {
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      throttleInfo = (ThrottleInfo) null;
      if (ServicePrincipals.IsServicePrincipal(tfsRequestContext, tfsRequestContext.UserContext) && tfsRequestContext.GetUserIdentity().Id != CpuThrottlingActionFilterAttribute.ArtifactServiceId || this.contentLengthThreshold >= 0 && !ContentLengthThrottleHelper.IsThrottled(tfsRequestContext, requestMessage, this.registryRootPath, this.controllerLabel, this.contentLengthThreshold))
        return false;
      ThrottleInfo primary = CpuThrottleHelper.Instance.IsThrottledWithInfo(tfsRequestContext, this.registryRootPath, this.controllerLabel, this.cpuThresholdFullyThrottling, this.cpuThresholdStartThrottling);
      return this.ThrottleBasedOnProvider(tfsRequestContext, primary, "DoubleExponentialCpuSmoothingProvider", controller.ActivityLogArea, out throttleInfo);
    }

    private bool ThrottleBasedOnProvider(
      IVssRequestContext requestContext,
      ThrottleInfo primary,
      string primaryName,
      string area,
      out ThrottleInfo throttleInfo)
    {
      if (primary.IsThrottled)
      {
        throttleInfo = new ThrottleInfo(true, string.Format("Primary Provider {0}: {1}.  Retry = {2}.", (object) primaryName, (object) primary.Reason, (object) primary.SuggestedRetry), primary.SuggestedRetry);
        return true;
      }
      throttleInfo = (ThrottleInfo) null;
      return false;
    }
  }
}
