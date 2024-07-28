// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitIntegration
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Serialization;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Git.Server
{
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03", Description = "DevOps VersionControl Integration web service")]
  [ProxyParentClass("GitClientProxy")]
  [ClientService(ServiceName = "GitIntegration", CollectionServiceIdentifier = "3B48BB21-AEF8-4218-95A9-FD73602A1B55")]
  public class GitIntegration : TeamFoundationWebService
  {
    private const string c_layer = "GitIntegrationService";
    private readonly string c_headsRefPath = "refs/heads";
    private readonly string c_tagsRefPath = "refs/tags";
    private const string s_Layer = "Integration.asmx";
    private static readonly IReadOnlyDictionary<string, string> s_filterMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        "heads",
        "refs/heads/"
      },
      {
        "tags",
        "refs/tags/"
      }
    };

    public GitIntegration() => this.RequestContext.ServiceName = "Git";

    [WebMethod]
    [SoapDocumentMethod(Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03/GetArtifacts")]
    [ClientIgnore]
    public Artifact[] GetArtifacts(string[] artifactUris)
    {
      IDictionary<Guid, ITfsGitRepository> repositoriesCache = (IDictionary<Guid, ITfsGitRepository>) new Dictionary<Guid, ITfsGitRepository>();
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetArtifacts), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (artifactUris), (IList<string>) artifactUris);
        this.EnterMethod(methodInformation);
        if (artifactUris == null || artifactUris.Length == 0)
          return Array.Empty<Artifact>();
        Artifact[] artifacts = new Artifact[artifactUris.Length];
        List<string> stringList = new List<string>();
        List<ArtifactId> artifactIdList = new List<ArtifactId>();
        for (int index = 0; index < artifactUris.Length; ++index)
        {
          ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUris[index]);
          try
          {
            artifacts[index] = !GitRefArtifactId.IsGitRefArtifactId(artifactId) ? (!PullRequestArtifactId.IsValid(artifactId) ? GitCommitArtifactId.DecodeArtifactUri(artifactUris[index]) : PullRequestArtifactId.GetArtifactForPullRequestArtifactUri(this.RequestContext, repositoriesCache, artifactUris[index], artifactId)) : GitRefArtifactId.DecodeArtifactUri(this.RequestContext, repositoriesCache, artifactUris[index], artifactId);
          }
          catch (ArgumentException ex)
          {
            this.RequestContext.TraceException(1013004, GitServerUtils.TraceArea, "GitIntegrationService", (Exception) ex);
          }
        }
        return artifacts;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(1013007, GitServerUtils.TraceArea, "Integration.asmx", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        foreach (KeyValuePair<Guid, ITfsGitRepository> keyValuePair in (IEnumerable<KeyValuePair<Guid, ITfsGitRepository>>) repositoriesCache)
        {
          try
          {
            if (keyValuePair.Value != null)
              keyValuePair.Value.Dispose();
          }
          catch (Exception ex)
          {
            this.RequestContext.TraceException(1013398, GitServerUtils.TraceArea, "Integration.asmx", ex);
          }
        }
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    [ClientIgnore]
    public bool DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.High, TimeSpan.FromHours(2.0));
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        if (string.IsNullOrEmpty(projectUri))
          throw new ArgumentNullException(nameof (projectUri));
        Guid projectId;
        try
        {
          projectId = ProjectInfo.GetProjectId(projectUri);
        }
        catch (ArgumentException ex)
        {
          return false;
        }
        try
        {
          this.RequestContext.GetService<IProjectService>().GetProject(this.RequestContext.Elevate(), projectId);
        }
        catch (ProjectDoesNotExistException ex)
        {
          return false;
        }
        this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().DeleteRepositories(this.RequestContext, new RepoScope(projectId, Guid.Empty));
        return true;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(1013008, GitServerUtils.TraceArea, "Integration.asmx", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateTeamProjectRepository(
      string teamProjectName,
      AccessControlEntryDetails[] permissions)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTeamProjectRepository), MethodType.Admin, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (teamProjectName), (object) teamProjectName);
        methodInformation.AddArrayParameter<AccessControlEntryDetails>(nameof (permissions), (IList<AccessControlEntryDetails>) permissions);
        this.EnterMethod(methodInformation);
        ProjectInfo project = this.RequestContext.GetService<IProjectService>().GetProject(this.RequestContext, teamProjectName);
        ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
        List<IAccessControlEntry> list = ((IEnumerable<AccessControlEntryDetails>) permissions).Select<AccessControlEntryDetails, IAccessControlEntry>((Func<AccessControlEntryDetails, IAccessControlEntry>) (p => p.ToAccessControlEntry())).ToList<IAccessControlEntry>();
        GitServerUtils.TranslateLegacyPermissionsToCurrentPermissions((IEnumerable<IAccessControlEntry>) list);
        IVssRequestContext requestContext = this.RequestContext;
        Guid id = project.Id;
        string repositoryName = teamProjectName;
        List<IAccessControlEntry> permissions1 = list;
        using (service.CreateTeamProjectRepository(requestContext, id, repositoryName, (IEnumerable<IAccessControlEntry>) permissions1))
          ;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRepositoryInfo[] QueryRepositories(
      string projectFilter)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryRepositories), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (projectFilter), (object) projectFilter);
        this.EnterMethod(methodInformation);
        string projectUriFilter = GitServerUtils.GetProjectUriFilter(this.RequestContext.Elevate(), projectFilter);
        IList<TfsGitRepositoryInfo> gitRepositoryInfoList = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(this.RequestContext, projectUriFilter, true);
        IProjectService service = this.RequestContext.GetService<IProjectService>();
        Dictionary<string, ProjectInfo> dictionary = new Dictionary<string, ProjectInfo>();
        IVssRequestContext requestContext = this.RequestContext.Elevate();
        foreach (ProjectInfo project in service.GetProjects(requestContext))
          dictionary.Add(project.Uri, project);
        string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(this.RequestContext);
        Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRepositoryInfo[] gitRepositoryInfoArray = new Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRepositoryInfo[gitRepositoryInfoList.Count];
        for (int index = 0; index < gitRepositoryInfoList.Count; ++index)
        {
          TfsGitRepositoryInfo gitRepositoryInfo = gitRepositoryInfoList[index];
          ProjectInfo projectInfo;
          if (dictionary.TryGetValue(gitRepositoryInfo.Key.GetProjectUri(), out projectInfo))
          {
            string repositoryCloneUrl = GitServerUtils.GetRepositoryCloneUrl(this.RequestContext, publicBaseUrl, projectInfo.Name, gitRepositoryInfo.Name);
            gitRepositoryInfoArray[index] = new Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRepositoryInfo(gitRepositoryInfoList[index], repositoryCloneUrl);
          }
        }
        return gitRepositoryInfoArray;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef[] QueryRefs(
      Guid repositoryId,
      string refTypeFilter)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryRefs), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (repositoryId), (object) repositoryId);
        methodInformation.AddParameter(nameof (refTypeFilter), (object) refTypeFilter);
        this.EnterMethod(methodInformation);
        if (refTypeFilter != null)
        {
          refTypeFilter = refTypeFilter.Trim();
          string str;
          if (GitIntegration.s_filterMap.TryGetValue(refTypeFilter, out str))
            refTypeFilter = str;
        }
        using (ITfsGitRepository repositoryById = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.RequestContext, repositoryId))
        {
          List<TfsGitRef> tfsGitRefList1 = repositoryById.Refs.MatchingNames((IEnumerable<string>) new string[1]
          {
            refTypeFilter
          }, GitRefSearchType.StartsWith);
          List<Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef> tfsGitRefList2 = new List<Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef>();
          foreach (TfsGitRef gitRef in (IEnumerable<TfsGitRef>) tfsGitRefList1)
          {
            if (gitRef.Name.StartsWith(this.c_headsRefPath, StringComparison.Ordinal))
              tfsGitRefList2.Add((Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef) new TfsGitBranchRef(this.RequestContext, gitRef));
            else if (gitRef.Name.StartsWith(this.c_tagsRefPath, StringComparison.Ordinal))
              tfsGitRefList2.Add((Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef) new TfsGitTagRef(this.RequestContext, gitRef));
            else
              tfsGitRefList2.Add(new Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRef(this.RequestContext, gitRef));
          }
          return tfsGitRefList2.ToArray();
        }
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private static class GitWebServiceConstants
    {
      public const string GitIntegration = "GitIntegration";
      public const string GitIntegrationIdentifierString = "3B48BB21-AEF8-4218-95A9-FD73602A1B55";
      public static readonly Guid GitIntegrationIdentifier = new Guid("3B48BB21-AEF8-4218-95A9-FD73602A1B55");
    }
  }
}
