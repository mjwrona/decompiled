// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitImportRequestsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitImportRequestsController : GitApiController
  {
    protected const string TraceLayer = "GitImportRequestsController";

    [HttpPost]
    [ClientLocationId("01828DDC-3600-4A41-8633-99B3A73A0EB3")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__importRequests.json", null, null, null)]
    public HttpResponseMessage CreateImportRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest importRequest,
      [ClientIgnore] string project = null)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>(importRequest, nameof (importRequest), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<GitImportRequestParameters>(importRequest.Parameters, "Parameters", this.TfsRequestContext.ServiceName);
      GitImportGitSource gitSource = importRequest.Parameters.GitSource;
      GitImportTfvcSource tfvcSource = importRequest.Parameters.TfvcSource;
      if (gitSource != null && tfvcSource != null || gitSource == null && tfvcSource == null)
        throw new InvalidArgumentValueException(nameof (importRequest), Resources.Get("InvalidParameters"));
      if (gitSource != null)
      {
        gitSource.Url = gitSource.Url?.Trim() ?? string.Empty;
        if (!gitSource.Overwrite && string.IsNullOrWhiteSpace(gitSource.Url) || gitSource.Overwrite && !string.IsNullOrWhiteSpace(gitSource.Url))
          throw new InvalidArgumentValueException(nameof (importRequest), Resources.Get("InvalidParameters"));
      }
      if (tfvcSource != null)
      {
        if (string.IsNullOrWhiteSpace(tfvcSource.Path) || tfvcSource.ImportHistoryDurationInDays > 180 || tfvcSource.ImportHistoryDurationInDays < 0)
          throw new InvalidArgumentValueException(nameof (importRequest), Resources.Get("InvalidParameters"));
        tfvcSource.Path = tfvcSource.Path.Trim();
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, project))
      {
        ITeamFoundationGitImportService service = this.TfsRequestContext.GetService<ITeamFoundationGitImportService>();
        if (gitSource != null && gitSource.Overwrite)
          importRequest.Parameters.GitSource.Url = (service.QueryImportRequests(this.TfsRequestContext, tfsGitRepository, false).FirstOrDefault<Microsoft.TeamFoundation.Git.Server.GitImportRequest>((Func<Microsoft.TeamFoundation.Git.Server.GitImportRequest, bool>) (x => x.Status == GitAsyncOperationStatus.Completed)) ?? throw new InvalidArgumentValueException("gitSource")).Parameters.GitSource.Url;
        if (!GitImportValidationUtility.ShouldSkipValidation(this.TfsRequestContext, this.Request))
        {
          ClientTraceData clientTraceData = new ClientTraceData();
          ImportRepositoryValidation repositoryValidation = this.GetImportRepositoryValidation(importRequest, this.ProjectInfo.Id);
          string errorMessage = (string) null;
          if ((repositoryValidation == null ? 0 : (GitImportValidationUtility.ValidateImportParams(this.TfsRequestContext, repositoryValidation, this.TraceArea, clientTraceData, out errorMessage) ? 1 : 0)) == 0)
          {
            this.TfsRequestContext.GetService<ClientTraceService>().Publish(this.TfsRequestContext, "Microsoft.TeamFoundation.Sourcecontrol.WebServer", "RemoteRepositoryValidation", clientTraceData);
            if (!string.IsNullOrWhiteSpace(errorMessage))
              throw new RemoteRepositoryValidationFailed(errorMessage);
            return this.Request.CreateResponse(HttpStatusCode.BadRequest);
          }
        }
        if (importRequest.Repository != null && importRequest.Repository.Id != Guid.Empty && importRequest.Repository.Id != tfsGitRepository.Key.RepoId)
          throw new InvalidArgumentValueException(Resources.Get("MismatchRepositoryId"));
        Guid result;
        return this.Request.CreateResponse<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>(HttpStatusCode.Created, service.CreateImportRequest(this.TfsRequestContext, tfsGitRepository, new InternalGitImportRequestParameters(importRequest.Parameters, this.Request.Headers.UserAgent?.ToString()), Guid.TryParse(project, out result) ? new Guid?(result) : new Guid?()).ToWebApiImportItem(tfsGitRepository, this.TfsRequestContext, this.Url, true));
      }
    }

    [HttpGet]
    [ClientLocationId("01828DDC-3600-4A41-8633-99B3A73A0EB3")]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>), null, null)]
    [ClientExample("GET__git_repositories__repositoryId__importRequests.json", "By repository ID", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__importRequests_includeAbandoned-True.json", "By repository ID with abandoned requests", null, null)]
    public HttpResponseMessage QueryImportRequests(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      bool includeAbandoned = false)
    {
      using (ITfsGitRepository repository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>>(HttpStatusCode.OK, (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>) this.TfsRequestContext.GetService<ITeamFoundationGitImportService>().QueryImportRequests(this.TfsRequestContext, repository, includeAbandoned).Select<Microsoft.TeamFoundation.Git.Server.GitImportRequest, Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>((Func<Microsoft.TeamFoundation.Git.Server.GitImportRequest, Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>) (x => x.ToWebApiImportItem(repository, this.TfsRequestContext, this.Url, true))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>());
    }

    [HttpGet]
    [ClientLocationId("01828DDC-3600-4A41-8633-99B3A73A0EB3")]
    [ClientExample("GET__git_repositories__repositoryId__importRequests__importRequestId_.json", null, null, null)]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest GetImportRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int importRequestId,
      [ClientIgnore] string projectId = null)
    {
      ArgumentUtility.CheckForOutOfRange(importRequestId, nameof (importRequestId), 0);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        return this.TfsRequestContext.GetService<ITeamFoundationGitImportService>().GetImportRequestById(this.TfsRequestContext, tfsGitRepository, importRequestId).ToWebApiImportItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
    }

    [HttpPatch]
    [ClientLocationId("01828DDC-3600-4A41-8633-99B3A73A0EB3")]
    [ClientExample("PATCH__git_repositories__repositoryId__importRequests__importRequestId_.json", "Retry or abandon a failed import request", null, null)]
    public Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest UpdateImportRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int importRequestId,
      [FromBody] Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest importRequestToUpdate,
      [ClientIgnore] string projectId = null)
    {
      ArgumentUtility.CheckForOutOfRange(importRequestId, nameof (importRequestId), 0);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>(importRequestToUpdate, nameof (importRequestToUpdate));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        if (importRequestToUpdate.Parameters != null || importRequestToUpdate.Repository != null || importRequestToUpdate.DetailedStatus != null || importRequestToUpdate.Status != GitAsyncOperationStatus.Queued && importRequestToUpdate.Status != GitAsyncOperationStatus.Abandoned)
          throw new InvalidArgumentValueException(nameof (importRequestToUpdate), Resources.Get("InvalidImportRequestUpdate"));
        return this.TfsRequestContext.GetService<ITeamFoundationGitImportService>().UpdateImportStatus(this.TfsRequestContext, tfsGitRepository, importRequestId, importRequestToUpdate.Status, (GitImportStatusDetail) null).ToWebApiImportItem(tfsGitRepository, this.TfsRequestContext, this.Url, true);
      }
    }

    private ImportRepositoryValidation GetImportRepositoryValidation(
      Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest importRequest,
      Guid projectId)
    {
      ImportRepositoryValidation repositoryValidation = new ImportRepositoryValidation()
      {
        GitSource = importRequest.Parameters.GitSource,
        TfvcSource = importRequest.Parameters.TfvcSource,
        Username = (string) null,
        Password = (string) null
      };
      if (importRequest.Parameters.ServiceEndpointId != Guid.Empty)
      {
        int num = 0;
        do
        {
          List<ServiceEndpoint> source = this.TfsRequestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(this.TfsRequestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) null, (string) null, false);
          if (source == null || !source.Any<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => x.Id == importRequest.Parameters.ServiceEndpointId)))
          {
            this.TfsRequestContext.Trace(1013665, TraceLevel.Warning, this.TraceArea, nameof (GitImportRequestsController), "Service Endpoint not acceessible to user after creating import request");
            Thread.Sleep(TimeSpan.FromSeconds(3.0));
            ++num;
          }
          else
            goto label_5;
        }
        while (num < 3);
        return (ImportRepositoryValidation) null;
label_5:
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        ServiceEndpoint serviceEndpoint = vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(vssRequestContext, this.ProjectInfo.Id, importRequest.Parameters.ServiceEndpointId);
        if (serviceEndpoint == null || serviceEndpoint.Authorization == null || serviceEndpoint.Authorization.Parameters == null)
          return (ImportRepositoryValidation) null;
        string str1;
        if (serviceEndpoint.Authorization.Parameters.TryGetValue("Username", out str1))
          repositoryValidation.Username = str1;
        string str2;
        if (serviceEndpoint.Authorization.Parameters.TryGetValue("Password", out str2))
          repositoryValidation.Password = str2;
      }
      return repositoryValidation;
    }
  }
}
