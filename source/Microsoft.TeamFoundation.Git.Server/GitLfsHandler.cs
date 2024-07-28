// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class GitLfsHandler : GitHttpHandler
  {
    public GitLfsHandler()
    {
    }

    public GitLfsHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected RepoNameKey RepoInfo { get; private set; }

    protected void FillRepoInfo() => this.RepoInfo = RepoNameKey.FromRequest(this.HandlerHttpContext.Request);

    protected void EnterMethod(string action)
    {
      MethodInformation methodInformation = new MethodInformation("GitLfsHandler_" + action, MethodType.Normal, EstimatedMethodCost.Moderate, TimeSpan.FromHours(24.0));
      methodInformation.AddParameter("ProjectName", (object) this.RepoInfo.ProjectName);
      methodInformation.AddParameter("RepositoryName", (object) this.RepoInfo.RepositoryName);
      this.EnterMethod(methodInformation);
    }

    internal override sealed void Execute()
    {
      try
      {
        GitLfsHandler.LfsHttpResult result = this.ErrorResponse(HttpStatusCode.BadRequest);
        this.FillRepoInfo();
        if (this.RepoInfo.IsValid)
          result = this.GetResult();
        this.WriteHttpResult(result);
      }
      catch (Exception ex) when (this.HandleException(ex, true, this.GetReportAction()))
      {
      }
    }

    protected abstract GitLfsHandler.LfsHttpResult GetResult();

    protected GitLfsHandler.LfsHttpResult ErrorResponse(HttpStatusCode statusCode, string message = null)
    {
      GitLfsErrorResponse resultObj = new GitLfsErrorResponse()
      {
        DocumentationUrl = (Uri) null,
        Message = message ?? HttpWorkerRequest.GetStatusDescription((int) statusCode),
        RequestId = this.RequestContext.ActivityId.ToString()
      };
      return this.GetLfsHttpResult<GitLfsErrorResponse>(statusCode, resultObj);
    }

    protected GitLfsHandler.LfsHttpResult GetLfsHttpResult<T>(
      HttpStatusCode statusCode,
      T resultObj)
    {
      return new GitLfsHandler.LfsHttpResult(statusCode)
      {
        ContentType = "application/vnd.git-lfs+json",
        ProcessBody = (Action) (() => this.HandlerHttpContext.Response.Write(JsonConvert.SerializeObject((object) (T) resultObj)))
      };
    }

    protected virtual Action<string> GetReportAction() => (Action<string>) null;

    protected void WriteHttpResult(GitLfsHandler.LfsHttpResult result)
    {
      try
      {
        this.HandlerHttpContext.Response.StatusCode = (int) result.StatusCode;
        this.HandlerHttpContext.Response.ContentType = result.ContentType;
        Action processBody = result.ProcessBody;
        if (processBody == null)
          return;
        processBody();
      }
      catch (Exception ex)
      {
        this.HandlerHttpContext.Response.Close();
      }
    }

    protected ITfsGitRepository FindRepository()
    {
      ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
      ITfsGitRepository repository;
      if (service.TryFindRepositoryByName(this.RequestContext, this.RepoInfo.ProjectName, this.RepoInfo.RepositoryName, out repository))
        return repository;
      if (this.RepoInfo.RepositoryName.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
      {
        string str = this.RepoInfo.RepositoryName.Substring(0, this.RepoInfo.RepositoryName.Length - ".git".Length);
        if (this.RepoInfo.ProjectName == this.RepoInfo.RepositoryName && service.TryFindRepositoryByName(this.RequestContext, str, str, out repository) || service.TryFindRepositoryByName(this.RequestContext, this.RepoInfo.ProjectName, str, out repository))
          return repository;
      }
      throw new GitRepositoryNotFoundException(this.RepoInfo.RepositoryName);
    }

    protected bool AcceptsMediaType(string mediaType)
    {
      string[] acceptTypes = this.HandlerHttpContext.Request.AcceptTypes;
      if (acceptTypes != null)
      {
        foreach (string input in acceptTypes)
        {
          if (MediaTypeWithQualityHeaderValue.Parse(input).MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    protected class LfsHttpResult
    {
      public LfsHttpResult(HttpStatusCode statusCode) => this.StatusCode = statusCode;

      public HttpStatusCode StatusCode { get; private set; }

      public string ContentType { get; set; }

      public Action ProcessBody { get; set; }
    }
  }
}
