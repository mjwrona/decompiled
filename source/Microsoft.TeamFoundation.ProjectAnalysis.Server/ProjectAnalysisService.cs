// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisService
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class ProjectAnalysisService : IProjectAnalysisService, IVssFrameworkService
  {
    private static readonly IList<IAnalyzer> s_analyzers = (IList<IAnalyzer>) new List<IAnalyzer>()
    {
      (IAnalyzer) new LanguageMetadataAnalyzer()
    };
    private const string c_layer = "ProjectAnalysisService";

    public ProjectAnalysisService()
      : this(ProjectAnalysisService.s_analyzers)
    {
    }

    public ProjectAnalysisService(IList<IAnalyzer> analyzers)
    {
      ArgumentUtility.CheckForNull<IList<IAnalyzer>>(analyzers, nameof (analyzers));
      this.Analyzers = analyzers;
    }

    public ProjectLanguageAnalytics GetLanguageAnalytics(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> repositoryIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ProjectAnalysisSecurityConstants.SecurityNamespaceId);
      securityNamespace.CheckReadPermission(requestContext, projectId.ToLanguageMetricsToken());
      if (!requestContext.IsFeatureEnabled("ProjectAnalysis.Server.EnableProjectAnalysis"))
        return new ProjectLanguageAnalytics(projectId)
        {
          LanguageBreakdown = (IList<LanguageStatistics>) new List<LanguageStatistics>(),
          RepositoryLanguageAnalytics = (IList<RepositoryLanguageAnalytics>) new List<RepositoryLanguageAnalytics>(),
          ResultPhase = ResultPhase.Preliminary
        };
      if (repositoryIds != null)
        throw new NotImplementedException(nameof (repositoryIds));
      IAnalyzer analyzer = this.Analyzers.Where<IAnalyzer>((Func<IAnalyzer, bool>) (a => string.Equals("Language", a.AnalyzerDescriptor.Name))).FirstOrDefault<IAnalyzer>();
      using (requestContext.TraceBlock(15280004, 15280005, nameof (ProjectAnalysisService), nameof (ProjectAnalysisService), MethodBase.GetCurrentMethod().Name))
      {
        List<IRepositoryDescriptor> allRepositories = this.GetAllRepositories(requestContext, projectId);
        Stopwatch stopwatch = Stopwatch.StartNew();
        IAnalytics analytics;
        if (analyzer.TryGetAnalytics(requestContext, projectId, (IEnumerable<IRepositoryDescriptor>) allRepositories, out analytics))
        {
          long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
          requestContext.Trace(15280002, TraceLevel.Verbose, nameof (ProjectAnalysisService), nameof (ProjectAnalysisService), string.Format("GetLanguageAnalytics:{0}:{1}", (object) analyzer.AnalyzerDescriptor.Id, (object) elapsedMilliseconds));
        }
        else
        {
          requestContext.Trace(15280006, TraceLevel.Verbose, nameof (ProjectAnalysisService), nameof (ProjectAnalysisService), "Analyzer {0} with Id {1} did not find any files to analyze on project {2}", (object) analyzer.AnalyzerDescriptor.Name, (object) analyzer.AnalyzerDescriptor.Id, (object) projectId);
          analytics = (IAnalytics) new ProjectLanguageAnalytics(projectId)
          {
            LanguageBreakdown = (IList<LanguageStatistics>) new List<LanguageStatistics>(),
            RepositoryLanguageAnalytics = (IList<RepositoryLanguageAnalytics>) new List<RepositoryLanguageAnalytics>(),
            ResultPhase = ResultPhase.Preliminary
          };
        }
        ProjectLanguageAnalytics languageAnalytics = analytics as ProjectLanguageAnalytics;
        return securityNamespace.HasPermission(requestContext, languageAnalytics.GetToken(), 1) ? languageAnalytics : (ProjectLanguageAnalytics) null;
      }
    }

    private List<IRepositoryDescriptor> GetAllRepositories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      List<IRepositoryDescriptor> allRepositories = new List<IRepositoryDescriptor>();
      allRepositories.AddRange(this.GetAllValidGitRepositories(requestContext, projectId));
      IRepositoryDescriptor tfvcRepository = this.GetTfvcRepository(requestContext, projectId);
      if (tfvcRepository != null)
        allRepositories.Add(tfvcRepository);
      return allRepositories;
    }

    private IEnumerable<IRepositoryDescriptor> GetAllValidGitRepositories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      string projectUri = ProjectInfo.GetProjectUri(projectId);
      return (IEnumerable<IRepositoryDescriptor>) requestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(requestContext, projectUri, true).Where<TfsGitRepositoryInfo>((Func<TfsGitRepositoryInfo, bool>) (r => r.DefaultBranch != null)).Select<TfsGitRepositoryInfo, TfsGitRepositoryDescriptor>((Func<TfsGitRepositoryInfo, TfsGitRepositoryDescriptor>) (r => new TfsGitRepositoryDescriptor()
      {
        Name = r.Name,
        Id = r.Key.RepoId,
        ProjectId = projectId
      }));
    }

    private IRepositoryDescriptor GetTfvcRepository(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProjectInfo project;
      return requestContext.IsFeatureEnabled("ProjectAnalysis.Server.EnableProjectAnalysis.Tfvc") && requestContext.TryGetTfvcProject(projectId, out project) ? (IRepositoryDescriptor) new TfvcRepositoryDescriptor(project) : (IRepositoryDescriptor) null;
    }

    [ExcludeFromCodeCoverage]
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<IAnalyzer> Analyzers { get; private set; }

    public IFileAccessorFactory RepoFileAccessoryFactory { get; private set; }
  }
}
