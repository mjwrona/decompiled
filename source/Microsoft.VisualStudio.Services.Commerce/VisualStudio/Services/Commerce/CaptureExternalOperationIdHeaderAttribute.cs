// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CaptureExternalOperationIdHeaderAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CaptureExternalOperationIdHeaderAttribute : ActionFilterAttribute
  {
    private const string Area = "Commerce";
    private const string Layer = "CaptureExternalOperationIdHeaderAttribute";

    public string HeaderName { get; set; }

    public CaptureExternalOperationIdHeaderAttribute(string headerName) => this.HeaderName = headerName;

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (actionContext == null || actionContext.ControllerContext == null)
        return;
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      try
      {
        string uniqueIdentifier = (string) null;
        IEnumerable<string> values;
        if (actionContext.ControllerContext.Request.Headers.TryGetValues(this.HeaderName, out values))
          uniqueIdentifier = values.FirstOrDefault<string>();
        if (!string.IsNullOrWhiteSpace(uniqueIdentifier))
        {
          controller.TfsRequestContext.Trace(5108740, TraceLevel.Info, "Commerce", nameof (CaptureExternalOperationIdHeaderAttribute), "Incoming external request id captured from \"" + this.HeaderName + "\" has value \"" + uniqueIdentifier + "\"");
          CommerceCommons.SetRequestContextUniqueIdentifier(controller.TfsRequestContext, uniqueIdentifier);
        }
        else
          controller.TfsRequestContext.Trace(5108741, TraceLevel.Warning, "Commerce", nameof (CaptureExternalOperationIdHeaderAttribute), "Incoming external request id was not captured, expected header \"" + this.HeaderName + "\" was not present.");
      }
      catch (Exception ex)
      {
        controller.TfsRequestContext.TraceException(5108742, "Commerce", nameof (CaptureExternalOperationIdHeaderAttribute), ex);
      }
    }
  }
}
