// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.RepositoryStatusController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "repositories")]
  public class RepositoryStatusController : SearchControllerBase
  {
    private BranchService m_branchService;

    [HttpGet]
    [ClientLocationId("1F60303C-7261-4387-80F1-742A2ECF2964")]
    [PublicProjectRequestRestrictions]
    public RepositoryStatusResponse GetRepositoryStatus(string repository)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083128, "REST-API", "REST-API", nameof (GetRepositoryStatus));
      try
      {
        if (repository == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
        GitRepository result = this.TfsRequestContext.GetService<ICollectionRedirectionService>().GetClient<GitHttpClient>(this.TfsRequestContext).GetRepositoryAsync(this.ProjectInfo.Name, repository, (object) null, new CancellationToken()).Result;
        this.m_branchService = this.TfsRequestContext.GetService<BranchService>();
        if (!this.TfsRequestContext.GetService<ICodeSecurityChecksService>().GitRepoHasReadPermission(this.TfsRequestContext, new GitRepositoryData()
        {
          Name = result.Name,
          Id = result.Id,
          ProjectName = this.ProjectInfo.Name,
          ProjectId = this.ProjectInfo.Id
        }))
          throw new UnauthorizedAccessException(SearchWebApiResources.UnauthorizedAccessException);
        List<string> list = this.m_branchService.GetBranches(this.TfsRequestContext, this.ProjectInfo.Name, result.Name).ToList<string>();
        List<BranchInfo> repositoryIndexInfo = this.m_branchService.GetRepositoryIndexInfo(this.TfsRequestContext, result.Id, list);
        RepositoryStatusResponse response = new RepositoryStatusResponse()
        {
          Id = result.Id,
          Name = result.Name,
          IndexedBranches = (IEnumerable<BranchInfo>) repositoryIndexInfo
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083128, "REST-API", "REST-API", nameof (GetRepositoryStatus));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
