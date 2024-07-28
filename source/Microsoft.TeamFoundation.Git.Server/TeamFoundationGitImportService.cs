// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitImportService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Import;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitImportService : 
    TeamFoundationGitAsyncOperationService<GitImportRequest, InternalGitImportRequestParameters, GitImportStatusDetail>,
    ITeamFoundationGitImportService,
    IVssFrameworkService
  {
    private static readonly GitImportStatusDetail m_defaultStatusDetail = new GitImportStatusDetail()
    {
      CurrentStep = 1,
      AllSteps = (IEnumerable<string>) GitImportAllSteps.GetAllSteps()
    };
    private const string c_initialImportJobIntervalInSecondsRegistryPath = "/Service/Git/Settings/Import/InitialImportJobIntervalInSeconds";
    private const int c_defaultInitialJobIntervalInSeconds = 15;

    public GitImportRequest CreateImportRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      InternalGitImportRequestParameters parameters,
      Guid? projectId = null)
    {
      repository.Permissions.CheckWrite(false);
      if (repository.Refs.GetDefaultOrAny() != null && (parameters.GitSource == null || !parameters.GitSource.Overwrite))
        throw new GitImportForbiddenOnNonEmptyRepository();
      IVssRequestContext requestContext1 = requestContext;
      string traceArea1 = GitServerUtils.TraceArea;
      string gitImportLayer1 = GitServerUtils.GitImportLayer;
      Guid guid = repository.Key.RepoId;
      string str1 = guid.ToString();
      requestContext1.Trace(1013593, TraceLevel.Verbose, traceArea1, gitImportLayer1, "Creating import operation for repository id {0}", (object) str1);
      GitImportRequest asyncOperation = this.CreateAsyncOperation(requestContext, repository.Key, GitAsyncOperationType.Import, parameters);
      IVssRequestContext requestContext2 = requestContext;
      string traceArea2 = GitServerUtils.TraceArea;
      string gitImportLayer2 = GitServerUtils.GitImportLayer;
      guid = asyncOperation.RepositoryId;
      string str2 = guid.ToString();
      // ISSUE: variable of a boxed type
      __Boxed<int> operationId = (ValueType) asyncOperation.OperationId;
      requestContext2.Trace(1013594, TraceLevel.Verbose, traceArea2, gitImportLayer2, "Created import request for repository id {0} with operation id {1}", (object) str2, (object) operationId);
      ClientTraceData ctData = new ClientTraceData();
      this.QueueImportJob(requestContext, asyncOperation.RepositoryId, asyncOperation.OperationId, projectId, ctData);
      ctData.Add("Action", (object) "CreateImport");
      TeamFoundationGitImportService.PopulateSourceHostCiData(parameters, ctData);
      this.PublishCtData(requestContext, repository, asyncOperation.OperationId, parameters, ctData);
      return this.FillDefaultDetailedStatus(asyncOperation);
    }

    public GitImportRequest GetImportRequestById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int importOperationId)
    {
      return this.FillDefaultDetailedStatus(this.GetAsyncOperationById(requestContext, repository.Key, importOperationId, true));
    }

    public IEnumerable<GitImportRequest> QueryImportRequests(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      bool includeAbandoned)
    {
      IEnumerable<GitImportRequest> source = this.QueryAsyncOperationsByType(requestContext, repository.Key, GitAsyncOperationType.Import);
      return (includeAbandoned ? source : source.Where<GitImportRequest>((Func<GitImportRequest, bool>) (x => x.Status != GitAsyncOperationStatus.Abandoned))).Select<GitImportRequest, GitImportRequest>((Func<GitImportRequest, GitImportRequest>) (x => this.FillDefaultDetailedStatus(x)));
    }

    public GitImportRequest UpdateImportStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int importOperationId,
      GitAsyncOperationStatus status,
      GitImportStatusDetail detailedStatus)
    {
      repository.Permissions.CheckWrite(false);
      GitImportRequest asyncOperationById = this.GetAsyncOperationById(requestContext, repository.Key, importOperationId, true);
      switch (status)
      {
        case GitAsyncOperationStatus.Queued:
          if (asyncOperationById.Status != GitAsyncOperationStatus.Failed)
            throw new InvalidOperationException(Resources.Get("GitImportRequestRetryDenied"));
          if (repository.Refs.GetDefaultOrAny() != null)
            throw new InvalidOperationException(Resources.Get("ImportInvalidRepository"));
          requestContext.Trace(1013595, TraceLevel.Verbose, GitServerUtils.TraceArea, GitServerUtils.GitImportLayer, "Retrying import request for repository id {0} with operation id {1}", (object) repository.Key.RepoId, (object) importOperationId);
          this.UpdateAsyncOperationStatus(requestContext, repository.Key, importOperationId, status, TeamFoundationGitImportService.m_defaultStatusDetail, new GitAsyncOperationStatus?(GitAsyncOperationStatus.Failed));
          ClientTraceData ctData1 = new ClientTraceData();
          this.QueueImportJob(requestContext, repository.Key.RepoId, importOperationId, new Guid?(), ctData1);
          ctData1.Add("Action", (object) "RetryImport");
          this.PublishCtData(requestContext, repository, importOperationId, asyncOperationById.Parameters, ctData1);
          break;
        case GitAsyncOperationStatus.Abandoned:
          this.UpdateAsyncOperationStatus(requestContext, repository.Key, importOperationId, status, detailedStatus, new GitAsyncOperationStatus?());
          ClientTraceData ctData2 = new ClientTraceData();
          ctData2.Add("Action", (object) "CancelImport");
          this.PublishCtData(requestContext, repository, importOperationId, asyncOperationById.Parameters, ctData2);
          break;
        default:
          this.UpdateAsyncOperationStatus(requestContext, repository.Key, importOperationId, status, detailedStatus, new GitAsyncOperationStatus?());
          break;
      }
      return this.GetAsyncOperationById(requestContext, repository.Key, importOperationId, true);
    }

    private static void PopulateSourceHostCiData(
      InternalGitImportRequestParameters parameters,
      ClientTraceData ctData)
    {
      string str = string.Empty;
      if (parameters.GitSource != null)
        str = TeamFoundationGitImportService.GetUriHost(parameters.GitSource.Url);
      if (string.IsNullOrWhiteSpace(str))
        return;
      ctData.Add("ImportSourceUrlHost", (object) str);
    }

    private static string GetUriHost(string uriString)
    {
      try
      {
        return new Uri(uriString).Host;
      }
      catch
      {
        return string.Empty;
      }
    }

    private static string GetUriScheme(string uriString)
    {
      try
      {
        return new Uri(uriString).Scheme;
      }
      catch
      {
        return string.Empty;
      }
    }

    private Guid QueueImportJob(
      IVssRequestContext requestContext,
      Guid repositoryId,
      int importOperationId,
      Guid? projectId,
      ClientTraceData ctData)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/Import/InitialImportJobIntervalInSeconds", 15);
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
      {
        Interval = num,
        ScheduledTime = DateTime.UtcNow
      };
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AsyncGitOperationJobData()
      {
        RepositoryId = repositoryId,
        OperationId = importOperationId,
        ProjectId = projectId
      });
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(Guid.NewGuid(), "GitImportJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitImportJob", xml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal);
      ctData.Add("ImportJobId", (object) foundationJobDefinition.JobId);
      foundationJobDefinition.Schedule.Add(foundationJobSchedule);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      string message = string.Format("A job with jobId: '{0}', is scheduled for repositoryId: '{1}' and importOperationId: '{2}'.", (object) foundationJobDefinition.JobId, (object) repositoryId, (object) importOperationId);
      requestContext.Trace(1013583, TraceLevel.Info, GitServerUtils.TraceArea, GitServerUtils.GitImportLayer, message);
      service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        foundationJobDefinition.JobId
      });
      return foundationJobDefinition.JobId;
    }

    private void PublishCtData(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int importOperationId,
      InternalGitImportRequestParameters gitImportRequestParameters,
      ClientTraceData ctData)
    {
      try
      {
        ctData.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
        ctData.Add("RepositoryName", (object) repository.Name);
        ctData.Add("AsyncOperationId", (object) importOperationId);
        ctData.Add("ImportUserAgent", (object) gitImportRequestParameters.UserAgent);
        if (gitImportRequestParameters.ServiceEndpointId != Guid.Empty)
          ctData.Add("ImportSourceIsPrivate", (object) true);
        else
          ctData.Add("ImportSourceIsPrivate", (object) false);
        string uriHost = TeamFoundationGitImportService.GetUriHost(gitImportRequestParameters.GitSource.Url);
        if (!string.IsNullOrWhiteSpace(uriHost))
          ctData.Add("ImportSourceUrlHost", (object) uriHost);
        string uriScheme = TeamFoundationGitImportService.GetUriScheme(gitImportRequestParameters.GitSource.Url);
        if (!string.IsNullOrWhiteSpace(uriScheme))
          ctData.Add("ImportSourceUrlScheme", (object) uriScheme);
        string str = ((IEnumerable<string>) gitImportRequestParameters.GitSource.Url.TrimEnd('/').Split('/')).Last<string>();
        if (str.EndsWith(".git"))
          str = str.Substring(0, str.LastIndexOf(".git"));
        ctData.Add("ImportSourceRepositoryName", (object) str);
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "Import", ctData);
      }
      catch
      {
      }
    }

    private GitImportRequest FillDefaultDetailedStatus(GitImportRequest importRequest)
    {
      if (importRequest.DetailedStatus == null)
        importRequest.DetailedStatus = TeamFoundationGitImportService.m_defaultStatusDetail;
      return importRequest;
    }
  }
}
