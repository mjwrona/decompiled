// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiV2Helper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiV2Helper
  {
    private const long c_maxFileSizeInBytes = 26214400;
    private const string c_gitIgnoreTemplateType = "gitignore";
    private const string c_gitIgnoreFilePath = "/.gitignore";
    private static Guid s_fileSizePolicyId = new Guid("2e26e725-8201-4edd-8bf5-978563c34a80");

    public static WikiV2 CreateWiki(
      IVssRequestContext requestContext,
      WikiCreateParametersV2 parameters,
      WikiJobHandler wikiJobHandler)
    {
      Guid guid = parameters != null ? parameters.ProjectId : throw new InvalidArgumentValueException(nameof (parameters), Resources.InvalidParametersOrNull);
      if (parameters.ProjectId == Guid.Empty)
        throw new InvalidArgumentValueException("ProjectId", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "ProjectId"));
      requestContext.Trace(15250301, TraceLevel.Info, "Wiki", "Service", "Creating wiki for project id: {0}", (object) parameters.ProjectId.ToString());
      WikiV2 wiki = parameters.Type != WikiType.CodeWiki ? WikiV2Helper.CreateProjectWiki(requestContext, parameters) : WikiV2Helper.CreateCodeWiki(requestContext, parameters);
      wiki.PopulateUrls(requestContext);
      wikiJobHandler.QueueWikiCreatedJob(requestContext, wiki);
      return wiki;
    }

    public static WikiV2 UpdateWiki(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string wikiIdentifier,
      IEnumerable<GitVersionDescriptor> versions,
      WikiJobHandler wikiJobHandler,
      string newName = null)
    {
      if (versions == null && string.IsNullOrEmpty(newName))
        throw new InvalidArgumentValueException("versions & newName", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "versions & newName"));
      WikiV2 wiki = WikiV2Helper.GetWikiByIdentifier(requestContext, teamProjectId, wikiIdentifier);
      if (wiki == null)
        return (WikiV2) null;
      if (wiki.Type == WikiType.ProjectWiki && versions != null && versions.Count<GitVersionDescriptor>() != 0)
        throw new WikiOperationNotSupportedException(string.Format(Resources.ProjectWikiUpdateErrorMessage, (object) nameof (versions)));
      if (requestContext.IsFeatureEnabled("WebAccess.Wiki.RenameWiki") && !string.IsNullOrEmpty(newName))
        wiki = WikiV2Helper.RenameWiki(requestContext, wiki, newName);
      if (versions != null)
      {
        if (versions.Count<GitVersionDescriptor>() == 0)
          throw new InvalidArgumentValueException(nameof (versions), string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) nameof (versions)));
        HashSet<GitVersionDescriptor> addedVersions;
        HashSet<GitVersionDescriptor> deletedVersions;
        WikiV2Helper.IdentifyVersionChanges(wiki.Versions, versions, out addedVersions, out deletedVersions);
        WikiV2Helper.UpdateVersions(requestContext, wiki, addedVersions, deletedVersions);
        wikiJobHandler.QueueWikiVersionChangeJob(requestContext, wiki, (IEnumerable<GitVersionDescriptor>) addedVersions, (IEnumerable<GitVersionDescriptor>) deletedVersions);
      }
      return wiki;
    }

    public static WikiV2 GetDefaultWiki(IVssRequestContext requestContext, Guid projectId)
    {
      IEnumerable<WikiV2> wikis = WikiV2Helper.GetWikis(requestContext, projectId);
      return (wikis != null ? wikis.FirstOrDefault<WikiV2>((Func<WikiV2, bool>) (w => w.Type.Equals((object) WikiType.ProjectWiki))) : (WikiV2) null) ?? (wikis != null ? wikis.FirstOrDefault<WikiV2>() : (WikiV2) null);
    }

    public static WikiV2 RenameWiki(IVssRequestContext requestContext, WikiV2 wiki, string name)
    {
      WikiV2Helper.ThrowIfInsufficientPermissionToRenamewiki(requestContext, wiki);
      WikiV2Helper.ValidateAndNormalizeWikiName(ref name);
      if (wiki.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        return wiki;
      string name1 = wiki.Name;
      WikiV2 wikiByName = WikiV2Helper.GetWikiByName(requestContext, wiki.ProjectId, name);
      if (wikiByName != null)
      {
        if (wikiByName.Id == wiki.Id)
          return wiki;
        throw new WikiAlreadyExistsException(string.Format(Resources.WikiAlreadyExistsWithName, (object) name));
      }
      WikiV2Helper.TryRenameWiki(requestContext, wiki, name);
      if (WikiV2Helper.HasMultipleWikiByName(requestContext, wiki))
      {
        WikiV2Helper.TryRenameWiki(requestContext, wiki, name1);
        throw new WikiRenameConflictException(string.Format(Resources.WikiRenameFail, (object) Resources.WikiAlreadyExistsWithName));
      }
      ProjectPropertiesHelper.PublishNotification(requestContext, wiki.ProjectId, wiki.RepositoryId);
      return wiki;
    }

    internal static void IdentifyVersionChanges(
      IEnumerable<GitVersionDescriptor> oldVersions,
      IEnumerable<GitVersionDescriptor> newVersions,
      out HashSet<GitVersionDescriptor> addedVersions,
      out HashSet<GitVersionDescriptor> deletedVersions)
    {
      if (oldVersions == null)
        oldVersions = Enumerable.Empty<GitVersionDescriptor>();
      if (newVersions == null)
        newVersions = Enumerable.Empty<GitVersionDescriptor>();
      GitVersionDescriptorComparer comparer = new GitVersionDescriptorComparer();
      addedVersions = new HashSet<GitVersionDescriptor>(newVersions.Except<GitVersionDescriptor>(oldVersions, (IEqualityComparer<GitVersionDescriptor>) comparer), (IEqualityComparer<GitVersionDescriptor>) comparer);
      deletedVersions = new HashSet<GitVersionDescriptor>(oldVersions.Except<GitVersionDescriptor>(newVersions, (IEqualityComparer<GitVersionDescriptor>) comparer), (IEqualityComparer<GitVersionDescriptor>) comparer);
    }

    internal static void UpdateVersions(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      HashSet<GitVersionDescriptor> addedVersions,
      HashSet<GitVersionDescriptor> deletedVersions)
    {
      if (addedVersions.Count == 0 && deletedVersions.Count == 0)
        return;
      int maximumVersionCount = RegistryHelper.GetMaximumVersionCount(requestContext);
      if (wiki.Versions.Count<GitVersionDescriptor>() - deletedVersions.Count + addedVersions.Count > maximumVersionCount)
        throw new WikiOperationNotSupportedException(string.Format(Resources.WikiVersionsMaximumThresholdReached, (object) maximumVersionCount));
      List<GitVersionDescriptor> source = new List<GitVersionDescriptor>();
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, wiki.RepositoryId))
      {
        source = wiki.Versions.Where<GitVersionDescriptor>((Func<GitVersionDescriptor, bool>) (v => !deletedVersions.Contains(v))).ToList<GitVersionDescriptor>();
        foreach (GitVersionDescriptor addedVersion in addedVersions)
        {
          WikiV2Helper.ThrowIfInvalidWikiVersion(repositoryById, addedVersion);
          WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, wiki.ProjectId, wiki.RepositoryId, addedVersion);
          WikiV2Helper.ThrowIfInvalidWikiMappedPath(requestContext, repositoryById, addedVersion, wiki.MappedPath);
          source.Add(addedVersion);
        }
      }
      wiki.Versions = source.Distinct<GitVersionDescriptor>((IEqualityComparer<GitVersionDescriptor>) new GitVersionDescriptorComparer());
      ProjectPropertiesHelper.AddOrUpdateWikiToProperties(requestContext, wiki.ProjectId, wiki);
    }

    private static void TryRenameWiki(IVssRequestContext requestContext, WikiV2 wiki, string name)
    {
      if (wiki.Type == WikiType.ProjectWiki)
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, wiki.RepositoryId))
        {
          try
          {
            service.RenameRepository(requestContext, repositoryById.Key, name)?.Dispose();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(15252803, "Wiki", "Service", ex);
            throw new WikiRenameConflictException(string.Format(Resources.WikiRenameFail, (object) ex.Message));
          }
        }
      }
      wiki.Name = name;
      ProjectPropertiesHelper.AddOrUpdateWikiToProperties(requestContext, wiki.ProjectId, wiki, false);
    }

    private static bool HasMultipleWikiByName(IVssRequestContext requestContext, WikiV2 wiki) => ProjectPropertiesHelper.GetAllWikisFromProperties(requestContext, wiki.ProjectId).Count<WikiV2>((Func<WikiV2, bool>) (w => w.Name.Equals(wiki.Name, StringComparison.OrdinalIgnoreCase))) > 1;

    public static WikiV2 DeleteWiki(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string wikiIdentifier,
      WikiJobHandler wikiJobHandler)
    {
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(requestContext, teamProjectId, wikiIdentifier);
      if (wikiByIdentifier == null)
        return (WikiV2) null;
      if (wikiByIdentifier.Type == WikiType.ProjectWiki)
        throw new WikiOperationNotSupportedException(string.Format(Resources.WikiDeleteNotSupported, (object) "ProjectWiki"));
      WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, wikiByIdentifier.ProjectId, wikiByIdentifier.RepositoryId);
      ProjectPropertiesHelper.DeleteWikiProperties(requestContext, wikiByIdentifier.ProjectId, wikiByIdentifier.Id, wikiByIdentifier.RepositoryId);
      wikiJobHandler.QueueWikiDeletedJob(requestContext, wikiByIdentifier);
      return wikiByIdentifier;
    }

    public static IEnumerable<WikiV2> GetWikis(
      IVssRequestContext requestContext,
      Guid teamProjectId)
    {
      return (IEnumerable<WikiV2>) ProjectPropertiesHelper.GetAllWikisFromProperties(requestContext, teamProjectId).Where<WikiV2>((Func<WikiV2, bool>) (wiki => WikiV2Helper.HasWikiReadPermissions(requestContext, wiki, wiki.ProjectId))).ToList<WikiV2>();
    }

    public static WikiV2 GetWikiByIdentifier(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string wikiIdentifier)
    {
      Guid result;
      WikiV2 wikiByIdentifier;
      if (Guid.TryParse(wikiIdentifier, out result))
      {
        wikiByIdentifier = WikiV2Helper.GetWikiById(requestContext, teamProjectId, result);
      }
      else
      {
        if (teamProjectId.Equals(Guid.Empty))
          throw new ScopeNotSupportedException(Resources.GetWikiByName, TfsApiResourceScope.Collection, Resources.GetWikiByNameProjectScopeRequired);
        string wikiName = wikiIdentifier;
        wikiByIdentifier = WikiV2Helper.GetWikiByName(requestContext, teamProjectId, wikiName);
      }
      return wikiByIdentifier;
    }

    public static ITfsGitRepository GetWikiRepository(
      IVssRequestContext requestContext,
      Guid wikiRepositoryId)
    {
      return requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, wikiRepositoryId, includeDisabled: true);
    }

    public static bool RepoHasAssociatedWiki(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (RepoHasAssociatedWiki)))
        return ProjectPropertiesHelper.GetAllWikisInAProject(requestContext, repo.Key.ProjectId).Any<WikiV2>((Func<WikiV2, bool>) (w => w.RepositoryId.Equals(repo.Key.RepoId)));
    }

    public static IEnumerable<WikiV2> GetWikis(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repoId,
      bool cleanupMissingProperties = false)
    {
      ArgumentUtility.CheckForEmptyGuid(teamProjectId, nameof (teamProjectId));
      ArgumentUtility.CheckForEmptyGuid(repoId, nameof (repoId));
      return ProjectPropertiesHelper.GetAllWikisInAProject(requestContext, teamProjectId, cleanupMissingProperties).Where<WikiV2>((Func<WikiV2, bool>) (w => w.RepositoryId.Equals(repoId)));
    }

    public static WikiV2 GetWikiById(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid wikiId,
      TimedCiEvent ciData = null)
    {
      using (new StopWatchHelper(ciData, nameof (GetWikiById)))
      {
        WikiV2 wikiV2 = ProjectPropertiesHelper.GetAllWikisFromProperties(requestContext, teamProjectId).Where<WikiV2>((Func<WikiV2, bool>) (w => w.Id.Equals(wikiId))).FirstOrDefault<WikiV2>();
        WikiV2 wiki = teamProjectId.Equals(Guid.Empty) ? wikiV2 : (wikiV2 == null || !wikiV2.ProjectId.Equals(teamProjectId) ? (WikiV2) null : wikiV2);
        WikiV2Helper.ThrowIfInsufficientUserWikiPermission(requestContext, wiki, (GitVersionDescriptor) null);
        return wiki;
      }
    }

    public static WikiV2 GetWikiByName(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string wikiName)
    {
      IEnumerable<WikiV2> source = !teamProjectId.Equals(Guid.Empty) ? ProjectPropertiesHelper.GetAllWikisFromProperties(requestContext, teamProjectId) : throw new InvalidArgumentValueException(nameof (teamProjectId), string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) nameof (teamProjectId)));
      if (wikiName == null)
        return (WikiV2) null;
      string trimmedWikiName = wikiName.Trim();
      WikiV2 wiki = source.Where<WikiV2>((Func<WikiV2, bool>) (w => w.Name.Trim().Equals(trimmedWikiName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<WikiV2>();
      WikiV2Helper.ThrowIfInsufficientUserWikiPermission(requestContext, wiki, (GitVersionDescriptor) null);
      return wiki;
    }

    public static WikiV2 GetProjectWiki(IVssRequestContext requestContext, Guid teamProjectId)
    {
      IEnumerable<WikiV2> allWikisInAproject = ProjectPropertiesHelper.GetAllWikisInAProject(requestContext, teamProjectId);
      WikiV2 wikiV2;
      if (allWikisInAproject == null)
      {
        wikiV2 = (WikiV2) null;
      }
      else
      {
        IEnumerable<WikiV2> source = allWikisInAproject.Where<WikiV2>((Func<WikiV2, bool>) (wiki => wiki.Type.Equals((object) WikiType.ProjectWiki)));
        wikiV2 = source != null ? source.FirstOrDefault<WikiV2>() : (WikiV2) null;
      }
      WikiV2 wiki1 = wikiV2;
      WikiV2Helper.ThrowIfInsufficientUserWikiPermission(requestContext, wiki1, (GitVersionDescriptor) null);
      return wiki1;
    }

    public static IEnumerable<WikiV2> GetAllProjectWikis(IVssRequestContext requestContext)
    {
      IEnumerable<WikiV2> wikisFromProperties = ProjectPropertiesHelper.GetAllWikisFromProperties(requestContext, Guid.Empty);
      return wikisFromProperties == null ? (IEnumerable<WikiV2>) null : wikisFromProperties.Where<WikiV2>((Func<WikiV2, bool>) (wiki => wiki.Type.Equals((object) WikiType.ProjectWiki) && WikiV2Helper.HasWikiReadPermissions(requestContext, wiki, wiki.ProjectId)));
    }

    public static WikiV2 GetDefaultProjectWiki(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid wikiId)
    {
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      try
      {
        using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, wikiId))
        {
          if (repositoryById == null)
            return (WikiV2) null;
          WikiV2 defaultProjectWiki = new WikiV2();
          defaultProjectWiki.Id = wikiId;
          defaultProjectWiki.Name = repositoryById.Name;
          defaultProjectWiki.ProjectId = teamProjectId;
          defaultProjectWiki.RepositoryId = wikiId;
          defaultProjectWiki.MappedPath = "/";
          defaultProjectWiki.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
          {
            new GitVersionDescriptor()
            {
              VersionType = GitVersionType.Branch,
              Version = repositoryById.Refs?.GetDefault()?.Name?.Substring("refs/heads/".Length)
            }
          };
          defaultProjectWiki.Type = WikiType.ProjectWiki;
          defaultProjectWiki.Properties = (IDictionary<string, string>) null;
          return defaultProjectWiki;
        }
      }
      catch (GitRepositoryNotFoundException ex)
      {
        return (WikiV2) null;
      }
    }

    public static string GetAvailableRepoNameForProjectWiki(
      IEnumerable<string> existingRepoNames,
      string projectName,
      Guid projectId)
    {
      string nameForProjectWiki1 = projectName + ".wiki";
      if (nameForProjectWiki1.Length <= 64 && !existingRepoNames.Contains<string>(nameForProjectWiki1))
        return nameForProjectWiki1;
      string nameForProjectWiki2 = projectName + "_wiki";
      if (nameForProjectWiki2.Length <= 64 && !existingRepoNames.Contains<string>(nameForProjectWiki2))
        return nameForProjectWiki2;
      Guid guid = projectId;
      string nameForProjectWiki3 = "wiki." + guid.ToString();
      if (!existingRepoNames.Contains<string>(nameForProjectWiki3))
        return nameForProjectWiki3;
      guid = Guid.NewGuid();
      return "wiki." + guid.ToString();
    }

    public static void SetWikiRepoPolicies(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repoId,
      bool shouldUpdate = false)
    {
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      string settings = "{\r\n    scope: [{ repositoryId: '" + repoId.ToString() + "' }],\r\n    maximumGitBlobSizeInBytes: " + 26214400.ToString() + "\r\n}";
      if (shouldUpdate)
      {
        IEnumerable<PolicyConfigurationRecord> configurationRecords = service.GetLatestPolicyConfigurationRecords(requestContext, teamProjectId, int.MaxValue, 1, out int? _);
        PolicyConfigurationRecord configurationRecord = configurationRecords != null ? configurationRecords.Where<PolicyConfigurationRecord>((Func<PolicyConfigurationRecord, bool>) (policy => policy.TypeId.Equals(WikiV2Helper.s_fileSizePolicyId))).LastOrDefault<PolicyConfigurationRecord>() : (PolicyConfigurationRecord) null;
        if (configurationRecord == null || configurationRecord.Settings == null || service.GetPolicyType(requestContext, WikiV2Helper.s_fileSizePolicyId)?.DeserializeSettings(configurationRecord.Settings) == null)
          return;
        service.UpdatePolicyConfiguration(requestContext, configurationRecord.ConfigurationId, WikiV2Helper.s_fileSizePolicyId, teamProjectId, configurationRecord.IsEnabled, configurationRecord.IsBlocking, false, settings);
      }
      else
        service.CreatePolicyConfiguration(requestContext, WikiV2Helper.s_fileSizePolicyId, teamProjectId, true, true, false, settings);
    }

    private static void ThrowIfInsufficientUserWikiPermission(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      GitRepositoryPermissions permission = GitRepositoryPermissions.GenericRead)
    {
      if (wiki == null)
        return;
      Guid projectId = wiki.ProjectId;
      try
      {
        WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, projectId, wiki.RepositoryId, wikiVersion, permission);
      }
      catch (Exception ex)
      {
        throw new WikiPermissionException(string.Format(Resources.WikiPermissionErrorMessage, (object) wiki.Name));
      }
    }

    private static WikiV2 CreateCodeWiki(
      IVssRequestContext requestContext,
      WikiCreateParametersV2 parameters)
    {
      string wikiName = !string.IsNullOrEmpty(parameters.Name) ? parameters.Name : throw new InvalidArgumentValueException("Name", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "Name"));
      WikiV2Helper.ValidateAndNormalizeWikiName(ref wikiName);
      if (WikiV2Helper.GetWikiByName(requestContext, parameters.ProjectId, wikiName) != null)
        throw new WikiAlreadyExistsException(string.Format(Resources.WikiAlreadyExistsWithName, (object) wikiName));
      Guid repositoryId = parameters.RepositoryId;
      if (parameters.RepositoryId == Guid.Empty)
        throw new InvalidArgumentValueException("RepositoryId", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "RepositoryId"));
      requestContext.GetService<IProjectService>().GetProject(requestContext, parameters.ProjectId);
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, parameters.RepositoryId))
      {
        if (repositoryById == null)
          throw new GitRepositoryNotFoundException(parameters.RepositoryId);
        if (parameters.Version == null)
          throw new InvalidArgumentValueException("Version", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "Version"));
        WikiV2Helper.ThrowIfInvalidWikiVersion(repositoryById, parameters.Version);
        WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, parameters.ProjectId, parameters.RepositoryId);
        parameters.MappedPath = !string.IsNullOrEmpty(parameters.MappedPath) ? PathHelper.TrimTrailingSeparatorInPath(parameters.MappedPath) : throw new InvalidArgumentValueException("MappedPath", string.Format(Resources.ParameterCannotBeNullOrEmpty, (object) "MappedPath"));
        WikiV2Helper.ThrowIfInvalidWikiMappedPath(requestContext, repositoryById, parameters.Version, parameters.MappedPath);
        WikiV2Helper.ThrowIfMappedPathAlreadyPublished(requestContext, parameters);
      }
      WikiV2 wikiV2 = new WikiV2();
      wikiV2.Name = wikiName;
      wikiV2.Type = WikiType.CodeWiki;
      wikiV2.Id = Guid.NewGuid();
      wikiV2.ProjectId = parameters.ProjectId;
      wikiV2.MappedPath = parameters.MappedPath;
      wikiV2.RepositoryId = parameters.RepositoryId;
      wikiV2.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
      {
        parameters.Version
      };
      WikiV2 wiki = wikiV2;
      ProjectPropertiesHelper.AddOrUpdateWikiToProperties(requestContext, wiki.ProjectId, wiki);
      return wiki;
    }

    private static WikiV2 CreateProjectWiki(
      IVssRequestContext requestContext,
      WikiCreateParametersV2 parameters)
    {
      Guid repositoryId = parameters.RepositoryId;
      if (parameters.RepositoryId != Guid.Empty)
        throw new InvalidArgumentValueException("RepositoryId", string.Format(Resources.ParameterNotExpectedForProjectWiki, (object) "RepositoryId"));
      if (parameters.MappedPath != null)
        throw new InvalidArgumentValueException("MappedPath", string.Format(Resources.ParameterNotExpectedForProjectWiki, (object) "MappedPath"));
      if (parameters.Version != null)
        throw new InvalidArgumentValueException("Version", string.Format(Resources.ParameterNotExpectedForProjectWiki, (object) "Version"));
      if (WikiV2Helper.GetWikiByName(requestContext, parameters.ProjectId, parameters.Name) != null)
        throw new WikiAlreadyExistsException(string.Format(Resources.WikiAlreadyExistsWithName, (object) parameters.Name));
      if (WikiV2Helper.GetProjectWiki(requestContext, parameters.ProjectId) != null)
      {
        requestContext.Trace(15250302, TraceLevel.Error, "Wiki", "Service", "Wiki already exists for project id: {0}", (object) parameters.ProjectId.ToString());
        throw new WikiAlreadyExistsException(string.Format(Resources.WikiAlreadyExistsInProject, (object) WikiV2Helper.GetProjectName(requestContext, parameters.ProjectId)));
      }
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      IList<TfsGitRepositoryInfo> source = service.QueryRepositories(requestContext);
      string wikiName = parameters.Name;
      if (string.IsNullOrWhiteSpace(wikiName))
        wikiName = WikiV2Helper.GetAvailableRepoNameForProjectWiki(source.Select<TfsGitRepositoryInfo, string>((Func<TfsGitRepositoryInfo, string>) (repoInfo => repoInfo.Name)), WikiV2Helper.GetProjectName(requestContext, parameters.ProjectId), parameters.ProjectId);
      WikiV2Helper.ValidateAndNormalizeWikiName(ref wikiName);
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, parameters.ProjectId);
      project.PopulateProperties(requestContext, "System.SourceControlGitEnabled", ProcessTemplateIdPropertyNames.ProcessTemplateType, "System.Wiki*");
      string repositoryName = wikiName.Replace(" ", "-");
      using (ITfsGitRepository repository = service.CreateRepository(requestContext, parameters.ProjectId, repositoryName, (IEnumerable<IAccessControlEntry>) null, (IEnumerable<IAccessControlEntry>) VersionControlProcessTemplateUtility.GetGitRepositoryPermissions(requestContext, project, true), true))
      {
        Guid repoId = repository.Key.RepoId;
        WikiV2 wikiV2 = new WikiV2();
        wikiV2.Id = repoId;
        wikiV2.Name = repository.Name;
        wikiV2.ProjectId = parameters.ProjectId;
        wikiV2.RepositoryId = repoId;
        wikiV2.MappedPath = "/";
        wikiV2.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
        {
          new GitVersionDescriptor()
          {
            VersionType = GitVersionType.Branch,
            Version = WikiConstants.WikiDefaultBranch
          }
        };
        wikiV2.Type = WikiType.ProjectWiki;
        wikiV2.Properties = (IDictionary<string, string>) null;
        WikiV2 wiki = wikiV2;
        repository.ModifyPaths("refs/heads/" + WikiConstants.WikiDefaultBranch, Sha1Id.Empty, Resources.WikiInitializationComment, (IEnumerable<GitChange>) WikiV2Helper.GetDefaultFilesChanges(), (GitUserDate) null, (GitUserDate) null);
        if (!WikiV2Helper.AddWikiToPropertiesIfNotSetAlready(requestContext, wiki))
        {
          requestContext.Trace(15250303, TraceLevel.Error, "Wiki", "Service", "Unable to write wiki repo setting as it already exists for project id: {0}, so deleting the repo id: {1}", (object) parameters.ProjectId.ToString(), (object) repoId.ToString());
          service.DeleteRepositories(requestContext.Elevate(), (RepoScope) repository.Key);
          throw new WikiAlreadyExistsException(string.Format(Resources.WikiAlreadyExistsInProject, (object) WikiV2Helper.GetProjectName(requestContext, parameters.ProjectId)));
        }
        WikiV2Helper.SetWikiRepoPolicies(requestContext, parameters.ProjectId, repoId);
        return wiki;
      }
    }

    private static bool AddWikiToPropertiesIfNotSetAlready(
      IVssRequestContext requestContext,
      WikiV2 wiki)
    {
      if (WikiV2Helper.GetProjectWiki(requestContext, wiki.ProjectId) != null)
        return false;
      ProjectPropertiesHelper.AddOrUpdateWikiToProperties(requestContext, wiki.ProjectId, wiki);
      return true;
    }

    private static List<GitChange> GetDefaultFilesChanges()
    {
      GitChange gitChange = new GitChange();
      gitChange.ChangeType = VersionControlChangeType.Add;
      GitItem gitItem = new GitItem();
      gitItem.Path = "/.gitignore";
      gitChange.Item = gitItem;
      GitChange templatedChange = gitChange;
      WikiV2Helper.CreateChangeContentFromTemplate(templatedChange);
      return new List<GitChange>() { templatedChange };
    }

    private static string GetProjectName(IVssRequestContext requestContext, Guid teamProjectId) => requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), teamProjectId);

    private static void CreateChangeContentFromTemplate(GitChange templatedChange)
    {
      byte[] bytes = Encoding.UTF8.GetBytes("");
      templatedChange.NewContent = new ItemContent()
      {
        Content = Convert.ToBase64String(bytes),
        ContentType = ItemContentType.Base64Encoded
      };
      templatedChange.NewContentTemplate = (GitTemplate) null;
    }

    public static bool CanUserAccessWiki(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IdentityDescriptor identityDescriptor)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      string securable = GitUtils.CalculateSecurable(wiki.ProjectId, wiki.RepositoryId, (string) null);
      IVssRequestContext requestContext1 = requestContext;
      string token = securable;
      EvaluationPrincipal evaluationPrincipal = (EvaluationPrincipal) identityDescriptor;
      return (2 & securityNamespace.QueryEffectivePermissions(requestContext1, token, evaluationPrincipal)) == 2;
    }

    public static void HasWikiPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      int pageId,
      GitRepositoryPermissions permission,
      out ISecuredObject wikiSecuredObject)
    {
      WikiV2 wiki;
      WikiPageIdDetails pageIdDetails = new WikiPageIdDetailsProvider().GetPageIdDetails(requestContext, projectId, pageId, out wiki);
      if (!WikiV2Helper.IsStakeholder(requestContext, projectId))
        WikiV2Helper.ThrowIfInsufficientUserWikiPermission(requestContext, wiki, pageIdDetails.WikiVersion, permission);
      wikiSecuredObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(wiki.ProjectId, wiki.RepositoryId);
    }

    public static bool IsStakeholder(IVssRequestContext requestContext, Guid projectId) => requestContext.IsStakeholder() && requestContext.GetService<IProjectService>().GetProjectVisibility(requestContext, projectId) != ProjectVisibility.Public;

    public static void ValidateWikiPushJobData(
      IVssRequestContext requestContext,
      WikiPushJobData jobData,
      TimedCiEvent ciEvent,
      out WikiV2 wiki,
      out string resultMessage,
      bool validateWiki = true)
    {
      ArgumentUtility.CheckForNull<WikiPushJobData>(jobData, nameof (jobData));
      ArgumentUtility.CheckForEmptyGuid(jobData.ProjectId, "ProjectId");
      ArgumentUtility.CheckForEmptyGuid(jobData.WikiId, "WikiId");
      ArgumentUtility.CheckForNull<GitVersionDescriptor>(jobData.WikiVersion, "WikiVersion");
      ArgumentUtility.CheckForNull<List<WikiPageChangeInfo>>(jobData.ChangedPages, "ChangedPages");
      if (jobData.PushId != -1)
        ArgumentUtility.CheckForNonPositiveInt(jobData.PushId, "PushId");
      if (jobData.WikiVersion.VersionType != GitVersionType.Branch)
      {
        requestContext.TraceErrorAlways(string.Format("Unknown version type detected:{0}", (object) jobData.WikiVersion), "Error_UnknownVersionType", ciEvent);
        resultMessage = string.Format("Unknown version type detected:{0}", (object) jobData.WikiVersion);
        throw new ArgumentException(resultMessage);
      }
      if (validateWiki)
      {
        wiki = WikiV2Helper.GetWikiById(requestContext, jobData.ProjectId, jobData.WikiId, ciEvent);
        if (wiki == null)
        {
          requestContext.TraceErrorAlways("Wiki not found", "Error_WikiNotFound", ciEvent);
          resultMessage = "Wiki not found";
          throw new WikiNotFoundException("Wiki not found");
        }
        resultMessage = "";
      }
      else
      {
        wiki = new WikiV2();
        wiki.Id = jobData.WikiId;
        wiki.ProjectId = jobData.ProjectId;
        resultMessage = "";
      }
    }

    private static bool HasWikiReadPermissions(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      Guid teamProjectId)
    {
      return WikiV2Helper.HasWikiPermissions(requestContext, wiki, teamProjectId, GitRepositoryPermissions.GenericRead);
    }

    private static bool HasWikiPermissions(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      Guid teamProjectId,
      GitRepositoryPermissions permission)
    {
      string securable = GitUtils.CalculateSecurable(teamProjectId, wiki.RepositoryId, (string) null);
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId).HasPermission(requestContext, securable, (int) permission);
    }

    private static void ThrowIfInvalidWikiVersion(
      ITfsGitRepository repository,
      GitVersionDescriptor version)
    {
      Sha1Id? nullable = new Sha1Id?();
      if (version.VersionType == GitVersionType.Branch)
        nullable = GitVersionParser.GetBranchObjectId(repository, version.Version);
      else if (version.VersionType == GitVersionType.Tag)
        nullable = GitVersionParser.GetTagReferenceId(repository, version.Version);
      if (!nullable.HasValue)
        throw new InvalidArgumentValueException(nameof (version), string.Format(Resources.WikiVersionInvalidOrDoesNotExist, (object) version));
    }

    private static void ThrowIfInvalidWikiMappedPath(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor version,
      string mappedPath)
    {
      GitItem gitItem = (GitItem) null;
      try
      {
        gitItem = GitItemUtility.RetrieveItemModel(requestContext, (UrlHelper) null, repository, new GitItemDescriptor()
        {
          Path = mappedPath,
          RecursionLevel = VersionControlRecursionType.None,
          VersionType = version.VersionType,
          Version = version.Version
        });
      }
      catch (GitItemNotFoundException ex)
      {
      }
      finally
      {
        if (gitItem == null || !gitItem.IsFolder || mappedPath != "/" && mappedPath.Length > 0 && mappedPath[mappedPath.Length - 1] == '/')
          throw new InvalidArgumentValueException(nameof (mappedPath), string.Format(Resources.WikiMappedPathInvalidOrDoesNotExist, (object) mappedPath, (object) version.Version));
      }
    }

    private static void ThrowIfMappedPathAlreadyPublished(
      IVssRequestContext requestContext,
      WikiCreateParametersV2 parameters)
    {
      foreach (WikiV2 wiki in WikiV2Helper.GetWikis(requestContext, parameters.ProjectId))
      {
        if (!(wiki.RepositoryId != parameters.RepositoryId) && wiki.MappedPath.Equals(parameters.MappedPath))
          throw new WikiOperationNotSupportedException(string.Format(Resources.WikiPublishOperationPathAlreadyPublished, (object) wiki.MappedPath, (object) wiki.Name));
      }
    }

    private static void ThrowIfInsufficientPermissionToRenamewiki(
      IVssRequestContext requestContext,
      WikiV2 wiki)
    {
      if (wiki.Type == WikiType.ProjectWiki)
        WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, wiki.ProjectId, wiki.RepositoryId, permission: GitRepositoryPermissions.RenameRepository);
      else
        WikiV2Helper.ThrowIfInsufficientUserRepositoryPermission(requestContext, wiki.ProjectId, wiki.RepositoryId);
    }

    private static void ThrowIfInsufficientUserRepositoryPermission(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repositoryId,
      GitVersionDescriptor version = null,
      GitRepositoryPermissions permission = GitRepositoryPermissions.GenericContribute)
    {
      string refName = (string) null;
      if (version != null)
        refName = version.VersionType == GitVersionType.Branch ? "refs/heads/" + version.Version : "refs/tags/" + version.Version;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GitConstants.GitSecurityNamespaceId);
      string securable = GitUtils.CalculateSecurable(teamProjectId, repositoryId, refName);
      try
      {
        securityNamespace.CheckPermission(requestContext, securable, (int) permission, false);
      }
      catch (AccessCheckException ex)
      {
        throw new WikiPermissionException(version == null ? string.Format(Resources.ErrorMessageInsufficientRepositoryPermission, (object) permission.ToString()) : Resources.ErrorMessageNoVersionContributePermission);
      }
    }

    private static void ValidateAndNormalizeWikiName(ref string wikiName)
    {
      try
      {
        wikiName = ProjectInfo.NormalizeProjectName(wikiName, nameof (wikiName), true, true);
        if (!CssUtils.IsValidProjectName(wikiName))
          throw new WikiInvalidNameException(wikiName);
      }
      catch (InvalidProjectNameException ex)
      {
        throw new WikiInvalidNameException(wikiName);
      }
    }
  }
}
