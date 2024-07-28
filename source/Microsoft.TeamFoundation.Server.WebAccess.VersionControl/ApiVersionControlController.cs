// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.ApiVersionControlController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.VersionControl.Server.Services;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiVersionControlController : VersionControlAreaController
  {
    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext.Exception is ItemNotFoundException | filterContext.Exception is GitRepositoryNotFoundException)
        throw new HttpException(404, filterContext.Exception.Message);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511160, 511170)]
    [OutputCache(CacheProfile = "Default", Duration = 0)]
    public FileResult ItemContent(
      Guid? repositoryId,
      string path,
      string version,
      bool? contentOnly)
    {
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      VersionControlProvider vcProvider = this.GetVcProvider(repositoryId);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file = vcProvider.GetItem(path, version, true);
      if (file.IsFolder)
        throw new ArgumentException(VCServerResources.InvalidItemType).Expected(this.TfsRequestContext.ServiceName);
      string contentType = file.ContentMetadata.ContentType;
      string responseContentType = MediaTypeFormatUtility.GetSafeResponseContentType(string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType);
      string fileNameFromPath = !contentOnly.GetValueOrDefault(false) || responseContentType == "application/octet-stream" ? vcProvider.GetFileNameFromPath(path) : (string) null;
      this.Response.BufferOutput = false;
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return (FileResult) this.File(vcProvider.GetFileContentStream(file), responseContentType, fileNameFromPath);
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(511171, 511179)]
    [TfsBypassAntiForgeryValidation]
    [ValidateInput(false)]
    public ActionResult ItemContentJson(
      Guid? repositoryId,
      string path,
      string version,
      long? maxContentLength,
      bool? splitContentIntoLines,
      bool? includeBinaryContent)
    {
      if (!maxContentLength.HasValue || maxContentLength.Value <= 0L)
        maxContentLength = new long?(VersionControlSettings.ReadMaxFileSize(this.TfsRequestContext.Elevate()));
      VersionControlProvider vcProvider = this.GetVcProvider(repositoryId);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel itemModel = vcProvider.GetItem(path, version, true, maxContentLength.Value);
      if (itemModel.IsFolder)
      {
        DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) new FileContent()
        {
          Content = (string) null
        });
        contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) contractJsonResult;
      }
      vcProvider.GetFileNameFromPath(path);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      FileContent fileContent = FileContentHelper.GetFileContent(vcProvider, itemModel, maxContentLength, splitContentIntoLines, includeBinaryContent);
      if (!string.IsNullOrEmpty(version) && version.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
      {
        this.Response.Cache.SetCacheability(HttpCacheability.Private);
        this.Response.Cache.SetMaxAge(TimeSpan.FromDays(3.0));
        this.Response.Cache.SetExpires(DateTime.Now.AddDays(3.0));
      }
      else
      {
        this.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1.0));
        this.Response.Cache.SetValidUntilExpires(false);
        this.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        this.Response.Cache.SetNoStore();
      }
      DataContractJsonResult contractJsonResult1 = new DataContractJsonResult((object) fileContent);
      contractJsonResult1.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult1;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(511171, 511179)]
    [TfsBypassAntiForgeryValidation]
    [OutputCache(CacheProfile = "Default", Duration = 0)]
    public ActionResult ItemContentZipped(
      Guid? repositoryId,
      string path,
      string version,
      string zipFileName)
    {
      VersionControlProvider vcProvider = this.GetVcProvider(repositoryId);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel rootItem = vcProvider.GetItem(path, version, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions()
      {
        RecursionLevel = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.Full
      });
      if (string.IsNullOrEmpty(zipFileName))
      {
        zipFileName = vcProvider.GetFileNameFromPath(rootItem.ServerItem);
        if (string.IsNullOrEmpty(zipFileName))
          zipFileName = FileSpec.RemoveInvalidFileNameChars(vcProvider.RepositoryName);
        zipFileName += ".zip";
      }
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      ZipFileStreamResult fileStreamResult = new ZipFileStreamResult(vcProvider.GetZipFileEntries(rootItem));
      fileStreamResult.FileDownloadName = zipFileName;
      return (ActionResult) fileStreamResult;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(511100, 511110)]
    [TfsBypassAntiForgeryValidation]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult History(Guid? repositoryId, [ModelBinder(typeof (JsonModelBinder))] ChangeListSearchCriteria searchCriteria)
    {
      ArgumentUtility.CheckForNull<ChangeListSearchCriteria>(searchCriteria, nameof (searchCriteria));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetVcProvider(repositoryId).QueryHistory(searchCriteria));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(511180, 511190)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult AssociatedWorkItems(Guid? repositoryId, string[] versions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) versions, "version");
      List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem> data = new List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem>();
      IEnumerable<int> linkedWorkItemIds = this.GetVcProvider(repositoryId).GetLinkedWorkItemIds(versions);
      if (linkedWorkItemIds.Any<int>())
      {
        string[] fields = new string[5]
        {
          "System.Id",
          "System.WorkItemType",
          "System.Title",
          "System.State",
          "System.AssignedTo"
        };
        data = new QueryResultPayload(this.TfsRequestContext, this.TfsRequestContext.GetService<WebAccessWorkItemService>().PageWorkItems(this.TfsRequestContext, linkedWorkItemIds, (IEnumerable<string>) fields), true).Rows.Select<object[], Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem>((Func<object[], Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem>) (row => new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem()
        {
          Id = (int) row[0],
          WorkItemType = (string) row[1],
          Title = (string) row[2],
          State = (string) row[3],
          AssignedTo = (string) row[4]
        })).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.AssociatedWorkItem>();
      }
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) data);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [TfsTraceFilter(511420, 511430)]
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Change(Guid? repositoryId, string version, int maxChanges)
    {
      Guid? nullable = repositoryId;
      Guid empty = Guid.Empty;
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0 ? (object) this.GetGitVcProviderById(repositoryId.Value).GetChangeList(version, maxChanges) : (object) this.GetTfsVcProvider().GetChangeList(version, maxChanges));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [TfsTraceFilter(511420, 511430)]
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult ChangeListChanges(
      Guid? repositoryId,
      string version,
      int maxChanges,
      int skipCount)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetVcProvider(repositoryId).GetChangeListChanges(version, maxChanges, skipCount));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(51280, 512090)]
    [TfsBypassAntiForgeryValidation]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult FileDiff(Guid? repositoryId, [ModelBinder(typeof (JsonModelBinder))] FileDiffParameters diffParameters)
    {
      ArgumentUtility.CheckForNull<FileDiffParameters>(diffParameters, nameof (diffParameters), "git");
      if (string.IsNullOrEmpty(diffParameters.OriginalPath))
        ArgumentUtility.CheckStringForNullOrEmpty(diffParameters.ModifiedPath, "diffParameters.modifiedPath");
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetVcProvider(repositoryId).GetFileDiffModel(diffParameters));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(51380, 513090)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Authors(Guid? repositoryId)
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetVcProvider(repositoryId).GetAuthors());
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(51510, 514520)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult UserPreferences()
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) VersionControlUserPreferences.GetUserPreferences(this.TfsWebContext));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(51510, 514520)]
    [OutputCache(CacheProfile = "NoCache")]
    [ValidateInput(false)]
    public ActionResult UpdateUserPreferences([ModelBinder(typeof (JsonModelBinder))] VersionControlUserPreferences preferences)
    {
      VersionControlUserPreferences.SetUserPreferences(this.TfsWebContext, preferences);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511350, 511360)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult AllGitRepositoriesOptions()
    {
      ProjectInfo project = this.TfsRequestContext.GetService<IRequestProjectService>().GetProject(this.TfsRequestContext);
      List<VersionControlRepositoryOption> data = new List<VersionControlRepositoryOption>();
      if (project == null)
        data.Add(new VersionControlRepositoryOption()
        {
          Key = "GravatarEnabled",
          Html = VCResources.GravatarEnabledNoLink,
          Value = VersionControlSettings.ReadGravatarEnabled(this.TfsRequestContext),
          Category = VCResources.GeneralOptionsGroup,
          Title = VCResources.GravatarOptionTitle
        });
      string str1 = this.TfsRequestContext.IsFeatureEnabled("Git.DefaultBranchIsMain") ? "main" : "master";
      string str2;
      if (project != null)
      {
        str2 = VersionControlSettings.ReadDefaultBranchName(this.TfsRequestContext, new Guid?(project.Id));
        string str3 = VersionControlSettings.ReadDefaultBranchName(this.TfsRequestContext, new Guid?());
        if (!string.IsNullOrEmpty(str3))
          str1 = str3;
      }
      else
        str2 = VersionControlSettings.ReadDefaultBranchName(this.TfsRequestContext, new Guid?());
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "DefaultBranchName",
        Html = VCResources.DefaultBranchNameNoLink,
        Value = !string.IsNullOrEmpty(str2),
        DefaultTextValue = str1,
        TextValue = str2,
        Category = VCResources.GeneralOptionsGroup,
        Title = VCResources.DefaultBranchNameTitle
      });
      bool flag = this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, "WebAccess.VersionControl.DisableTfvcRepository");
      if (project == null & flag)
        data.Add(this.CreateTfvcDisableRepositoryRepositoryOption());
      if (project != null)
      {
        data.Add(new VersionControlRepositoryOption()
        {
          Key = "NewReposCreatedBranchesManagePermissionsEnabled",
          Html = VCResources.ResourceManager.GetString("NewReposCreatedBranchesManagePermissionsEnabledOption"),
          Value = VersionControlSettings.ReadNewReposCreatedBranchesManagePermissionsEnabled(this.TfsRequestContext, project.Id),
          Category = VCResources.GeneralOptionsGroup,
          Title = VCResources.NewReposCreatedBranchesManagePermissionsEnabledTitle
        });
        data.Add(new VersionControlRepositoryOption()
        {
          Key = "PullRequestAsDraftByDefault",
          Html = VCResources.ResourceManager.GetString("PullRequestAsDraftByDefaultOption"),
          Value = VersionControlSettings.ReadPullRequestAsDraftByDefault(this.TfsRequestContext, project.Id),
          Category = VCResources.GeneralOptionsGroup,
          Title = VCResources.PullRequestAsDraftByDefaultTitle
        });
        if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving") && this.TfsRequestContext.IsFeatureEnabled("SourceControl.ShowToggleToSwitchOldCheckinPoliciesSaving"))
          data.Add(new VersionControlRepositoryOption()
          {
            Key = "DisableOldTfvcCheckinPolicies",
            Html = VCResources.DisableOldTfvcCheckinPoliciesDescription,
            Value = VersionControlSettingService.ReadDisableOldTfvcCheckinPolicies(this.TfsRequestContext, project.Id),
            Category = VCResources.GeneralOptionsGroup,
            Title = VCResources.DisableOldTfvcCheckinPoliciesTitle
          });
      }
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) data);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    private VersionControlRepositoryOption CreateTfvcDisableRepositoryRepositoryOption() => new VersionControlRepositoryOption()
    {
      Key = "DsableTfvcRepositories",
      Html = VCResources.TfvcRepositoriesDisabledDescription,
      Value = VersionControlSettings.ReadDisableTfvcRepositories(this.TfsRequestContext),
      Category = VCResources.GeneralOptionsGroup,
      Title = VCResources.TfvcRepositoriesDisabledTitle
    };

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511350, 511360)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult RepositoryOptions(Guid repositoryId)
    {
      List<VersionControlRepositoryOption> data = new List<VersionControlRepositoryOption>();
      bool? nullable = VersionControlSettings.ReadForksEnabled(this.TfsRequestContext, repositoryId);
      if (nullable.HasValue)
        data.Add(new VersionControlRepositoryOption()
        {
          Key = "ForksEnabled",
          Html = VCResources.ResourceManager.GetString("ForksEnabledOption"),
          Value = nullable.Value,
          Category = VCResources.ForksOptionsGroup,
          Title = VCResources.ForksOptionsGroup
        });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "WitMentionsEnabled",
        Html = VCResources.ResourceManager.GetString("WitMentionsEnabledOption"),
        Value = VersionControlSettings.ReadWitMentionsEnabled(this.TfsRequestContext, repositoryId),
        Category = VCResources.WorkItemOptionsGroup,
        Title = VCResources.MentionCommitLinkingOptionTitle
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "WitResolutionMentionsEnabled",
        Html = string.Format(VCResources.ResourceManager.GetString("WitResolutionMentionsEnabledOption"), (object) VCResources.FixesKeywordExample),
        Value = VersionControlSettings.ReadWitResolutionMentionsEnabled(this.TfsRequestContext, repositoryId),
        Category = VCResources.WorkItemOptionsGroup,
        Title = VCResources.MentionCommitResolutionOptionTitle
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "WitTransitionsSticky",
        Html = VCResources.ResourceManager.GetString("WitTransitionsStickyOption"),
        Value = VersionControlSettings.ReadWitTransitionsSticky(this.TfsRequestContext, repositoryId),
        Category = VCResources.WorkItemOptionsGroup,
        Title = VCResources.TransitionStickyOptionTitle
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "IsDisabled",
        Html = VCResources.ResourceManager.GetString("IsDisabledOption"),
        Value = VersionControlSettings.ReadDisabledRepo(this.TfsRequestContext, repositoryId),
        Category = VCResources.IsDisabled,
        Title = VCResources.IsDisabledOptionTitle
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "RepoCreatedBranchesManagePermissionsEnabled",
        Html = VCResources.ResourceManager.GetString("RepoCreatedBranchesManagePermissionsEnabledOption"),
        Value = VersionControlSettings.ReadRepoCreatedBranchesManagePermissionsEnabled(this.TfsRequestContext, repositoryId),
        Category = VCResources.RepoCreatedBranchesManagePermissionsEnabled,
        Title = VCResources.RepoCreatedBranchesManagePermissionsEnabledTitle
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "StrictVoteMode",
        Html = VCResources.ResourceManager.GetString("StrictVoteModeOption"),
        Value = VersionControlSettings.ReadStrictVoteMode(this.TfsRequestContext, repositoryId),
        Category = VCResources.StrictVoteModeGroup,
        Title = VCResources.StrictVoteModeGroup
      });
      ProjectInfo project = this.TfsRequestContext.GetService<IRequestProjectService>().GetProject(this.TfsRequestContext);
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "InheritPullRequestCreationMode",
        Html = VCResources.ResourceManager.GetString("InheritPullRequestCreationModeOption"),
        Value = VersionControlSettings.ReadInheritPullRequestCreationMode(this.TfsRequestContext, repositoryId),
        Category = VCResources.InheritPullRequestCreationModeGroup,
        Title = VCResources.InheritPullRequestCreationModeGroup
      });
      data.Add(new VersionControlRepositoryOption()
      {
        Key = "RepoPullRequestAsDraftByDefault",
        Html = VCResources.ResourceManager.GetString("RepoPullRequestAsDraftByDefaultOption"),
        Value = VersionControlSettings.ReadInheritPullRequestCreationMode(this.TfsRequestContext, repositoryId) ? VersionControlSettings.ReadPullRequestAsDraftByDefault(this.TfsRequestContext, project.Id) : VersionControlSettings.ReadRepoPullRequestAsDraftByDefault(this.TfsRequestContext, repositoryId),
        Category = VCResources.RepoPullRequestAsDraftByDefaultGroup,
        Title = VCResources.RepoPullRequestAsDraftByDefaultGroup,
        ParentOptionKey = "InheritPullRequestCreationMode"
      });
      if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.SuggestBranchName"))
        data.Add(new VersionControlRepositoryOption()
        {
          Key = "Suggestion",
          Html = VCResources.ResourceManager.GetString("SuggestionOption"),
          TextValue = VersionControlSettings.ReadSuggestion(this.TfsRequestContext, repositoryId),
          Category = VCResources.SuggestionForBranchName,
          Title = VCResources.SuggestionForBranchName
        });
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) data);
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(511350, 511360)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult UpdateRepositoryOption(
      Guid repositoryId,
      [ModelBinder(typeof (JsonModelBinder))] VersionControlRepositoryOption option)
    {
      if (option != null)
      {
        ProjectInfo project = this.TfsRequestContext.GetService<IRequestProjectService>().GetProject(this.TfsRequestContext);
        if (option.Key.Equals("GravatarEnabled", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateGravatarEnabled(this.TfsRequestContext, option.Value);
        else if (option.Key.Equals("DefaultBranchName", StringComparison.OrdinalIgnoreCase))
        {
          if (option.Value && !RefUtil.IsValidRefName("refs/heads/" + option.TextValue, true))
            return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, VCServerResources.InvalidBranchName);
          VersionControlSettings.UpdateDefaultBranchName(this.TfsRequestContext, project?.Id, option.Value ? option.TextValue : (string) null);
        }
        else if (ApiVersionControlController.IsOptionNameEquals(option, "DsableTfvcRepositories"))
          VersionControlSettings.UpdateDisableTfvcRepositories(this.TfsRequestContext, option.Value);
        else if (option.Key.Equals("WitMentionsEnabled", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateWitMentionsEnabled(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("WitResolutionMentionsEnabled", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateWitResolutionMentionsEnabled(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("ForksEnabled", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdatedForksEnable(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("WitTransitionsSticky", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateWitTransitionsSticky(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("IsDisabled", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            VersionControlSettings.UpdateDisabledState(this.TfsRequestContext, repositoryId, option.Value);
          }
          catch (Exception ex)
          {
            if (ex is GitRepositoryNameStateConstrainException)
              return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.Conflict, ex.Message);
            throw;
          }
        }
        else if (option.Key.Equals("RepoCreatedBranchesManagePermissionsEnabled", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateRepoCreatedBranchesManagePermissionsEnabled(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("NewReposCreatedBranchesManagePermissionsEnabled", StringComparison.OrdinalIgnoreCase) && project != null)
          VersionControlSettings.UpdateNewReposCreatedBranchesManagePermissionsEnabled(this.TfsRequestContext, project.Id, option.Value);
        else if (option.Key.Equals("StrictVoteMode", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateStrictVoteMode(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("InheritPullRequestCreationMode", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateInheritPullRequestCreationMode(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("RepoPullRequestAsDraftByDefault", StringComparison.OrdinalIgnoreCase))
          VersionControlSettings.UpdateRepoPullRequestAsDraftByDefault(this.TfsRequestContext, repositoryId, option.Value);
        else if (option.Key.Equals("PullRequestAsDraftByDefault", StringComparison.OrdinalIgnoreCase) && project != null)
          VersionControlSettings.UpdatePullRequestAsDraftByDefault(this.TfsRequestContext, project.Id, option.Value);
        else if (option.Key.Equals("Suggestion", StringComparison.OrdinalIgnoreCase))
        {
          if (option.Value && option.TextValue != "" && !RefUtil.IsValidRefName("refs/heads/" + option.TextValue, true))
            return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.BadRequest, VCServerResources.InvalidBranchName);
          VersionControlSettings.UpdateSuggestion(this.TfsRequestContext, repositoryId, option.TextValue);
        }
        else if (option.Key.Equals("DisableOldTfvcCheckinPolicies", StringComparison.OrdinalIgnoreCase) && project != null && this.TfsRequestContext.IsFeatureEnabled("SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving") && this.TfsRequestContext.IsFeatureEnabled("SourceControl.ShowToggleToSwitchOldCheckinPoliciesSaving"))
          VersionControlSettingService.UpdateDisableOldTfvcCheckinPolicies(this.TfsRequestContext, project.Id, option.Value);
      }
      return (ActionResult) new EmptyResult();
    }

    private static bool IsOptionNameEquals(VersionControlRepositoryOption option, string name) => option.Key.Equals(name, StringComparison.OrdinalIgnoreCase);

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GitUserDefaultBranchName(Guid repositoryId) => (ActionResult) new RestApiJsonResult((object) new
    {
      branchName = this.GetGitVcProviderById(repositoryId).GetDefaultBranchName(true)
    });

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    [TfsTraceFilter(511370, 511380)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult SetGitDefaultRef(Guid repositoryId, string refName)
    {
      this.GetGitVcProviderById(repositoryId).Repository.Refs.SetDefault(refName);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    [TfsTraceFilter(511390, 511400)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult LockGitRef(Guid repositoryId, string refName)
    {
      this.GetGitVcProviderById(repositoryId).Repository.Refs.Lock(refName);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    [TfsTraceFilter(511440, 511450)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult UnlockGitRef(Guid repositoryId, string refName)
    {
      this.GetGitVcProviderById(repositoryId).Repository.Refs.Unlock(refName);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GitUserDefaultRepository(string projectName)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ProjectInfo projectInfo = TfsProjectHelpers.GetProjectFromName(this.TfsRequestContext, projectName);
      return (ActionResult) new RestApiJsonResult((object) new
      {
        repository = VersionControlRepositoryInfoFactory.GetUserDefaultGitRepository(this.TfsRequestContext, this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(this.TfsRequestContext, projectInfo.Uri, true).Select<TfsGitRepositoryInfo, GitRepository>((Func<TfsGitRepositoryInfo, GitRepository>) (x => x.ToWebApiItem(this.TfsRequestContext, projectInfo))), projectInfo.Id, projectInfo.Name)
      });
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsBypassAntiForgeryValidation]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult CommitItems(Guid repositoryId, string version, string path)
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetGitVcProviderById(repositoryId).GetCommitItems(version, path));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsBypassAntiForgeryValidation]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult LastChangeTreeItems(
      Guid repositoryId,
      string version,
      string path,
      bool allowPartial,
      bool? includeCommits)
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetGitVcProviderById(repositoryId).GetLastChangeTreeItems(version, path, allowPartial, includeCommits.HasValue && includeCommits.Value));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(511060, 511070)]
    [TfsBypassAntiForgeryValidation]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult AnnotateGitDiffs(Guid repositoryId, string path, [ModelBinder(typeof (JsonModelBinder))] string[] versions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) versions, nameof (versions));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetGitVcProviderById(repositoryId).GetAnnotateDiffModels(path, versions));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult BranchDiffSummary(Guid repositoryId, string baseVersion, [ModelBinder(typeof (JsonModelBinder))] string[] versions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(baseVersion, nameof (baseVersion));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) versions, nameof (versions));
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetGitVcProviderById(repositoryId).GetBranchDiffModels(baseVersion, (IEnumerable<string>) versions));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [HttpGet]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult DiffCommits(
      Guid repositoryId,
      string baseVersion,
      string targetVersion,
      int maxNumberOfChanges,
      int skipCount)
    {
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetGitVcProviderById(repositoryId).DiffCommits(baseVersion, targetVersion, maxNumberOfChanges, skipCount));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [HttpGet]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Changesets(string path, string version)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      TfsVersionControlProvider tfsVcProvider = this.GetTfsVcProvider();
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) tfsVcProvider.GetChangesetsByIds(tfsVcProvider.GetItem(path, version, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions()
      {
        RecursionLevel = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.OneLevel
      }).ChildItems.Cast<TfsItem>().Select<TfsItem, int>((Func<TfsItem, int>) (x => x.ChangesetVersion)).ToArray<int>()));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511080, 511090)]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Shelvesets(string owner)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(owner, nameof (owner));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetTfsVcProvider().GetShelvesets(string.Empty, owner, 0).OrderByDescending<TfsChangeList, DateTime>((Func<TfsChangeList, DateTime>) (ss => ss.CreationDate)));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(511040, 511050)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult ItemsById(string ids, int? changeset)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(ids, nameof (ids));
      int[] array = ((IEnumerable<string>) ids.Split(new char[1]
      {
        ','
      }, StringSplitOptions.None)).Select<string, int>((Func<string, int>) (idString =>
      {
        int result;
        if (int.TryParse(idString, out result))
          return result;
        throw new Microsoft.TeamFoundation.Server.WebAccess.InvalidArgumentValueException(nameof (ids));
      })).ToArray<int>();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) array, nameof (ids));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetTfsVcProvider().GetItemsById(array, changeset.GetValueOrDefault(0), false));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    [TfsTraceFilter(511080, 511090)]
    [TfsBypassAntiForgeryValidation]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult AnnotateTfsDiffs([ModelBinder(typeof (JsonModelBinder))] TfsAnnotateDiffParameters[] annotateDiffParameters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) annotateDiffParameters, nameof (annotateDiffParameters));
      DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) this.GetTfsVcProvider().GetAnnotateDiffModels(annotateDiffParameters));
      contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) contractJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(511300, 511310)]
    [ValidateInput(false)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult UpdateChangeset(int? changeset, string comment, string notes)
    {
      ArgumentUtility.CheckForNull<int>(changeset, nameof (changeset));
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote[] notes1 = Microsoft.TeamFoundation.Server.WebAccess.ConvertUtility.FromDataContractJson<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote[]>(notes);
      this.GetTfsVcProvider().UpdateChangeset(changeset.Value, comment, (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.CheckinNote>) notes1);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsTraceFilter(511321, 511330)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult CreateProjectFolder(string projectUri)
    {
      ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
      this.GetTfsVcProvider().CreateProjectFolder(projectUri);
      return (ActionResult) new EmptyResult();
    }
  }
}
