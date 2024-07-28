// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportHelper
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  public class PermissionsReportHelper
  {
    private const string RepoTokenTemplate = "repov2/{0}/{1}";
    private const string ProjectGitTokenTemplate = "repov2/{0}";

    public static string GetPermissionSetToken(
      IVssRequestContext requestContext,
      PermissionsReportResource resource)
    {
      switch (resource.ResourceType)
      {
        case ResourceType.Collection:
          return FrameworkSecurity.CollectionManagementNamespaceToken + (object) FrameworkSecurity.CollectionManagementPathSeparator + requestContext.ServiceHost.InstanceId.ToString();
        case ResourceType.Project:
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstfs:///Classification/TeamProject/{0}", (object) PermissionsReportHelper.GetProject(requestContext, resource.ResourceId).Id);
        case ResourceType.Repo:
          return PermissionsReportHelper.GetRepoToken(requestContext, resource.ResourceId);
        case ResourceType.Ref:
          return PermissionsReportHelper.GetRefToken(requestContext, resource.ResourceId);
        case ResourceType.ProjectGit:
          return PermissionsReportHelper.GetRepoToken(requestContext, resource.ResourceId, string.Empty);
        case ResourceType.Release:
          return PermissionsReportHelper.ValidateReleaseToken(requestContext, resource.ResourceId);
        case ResourceType.Tfvc:
          return PermissionsReportHelper.GetTfvcToken(requestContext, resource.ResourceId);
        default:
          return string.Empty;
      }
    }

    private static string GetRefToken(IVssRequestContext requestContext, string resourceId)
    {
      int length = resourceId.IndexOf("/refs/");
      string resourceId1 = length != -1 ? resourceId.Substring(0, length) : throw new ArgumentException("Ref resource " + resourceId + " is invalid. Missing /refs");
      string refName = resourceId.Substring(length + 1);
      string repoToken = PermissionsReportHelper.GetRepoToken(requestContext, resourceId1);
      string input = repoToken.Substring(repoToken.LastIndexOf('/') + 1);
      using (ITfsGitRepository repo = PermissionsReportHelper.GetRepo(requestContext, Guid.Parse(input)))
      {
        if (!refName.EndsWith("tags") && !refName.EndsWith("heads"))
        {
          if (repo.Refs.MatchingName(refName) == null)
            goto label_9;
        }
        return repoToken + "/" + refName.Replace('/', '^');
      }
label_9:
      throw new ArgumentException("Ref resource " + resourceId + " is invalid.");
    }

    private static string GetRepoToken(IVssRequestContext requestContext, string resourceId)
    {
      Guid result = Guid.Empty;
      if (Guid.TryParse(resourceId, out result))
        return PermissionsReportHelper.GetRepoToken(requestContext, result);
      string[] strArray = resourceId.Split('/');
      return strArray != null && strArray.Length == 2 ? PermissionsReportHelper.GetRepoToken(requestContext, strArray[0], strArray[1]) : throw new ArgumentException("Repo resource " + resourceId + " is invalid.");
    }

    private static string GetRepoToken(
      IVssRequestContext requestContext,
      string projectId,
      string repoName)
    {
      Guid empty = Guid.Empty;
      Guid guid = Guid.Empty;
      ProjectInfo project = PermissionsReportHelper.GetProject(requestContext, projectId);
      Guid id = project.Id;
      if (string.IsNullOrWhiteSpace(repoName))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "repov2/{0}", (object) id);
      using (ITfsGitRepository repo = PermissionsReportHelper.GetRepo(requestContext, project.Name, repoName))
        guid = repo.Key.RepoId;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "repov2/{0}/{1}", (object) id, (object) guid);
    }

    private static string GetRepoToken(IVssRequestContext requestContext, Guid repoId)
    {
      requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repo = PermissionsReportHelper.GetRepo(requestContext, repoId))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "repov2/{0}/{1}", (object) repo.Key.ProjectId, (object) repo.Key.RepoId);
    }

    private static ITfsGitRepository GetRepo(
      IVssRequestContext requestContext,
      string projectName,
      string repoName)
    {
      return requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(requestContext, projectName, repoName);
    }

    private static ITfsGitRepository GetRepo(IVssRequestContext requestContext, Guid repoId) => requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repoId);

    private static string ValidateReleaseToken(IVssRequestContext requestContext, string resourceId)
    {
      Guid result = Guid.Empty;
      IProjectService service = requestContext.GetService<IProjectService>();
      if (!Guid.TryParse(resourceId, out result))
        Guid.TryParse(((!string.IsNullOrEmpty(resourceId) ? resourceId.Split('/') : throw new ArgumentException("Release resource " + resourceId + " is null or empty.")) ?? throw new ArgumentException("Release resource " + resourceId + " is invalid."))[0], out result);
      if (service.GetProject(requestContext, result) == null)
        throw new ArgumentException("Release resource " + resourceId + " is invalid.");
      return resourceId;
    }

    private static string GetTfvcToken(IVssRequestContext requestContext, string resourceId) => resourceId.StartsWith("$/", StringComparison.OrdinalIgnoreCase) ? resourceId : throw new ArgumentException("Tfvc resource " + resourceId + " is invalid.");

    private static ProjectInfo GetProject(IVssRequestContext requestContext, string projectId)
    {
      Guid result = Guid.Empty;
      IProjectService service = requestContext.GetService<IProjectService>();
      return (!Guid.TryParse(projectId, out result) ? service.GetProject(requestContext, projectId) : service.GetProject(requestContext, result)) ?? throw new ArgumentException("Project resource " + projectId + " is invalid.");
    }

    public static string GetResourceName(
      IVssRequestContext requestContext,
      PermissionsReportResource resource)
    {
      return resource.ResourceType == ResourceType.Repo ? PermissionsReportHelper.GetRepoName(requestContext, resource.ResourceId) : string.Empty;
    }

    private static string GetRepoName(IVssRequestContext requestContext, string resourceId)
    {
      Guid result = Guid.Empty;
      if (Guid.TryParse(resourceId, out result))
      {
        requestContext.GetService<ITeamFoundationGitRepositoryService>();
        using (ITfsGitRepository repo = PermissionsReportHelper.GetRepo(requestContext, result))
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, repo.Name);
      }
      else
      {
        string[] strArray = resourceId.Split('/');
        return strArray != null && strArray.Length == 2 ? PermissionsReportHelper.GetRepoName(requestContext, strArray[0], strArray[1]) : throw new ArgumentException("Repo resource " + resourceId + " is invalid.");
      }
    }

    private static string GetRepoName(
      IVssRequestContext requestContext,
      string projectId,
      string repoId)
    {
      ProjectInfo project = PermissionsReportHelper.GetProject(requestContext, projectId);
      using (ITfsGitRepository repo = PermissionsReportHelper.GetRepo(requestContext, project.Name, repoId))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, repo.Name);
    }
  }
}
