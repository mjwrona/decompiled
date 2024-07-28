// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SetCsmV2ResponseHeadersAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class SetCsmV2ResponseHeadersAttribute : ActionFilterAttribute
  {
    private const string Area = "Commerce";
    private const string Layer = "SetCsmV2ResponseHeadersAttribute";
    private const int SetResponseHeadersEnter = 5106091;
    private const int SetResponseHeadersSettingDate = 5106093;
    private const int SetResponseHeadersSettingRequestId = 5106093;
    private const int SetResponseHeadersExceptionOccurred = 5106094;
    private const int SetResponseHeadersControllerNull = 5106095;
    private const int SetResponseHeadersLeave = 5106100;

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      base.OnActionExecuted(actionExecutedContext);
      TfsApiController contextController = this.GetActionContextController(actionExecutedContext);
      if (contextController == null)
      {
        TeamFoundationTracingService.TraceRaw(5106095, TraceLevel.Error, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), "SetResponseHeadersAttribute is used on a controller that does not derive from TfsApiController.");
      }
      else
      {
        contextController.TfsRequestContext.TraceEnter(5106091, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), nameof (OnActionExecuted));
        try
        {
          if (actionExecutedContext.Response != null)
          {
            if (actionExecutedContext.Response.Headers != null)
            {
              if (!actionExecutedContext.Response.Headers.Contains("Date"))
              {
                contextController.TfsRequestContext.Trace(5106093, TraceLevel.Info, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), string.Empty);
                actionExecutedContext.Response.Headers.Add("Date", DateTime.UtcNow.ToString("r"));
              }
              if (!actionExecutedContext.Response.Headers.Contains("x-ms-request-id"))
              {
                contextController.TfsRequestContext.Trace(5106093, TraceLevel.Info, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), string.Empty);
                actionExecutedContext.Response.Headers.Add("x-ms-request-id", contextController.TfsRequestContext.ActivityId.ToString("D"));
              }
            }
          }
        }
        catch (Exception ex)
        {
          contextController.TfsRequestContext.TraceException(5106094, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), ex);
        }
        contextController.TfsRequestContext.TraceLeave(5106100, "Commerce", nameof (SetCsmV2ResponseHeadersAttribute), nameof (OnActionExecuted));
      }
    }

    internal virtual TfsApiController GetActionContextController(
      HttpActionExecutedContext executedContext)
    {
      TfsApiController contextController = (TfsApiController) null;
      if (executedContext?.ActionContext?.ControllerContext != null)
        contextController = executedContext.ActionContext.ControllerContext.Controller as TfsApiController;
      return contextController;
    }
  }
}
