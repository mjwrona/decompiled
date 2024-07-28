// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CustomRepositoryStatusController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "customRepositories")]
  public class CustomRepositoryStatusController : SearchControllerBase
  {
    private BranchService m_branchService;

    [HttpGet]
    [ClientInternalUseOnly(true)]
    [ClientLocationId("DE4C37B8-F55F-4C38-8F3F-A296CFB63121")]
    [PublicProjectRequestRestrictions]
    public CustomRepositoryStatusResponse GetCustomRepositoryStatus(string repository)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083150, "REST-API", "REST-API", nameof (GetCustomRepositoryStatus));
      try
      {
        if (repository == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
        ICodeSecurityChecksService service = this.TfsRequestContext.GetService<ICodeSecurityChecksService>();
        service.ValidateAndSetUserPermissionsForSearchService(this.TfsRequestContext);
        service.PopulateUserSecurityChecksDataInRequestContext(this.TfsRequestContext);
        this.m_branchService = this.TfsRequestContext.GetService<BranchService>();
        List<string> list = this.m_branchService.GetBranches(this.TfsRequestContext, this.ProjectInfo.Name, repository).ToList<string>();
        List<DepotInfo> repositoryIndexInfo = this.m_branchService.GetCustomRepositoryIndexInfo(this.ProjectInfo.Name, repository, list);
        CustomRepositoryStatusResponse response = new CustomRepositoryStatusResponse()
        {
          Id = Guid.Empty,
          Name = repository,
          IndexedTopLevelFolders = (IEnumerable<DepotInfo>) repositoryIndexInfo
        };
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) response);
        return response;
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083150, "REST-API", "REST-API", nameof (GetCustomRepositoryStatus));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
