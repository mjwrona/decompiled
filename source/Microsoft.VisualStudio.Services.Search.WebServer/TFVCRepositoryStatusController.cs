// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.TFVCRepositoryStatusController
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
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.TfvcRepositoryStatus;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "tfvc")]
  public class TFVCRepositoryStatusController : SearchControllerBase
  {
    private BranchService m_branchService;

    [HttpGet]
    [ClientLocationId("D5BF4E52-E0AF-4626-8C50-8A80B18FA69F")]
    [PublicProjectRequestRestrictions]
    public TfvcRepositoryStatusResponse GetTfvcRepositoryStatus()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083151, "REST-API", "REST-API", nameof (GetTfvcRepositoryStatus));
      try
      {
        ICodeSecurityChecksService service = this.TfsRequestContext.GetService<ICodeSecurityChecksService>();
        this.m_branchService = this.TfsRequestContext.GetService<BranchService>();
        List<BranchInfo> repositoryIndexInfo = this.m_branchService.GetTfvcRepositoryIndexInfo(this.ProjectInfo.Id, this.ProjectInfo.Name);
        TfvcRepositoryStatusResponse response = new TfvcRepositoryStatusResponse()
        {
          Id = this.ProjectInfo.Id,
          Name = this.ProjectInfo.Name,
          IndexingInformation = (IEnumerable<BranchInfo>) repositoryIndexInfo
        };
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        service.PopulateUserSecurityChecksDataInRequestContext(tfsRequestContext);
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) response);
        return response;
      }
      catch (AggregateException ex) when (ex.InnerException.Message.Contains("TF401027"))
      {
        throw new UnauthorizedAccessException(SearchWebApiResources.UnauthorizedAccessException);
      }
      catch (SearchException ex)
      {
        throw ex.ConvertLegacyExceptionToCorrectException();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083151, "REST-API", "REST-API", nameof (GetTfvcRepositoryStatus));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
