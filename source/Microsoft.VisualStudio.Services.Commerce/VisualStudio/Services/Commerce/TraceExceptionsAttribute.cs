// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.TraceExceptionsAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class TraceExceptionsAttribute : ExceptionFilterAttribute
  {
    private FilterAttributeHelper filterHelper;

    internal int TracePoint { get; private set; }

    public TraceExceptionsAttribute(int tracePoint)
      : this(tracePoint, new FilterAttributeHelper())
    {
    }

    internal TraceExceptionsAttribute(int tracePoint, FilterAttributeHelper filterHelper)
    {
      this.TracePoint = tracePoint;
      this.filterHelper = filterHelper;
    }

    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
      try
      {
        CommerceControllerBase contextController = this.filterHelper.GetActionContextController(actionExecutedContext);
        if (contextController?.TfsRequestContext == null)
          return;
        contextController.TfsRequestContext.TraceException(this.TracePoint, contextController.Area, contextController.Layer, actionExecutedContext.Exception);
      }
      finally
      {
        base.OnException(actionExecutedContext);
      }
    }
  }
}
