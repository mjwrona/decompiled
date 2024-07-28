// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TraceFiltersController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [VersionedApiControllerCustomName("Tracing", "Filters", 3)]
  public class TraceFiltersController : TfsApiController
  {
    public const string RouteName = "TraceFilters";

    public override string TraceArea => "TraceFilters";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidTraceParametersException>(HttpStatusCode.BadRequest);
    }

    public override string ActivityLogArea => "Framework";

    [TraceFilter(1095030, 1095039)]
    [HttpGet]
    public IEnumerable<TraceFilter> GetTraceFilters(Guid? ownerId = null)
    {
      this.CheckPermission(TraceFiltersController.TracingPermissions.TracingRead);
      this.AllowErrorDetails();
      return this.TfsRequestContext.GetService<TeamFoundationTracingService>().QueryTraces(this.TfsRequestContext, ownerId);
    }

    [TraceFilter(1095020, 1095029)]
    [HttpGet]
    public TraceFilter GetTraceFilter(Guid traceId)
    {
      this.CheckPermission(TraceFiltersController.TracingPermissions.TracingRead);
      this.AllowErrorDetails();
      return (!(traceId == new Guid()) ? this.TfsRequestContext.GetService<TeamFoundationTracingService>().QueryTrace(this.TfsRequestContext, traceId) : throw new InvalidTraceParametersException("Parameter 'traceId' cannot be an empty GUID")) ?? throw new HttpResponseException(HttpStatusCode.NotFound);
    }

    [TraceFilter(1095000, 1095009)]
    [HttpPost]
    public TraceFilter AddTraceFilter(TraceFilter traceFilter)
    {
      this.CheckPermission(TraceFiltersController.TracingPermissions.TracingWrite);
      this.AllowErrorDetails();
      if (traceFilter == null)
        throw new InvalidTraceParametersException("Parameter 'traceFilter' cannot be null");
      TeamFoundationTracingService service = this.TfsRequestContext.GetService<TeamFoundationTracingService>();
      traceFilter = service.QueryTrace(this.TfsRequestContext, service.StartTrace(this.TfsRequestContext, traceFilter));
      return traceFilter;
    }

    [TraceFilter(1095010, 1095019)]
    [HttpDelete]
    public void RemoveTraceFilter(Guid traceId)
    {
      this.CheckPermission(TraceFiltersController.TracingPermissions.TracingWrite);
      this.AllowErrorDetails();
      if (traceId == new Guid())
        throw new InvalidTraceParametersException("Parameter 'traceId' cannot be an empty GUID");
      this.TfsRequestContext.GetService<TeamFoundationTracingService>().StopTrace(this.TfsRequestContext, traceId);
    }

    private void CheckPermission(
      TraceFiltersController.TracingPermissions permission)
    {
      this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.TracingNamespaceId).CheckPermission(this.TfsRequestContext, FrameworkSecurity.TracingNamespaceToken, (int) permission);
    }

    private void AllowErrorDetails() => this.Request.Properties[HttpPropertyKeys.IncludeErrorDetailKey] = (object) new Lazy<bool>((Func<bool>) (() => true));

    [Flags]
    internal enum TracingPermissions
    {
      TracingRead = 1,
      TracingWrite = 2,
    }
  }
}
