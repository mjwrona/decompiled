// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomRepositoryBranchStatusController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customRepositoryBranch")]
  public class CustomRepositoryBranchStatusController : SearchControllerBase
  {
    [HttpGet]
    [ClientInternalUseOnly(true)]
    [ClientLocationId("5D9391B9-E2C3-4B87-9033-83C3AED4069E")]
    [PublicProjectRequestRestrictions]
    public CustomRepositoryBranchStatusResponse GetCustomRepositoryBranchStatus(
      string repository,
      string branch)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083161, "REST-API", "REST-API", nameof (GetCustomRepositoryBranchStatus));
      try
      {
        if (repository == null || branch == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
        ICodeSecurityChecksService service = this.TfsRequestContext.GetService<ICodeSecurityChecksService>();
        service.ValidateAndSetUserPermissionsForSearchService(this.TfsRequestContext);
        service.PopulateUserSecurityChecksDataInRequestContext(this.TfsRequestContext);
        return CustomRepositoryBranchStalenessProvider.GetBranchStalenessData(this.TfsRequestContext, this.ProjectInfo.Name, branch);
      }
      catch (AggregateException ex) when (ex.InnerException.Message.Contains("TF401027"))
      {
        throw new UnauthorizedAccessException(SearchWebApiResources.UnauthorizedAccessException);
      }
      catch (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException ex)
      {
        throw ex.ConvertLegacyExceptionToCorrectException();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083161, "REST-API", "REST-API", nameof (GetCustomRepositoryBranchStatus));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
