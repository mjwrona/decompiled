// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SecurityChecksController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "userAccessibleRepositories")]
  public class SecurityChecksController : SearchApiController
  {
    protected bool EnableSecurityChecksInQueryPipeline { get; set; }

    [HttpGet]
    [ClientLocationId("c4c5c9eb-6028-454a-8f46-d92a996da947")]
    public IEnumerable<GitRepositoryData> GetUserAccessibleRepositories([FromUri] string projectIdentifier = null)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080020, "REST-API", "REST-API", nameof (GetUserAccessibleRepositories));
      try
      {
        return this.HandleGetUserAccessibleRepositoriesRequest(this.TfsRequestContext, projectIdentifier, new SecurityChecksController.Func<IVssRequestContext, string, bool>(this.IsSecurityChecksEnabled));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080022, "REST-API", "REST-API", ex);
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case NotImplementedException _:
            ExceptionDispatchInfo.Capture(ex).Throw();
            return (IEnumerable<GitRepositoryData>) null;
          default:
            throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080021, "REST-API", "REST-API", nameof (GetUserAccessibleRepositories));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected IEnumerable<GitRepositoryData> HandleGetUserAccessibleRepositoriesRequest(
      IVssRequestContext requestContext,
      string projectIdentifier,
      SecurityChecksController.Func<IVssRequestContext, string, bool> featureFlagDelegate)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      string output;
      if (!featureFlagDelegate(requestContext, out output))
        throw new NotImplementedException(output);
      return requestContext.GetService<ICodeSecurityChecksService>().GetUserAccessibleRepositories(requestContext, projectIdentifier);
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.EnableSecurityChecksInQueryPipeline = this.TfsRequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableSecurityChecksInQueryPipeline");
    }

    private bool IsSecurityChecksEnabled(IVssRequestContext requestContext, out string reason)
    {
      if (!this.EnableSecurityChecksInQueryPipeline)
      {
        reason = "Security checks feature is disabled in the query pipeline.";
        return false;
      }
      reason = string.Empty;
      return true;
    }

    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public delegate TItem3 Func<in TItem1, TItem2, out TItem3>(TItem1 input, out TItem2 output);
  }
}
