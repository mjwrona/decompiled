// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomTenantController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Index;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customTenant")]
  public class CustomTenantController : SearchApiController
  {
    private ICustomTenantForwarder m_customTenantForwarder;

    public CustomTenantController()
    {
    }

    public CustomTenantController(ICustomTenantForwarder customTenantForwarder) => this.m_customTenantForwarder = customTenantForwarder;

    [HttpPost]
    [ClientLocationId("DD2A401F-0348-4D07-97C4-1071310AEE9A")]
    public bool RegisterTenant(CustomTenant tenant)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080060, "REST-API", "REST-API", nameof (RegisterTenant));
      try
      {
        return this.HandleRegisterTenant(this.TfsRequestContext, tenant);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080061, "REST-API", "REST-API", nameof (RegisterTenant));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientLocationId("F04D1DDA-59AA-4648-BB0B-EDA3E6F2EFE9")]
    public IEnumerable<string> GetTenantCollectionNames(string tenantName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080060, "REST-API", "REST-API", nameof (GetTenantCollectionNames));
      try
      {
        return this.HandleGetTenantCollectionNames(this.TfsRequestContext, tenantName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080061, "REST-API", "REST-API", nameof (GetTenantCollectionNames));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected bool HandleRegisterTenant(IVssRequestContext requestContext, CustomTenant tenant)
    {
      bool flag = false;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableAdminOperations", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Setting '{0}' is not enabled.", (object) "/Service/ALMSearch/Settings/EnableAdminOperations")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        this.PublishRequest(tenant);
        flag = this.m_customTenantForwarder.ForwardRegisterTenantRequest(requestContext, tenant);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("RegisterCustomTenantTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080062, "REST-API", "REST-API", ex);
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            ExceptionDispatchInfo.Capture(ex).Throw();
            break;
          default:
            throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
        }
      }
      return flag;
    }

    protected IEnumerable<string> HandleGetTenantCollectionNames(
      IVssRequestContext requestContext,
      string tenantName)
    {
      IEnumerable<string> tenantCollectionNames = (IEnumerable<string>) null;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableAdminOperations", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Setting '{0}' is not enabled.", (object) "/Service/ALMSearch/Settings/EnableAdminOperations")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        tenantCollectionNames = this.m_customTenantForwarder.ForwardGetTenantCollectionNamesRequest(requestContext, tenantName);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomTenantCollectionNamesTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080062, "REST-API", "REST-API", ex.Message);
            ExceptionDispatchInfo.Capture(ex).Throw();
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080062, "REST-API", "REST-API", ex);
            throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
        }
      }
      return tenantCollectionNames;
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      if (this.m_customTenantForwarder != null)
        return;
      this.m_customTenantForwarder = (ICustomTenantForwarder) new CustomTenantForwarder(this.TfsRequestContext.GetConfigValue("/Service/ALMSearch/Settings/TenantSearchPlatformConnectionString"), this.TfsRequestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/CustomSearchPlatformSettings", "ConnectionTimeout=180"), this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    private void PublishRequest(CustomTenant tenant) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
    {
      ["CTName"] = (object) tenant.Name,
      ["CT Indices"] = (object) string.Join<IndexDetail>(" | ", tenant.Indices)
    });
  }
}
