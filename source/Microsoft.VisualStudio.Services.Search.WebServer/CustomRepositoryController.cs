// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomRepositoryController
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
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customRepository")]
  public class CustomRepositoryController : SearchApiController
  {
    private ICustomRepositoryForwarder m_customRepositoryForwarder;

    public CustomRepositoryController()
      : this((ICustomRepositoryForwarder) new CustomRepositoryForwarder())
    {
    }

    public CustomRepositoryController(
      ICustomRepositoryForwarder customRepositoryForwarder)
    {
      this.m_customRepositoryForwarder = customRepositoryForwarder;
    }

    [HttpPost]
    [ClientLocationId("5A15DEB0-9F03-4B84-A004-2F1C6717D8B4")]
    public CustomRepository RegisterCustomRepository(CustomRepository repository)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080040, "REST-API", "REST-API", nameof (RegisterCustomRepository));
      try
      {
        return this.HandleRegisterRepository(this.TfsRequestContext, repository);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080041, "REST-API", "REST-API", nameof (RegisterCustomRepository));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientLocationId("B2131804-02B0-44E7-8ACE-3055AD379D23")]
    public IEnumerable<string> GetRepositories(string projectName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080040, "REST-API", "REST-API", nameof (GetRepositories));
      try
      {
        return this.HandleGetRepositories(this.TfsRequestContext, projectName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080041, "REST-API", "REST-API", nameof (GetRepositories));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientLocationId("EABF0BE0-9DF6-4E81-9A44-F18BE5B5A342")]
    public CustomRepository GetRepository(string projectName, string repositoryName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080040, "REST-API", "REST-API", nameof (GetRepository));
      try
      {
        return this.HandleGetRepository(this.TfsRequestContext, projectName, repositoryName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080041, "REST-API", "REST-API", nameof (GetRepository));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    [HttpGet]
    [ClientIgnore]
    [ClientLocationId("737BE0FB-3664-4703-869C-1D8491B04E3D")]
    public CustomRepositoryHealthResponse GetRepositoryHealth(
      string projectName,
      string repositoryName,
      string branchName,
      int numberOfResults,
      long continuationToken = 0)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080040, "REST-API", "REST-API", nameof (GetRepositoryHealth));
      try
      {
        return this.HandleGetRepositoryHealth(this.TfsRequestContext, projectName, repositoryName, branchName, numberOfResults, continuationToken);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080041, "REST-API", "REST-API", nameof (GetRepositoryHealth));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected CustomRepository HandleRegisterRepository(
      IVssRequestContext requestContext,
      CustomRepository repository)
    {
      CustomRepository customRepository;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        customRepository = this.m_customRepositoryForwarder.ForwardRegisterRepositoryRequest(requestContext, repository);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("RegisterCustomRepositoryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case UnsupportedHostTypeException _:
          case ArgumentNullException _:
          case NotSupportedException _:
          case InvalidCustomRequestException _:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080042, "REST-API", "REST-API", ex.Message);
            break;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080042, "REST-API", "REST-API", ex);
            break;
        }
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return customRepository;
    }

    protected IEnumerable<string> HandleGetRepositories(
      IVssRequestContext requestContext,
      string projectName)
    {
      IEnumerable<string> repositoriesRequest;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        repositoriesRequest = this.m_customRepositoryForwarder.ForwardGetRepositoriesRequest(requestContext, projectName);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomRepositoriesTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080042, "REST-API", "REST-API", ex);
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return repositoriesRequest;
    }

    protected CustomRepository HandleGetRepository(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName)
    {
      CustomRepository repositoryRequest;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        repositoryRequest = this.m_customRepositoryForwarder.ForwardGetRepositoryRequest(requestContext, projectName, repositoryName);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomRepositoryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080042, "REST-API", "REST-API", ex);
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return repositoryRequest;
    }

    protected CustomRepositoryHealthResponse HandleGetRepositoryHealth(
      IVssRequestContext tfsRequestContext,
      string projectName,
      string repositoryName,
      string branchName,
      int numberOfResults,
      long continuationToken)
    {
      CustomRepositoryHealthResponse repositoryHealthRequest;
      try
      {
        if (!tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(tfsRequestContext.ServiceHost.HostType);
        if (!tfsRequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        Stopwatch stopwatch = Stopwatch.StartNew();
        repositoryHealthRequest = this.m_customRepositoryForwarder.ForwardGetRepositoryHealthRequest(tfsRequestContext, projectName, repositoryName, branchName, numberOfResults, continuationToken);
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetCustomRepositoryHealth", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080042, "REST-API", "REST-API", ex);
        throw new InvalidCustomRequestException(ex.Message, ex);
      }
      return repositoryHealthRequest;
    }

    [HttpGet]
    [ClientLocationId("EA0B9B23-A6C4-48E3-BDFC-46EBD15C4D7E")]
    public RepositoryIndexingProperties GetLastIndexedChangeId(
      string projectName,
      string repositoryName,
      string branchName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080050, "REST-API", "REST-API", nameof (GetLastIndexedChangeId));
      try
      {
        return this.HandleGetLastIndexedChangeId(this.TfsRequestContext, projectName, repositoryName, branchName);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080051, "REST-API", "REST-API", nameof (GetLastIndexedChangeId));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected RepositoryIndexingProperties HandleGetLastIndexedChangeId(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName)
    {
      if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
      RepositoryIndexingProperties lastIndexedChangeId = this.m_customRepositoryForwarder.GetLastIndexedChangeId(requestContext, projectName, repositoryName, branchName);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080043, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("ProjectName {0}, RepoName: {1}, BranchName: {2}, RepositoryIndexingProperties: {3}", (object) projectName, (object) repositoryName, (object) branchName, (object) lastIndexedChangeId)));
      return lastIndexedChangeId;
    }
  }
}
