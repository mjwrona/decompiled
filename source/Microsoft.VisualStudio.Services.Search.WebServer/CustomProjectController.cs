// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomProjectController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Index;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customProject")]
  public class CustomProjectController : SearchApiController
  {
    private ICustomProjectForwarder m_customProjectForwarder;

    public CustomProjectController()
      : this((ICustomProjectForwarder) new CustomProjectForwarder())
    {
    }

    public CustomProjectController(ICustomProjectForwarder customProjectForwarder) => this.m_customProjectForwarder = customProjectForwarder;

    [HttpGet]
    [ClientLocationId("335FEA08-C0AD-4D59-90F5-067D716DD93E")]
    public IEnumerable<string> GetProjects()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080030, "REST-API", "REST-API", nameof (GetProjects));
      try
      {
        return this.HandleGetProjects(this.TfsRequestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080031, "REST-API", "REST-API", nameof (GetProjects));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected IEnumerable<string> HandleGetProjects(IVssRequestContext requestContext)
    {
      IEnumerable<string> projectsRequest;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        projectsRequest = this.m_customProjectForwarder.ForwardGetProjectsRequest(requestContext);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomProjectsTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080032, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080032, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return projectsRequest;
    }
  }
}
