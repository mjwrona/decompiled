// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitRefService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TeamFoundationGitRefService : 
    ITeamFoundationGitRefService,
    IVssFrameworkService,
    IInternalGitRefService
  {
    private const string c_layer = "TeamFoundationGitRefService";

    public void ServiceStart(IVssRequestContext systemRc)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRc)
    {
    }

    public TfsGitRefUpdateResultSet UpdateRefs(
      IVssRequestContext rc,
      Guid repoId,
      List<TfsGitRefUpdateRequest> refUpdates,
      GitRefUpdateMode updateMode = GitRefUpdateMode.BestEffort,
      IGitPushReporter pushReporter = null,
      ClientTraceData ctData = null,
      ITeamFoundationGitRefUpdateValidator refUpdateValidator = null)
    {
      return this.UpdateRefs(rc, repoId, refUpdates, new List<TfsIncludedGitCommit>(), updateMode, pushReporter, ctData, refUpdateValidator ?? (ITeamFoundationGitRefUpdateValidator) new DefaultGitRefUpdateValidator(), (QueuedGitPushJobsContext) null);
    }

    public Guid ReadRefCreatorWithDefaultAce(
      IVssRequestContext rc,
      RepoKey repoKey,
      string refName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        return gitCoreComponent.ReadRefCreatorWithDefaultAce(repoKey, refName);
    }

    public string ReadAnyRefWithDefaultAceByCreator(
      IVssRequestContext rc,
      RepoKey repoKey,
      Guid creatorId)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        return gitCoreComponent.ReadAnyRefWithDefaultAceByCreator(repoKey, creatorId);
    }

    public void SetAccessControlListForRefCreator(IVssRequestContext rc, string token)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      if (!token.EndsWith("/"))
        token += "/";
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.SetAccessControlListForRefCreator(token);
    }

    TfsGitRefUpdateResultSet IInternalGitRefService.UpdateRefs(
      IVssRequestContext rc,
      Guid repoId,
      List<TfsGitRefUpdateRequest> refUpdates,
      List<TfsIncludedGitCommit> includedCommits,
      GitRefUpdateMode updateMode,
      IGitPushReporter pushReporter,
      ClientTraceData ctData,
      ITeamFoundationGitRefUpdateValidator refUpdateValidator,
      QueuedGitPushJobsContext queuedGitPushJobsContext)
    {
      return this.UpdateRefs(rc, repoId, refUpdates, includedCommits, updateMode, pushReporter, ctData, refUpdateValidator ?? (ITeamFoundationGitRefUpdateValidator) new DefaultGitRefUpdateValidator(), queuedGitPushJobsContext);
    }

    private TfsGitRefUpdateResultSet UpdateRefs(
      IVssRequestContext rc,
      Guid repoId,
      List<TfsGitRefUpdateRequest> refUpdates,
      List<TfsIncludedGitCommit> includedCommits = null,
      GitRefUpdateMode updateMode = GitRefUpdateMode.BestEffort,
      IGitPushReporter pushReporter = null,
      ClientTraceData ctData = null,
      ITeamFoundationGitRefUpdateValidator refUpdateValidator = null,
      QueuedGitPushJobsContext queuedGitPushJobsContext = null)
    {
      using (rc.TraceBlock(1013107, 1013108, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), nameof (UpdateRefs)))
      {
        refUpdates.ForEach((Action<TfsGitRefUpdateRequest>) (refToUpdate =>
        {
          if (refToUpdate.OldObjectId.IsEmpty && refToUpdate.Name != null && !TeamFoundationGitRefService.IsBranchNameValid(refToUpdate.Name))
            throw new GitArgumentException(Resources.Format("GitFortyHexCharsRefName"));
        }));
        using (TeamFoundationGitRefService.UpdateRefsContext urContext = new TeamFoundationGitRefService.UpdateRefsContext(rc, rc.GetService<IProjectService>(), repoId, refUpdateValidator ?? (ITeamFoundationGitRefUpdateValidator) new DefaultGitRefUpdateValidator(), refUpdates, includedCommits, updateMode, pushReporter, ctData))
        {
          urContext.AddCTData("RefsAttempted", urContext.RefUpdates.Count);
          TeamFoundationGitRefService.FilterRefs(rc, urContext, includedCommits != null && includedCommits.Any<TfsIncludedGitCommit>());
          if (urContext.AnyFailures && (urContext.UpdateMode == GitRefUpdateMode.AllOrNone || urContext.RefUpdates.Count == 0))
            return urContext.ResultSet;
          this.PerformConsistencyCheck(rc, urContext);
          TeamFoundationGitRefService.ApplyRefUpdates(rc, urContext);
          if ((urContext.UpdateMode == GitRefUpdateMode.BestEffort || !urContext.AnyFailures) && TeamFoundationGitRefService.PublishNotifications(rc, urContext, queuedGitPushJobsContext))
            urContext.AddCTData("RefsSucceeded", urContext.ResultSet.CountSucceeded);
          return urContext.ResultSet;
        }
      }
    }

    private void PerformConsistencyCheck(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext)
    {
      GitFsckMode fsckMode = GitFsckMode.None;
      if (!rc.IsFeatureEnabled("Git.DisableRefUpdateBlockUnsafeSubmodule"))
        fsckMode |= GitFsckMode.Submodules;
      if (!rc.IsFeatureEnabled("Git.DisableRefUpdateBlockDotGitAlternateDataStreamCheck"))
        fsckMode |= GitFsckMode.AlternateDataStreams;
      if (rc.IsFeatureEnabled("Git.EnableRefUpdateWarnItemPathBackslash"))
        fsckMode |= GitFsckMode.ItemPathBackslash;
      if (fsckMode == GitFsckMode.None)
        return;
      foreach (TfsGitRefUpdateRequest refUpdate in urContext.RefUpdates)
      {
        TfsGitObject gitObject = urContext.Repo.TryLookupObject(refUpdate.NewObjectId);
        TfsGitCommit commit = gitObject != null ? gitObject.TryResolveToCommit() : (TfsGitCommit) null;
        if (commit != null)
        {
          foreach (string str in GitFsck.Fsck(rc, commit, fsckMode))
          {
            rc.TraceAlways(1013886, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), str);
            urContext.PushReporter?.WriteOutputLine(str);
          }
        }
      }
    }

    private static void FilterRefs(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      bool includesNewCommits = false)
    {
      List<TfsGitRefUpdateRequest> refUpdateRequestList = new List<TfsGitRefUpdateRequest>();
      List<TfsGitRefUpdateResult> gitRefUpdateResultList = new List<TfsGitRefUpdateResult>();
      ITeamFoundationEventService service = rc.GetService<ITeamFoundationEventService>();
      HashSet<string> stringSet = new HashSet<string>(urContext.Repo.Settings.EnforceConsistentCase ? (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase : (IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (TfsGitRefUpdateRequest refUpdate in urContext.RefUpdates)
      {
        if (!stringSet.Add(refUpdate.Name))
          throw new GitArgumentException(Resources.Format("GitDuplicateRefNameSupplied", (object) refUpdate.Name));
        if (!(refUpdate.OldObjectId == refUpdate.NewObjectId) || refUpdate.NewObjectId.IsEmpty)
        {
          TfsGitRefUpdateResult refUpdateResult = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.Unprocessed);
          if (!RefUtil.IsValidRefName(refUpdate.Name, true) && !refUpdate.NewObjectId.IsEmpty)
          {
            refUpdateRequestList.Add(refUpdate);
            bool flag = RefUtil.IsValidRefName(refUpdate.Name, false);
            rc.Trace(1013174, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Invalid Ref Name: {0}. Git spec valid? {1}", (object) refUpdate.Name, (object) flag);
            TeamFoundationGitRefService.UpdateRefsContext updateRefsContext = urContext;
            string name = refUpdate.Name;
            string str;
            if (!flag)
              str = Resources.Format("InvalidGitRefName", (object) refUpdate.Name);
            else
              str = Resources.Format("InvalidGitRefNameForTfs", (object) refUpdate.Name);
            string line = string.Format("ng {0} {1}", (object) name, (object) str);
            updateRefsContext.WritePushReportLine(line);
            TfsGitRefUpdateResult gitRefUpdateResult = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.InvalidRefName);
            urContext.ResultSet.Results.Add(gitRefUpdateResult);
            ++urContext.ResultSet.CountFailed;
          }
          else
          {
            string errorMessage;
            TfsGitRefUpdateResult result;
            if (!TeamFoundationGitRefService.UserHasRefUpdatePermissions(rc, refUpdate, urContext, includesNewCommits, out errorMessage, out result))
            {
              if (refUpdate.NewObjectId == Sha1Id.Empty && result.Status == GitRefUpdateStatus.ForcePushRequired)
              {
                urContext.UpdatesToFailIfRefsExist.Add(refUpdate);
              }
              else
              {
                refUpdateRequestList.Add(refUpdate);
                rc.Trace(1013175, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Permission lacking to write ref named: {0}", (object) refUpdate.Name);
                urContext.WritePushReportLine(string.Format("ng {0} {1}", (object) refUpdate.Name, (object) errorMessage));
                TfsGitRefUpdateResult gitRefUpdateResult = result;
                ++urContext.ResultSet.CountFailed;
                urContext.ResultSet.Results.Add(gitRefUpdateResult);
                continue;
              }
            }
            string rejectionReason;
            bool flag = urContext.RefUpdateValidator.ValidatePolicies(rc, urContext.Repo, refUpdate, out rejectionReason);
            rc.Trace(1013348, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Ref update approved: {0}, rejection reason: {1}", (object) flag, (object) rejectionReason);
            if (!flag)
            {
              TeamFoundationGitRefService.DenyRefUpdate(urContext, refUpdate, rejectionReason);
              refUpdateRequestList.Add(refUpdate);
            }
            else
            {
              if (rejectionReason != null)
                urContext.PushReporter?.WriteOutputLine(rejectionReason);
              try
              {
                service.PublishDecisionPoint(rc, (object) new RefUpdateNotification(urContext.Repo.Key.GetProjectUri(), urContext.Repo.Key.RepoId, urContext.Repo.Name, rc.UserContext, rc.AuthenticatedUserName, refUpdateResult));
                gitRefUpdateResultList.Add(refUpdateResult);
              }
              catch (ActionDeniedBySubscriberException ex)
              {
                refUpdateRequestList.Add(refUpdate);
                rc.Trace(1013144, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Plugin rejected update for ref with name: {0}", (object) refUpdate.Name);
                string subscriberName = TeamFoundationGitRefService.GetSubscriberName(ex);
                TeamFoundationGitRefService.UpdateRefsContext updateRefsContext = urContext;
                object[] objArray = new object[4]
                {
                  (object) refUpdate.Name,
                  (object) Resources.Get("RefRejectedByPlugin"),
                  null,
                  null
                };
                string str1;
                if (!subscriberName.IsNullOrEmpty<char>())
                  str1 = Resources.Format("PluginName", (object) subscriberName);
                else
                  str1 = string.Empty;
                objArray[2] = (object) str1;
                string str2;
                if (!ex.Message.IsNullOrEmpty<char>())
                  str2 = Resources.Format("CustomMessage", (object) ex.Message);
                else
                  str2 = string.Empty;
                objArray[3] = (object) str2;
                string line = string.Format("ng {0} {1} {2} {3}", objArray);
                updateRefsContext.WritePushReportLine(line);
                TfsGitRefUpdateResult gitRefUpdateResult = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.RejectedByPlugin, subscriberName, ex.Message);
                urContext.ResultSet.Results.Add(gitRefUpdateResult);
                ++urContext.ResultSet.CountFailed;
              }
            }
          }
        }
      }
      if (refUpdateRequestList.Count != 0)
      {
        refUpdateRequestList.ForEach((Action<TfsGitRefUpdateRequest>) (ru => urContext.RefUpdates.Remove(ru)));
        if (urContext.UpdateMode == GitRefUpdateMode.BestEffort)
        {
          urContext.MissingCommitWalker.PruneForRemainingRefUpdates((IEnumerable<TfsGitRefUpdateRequest>) urContext.RefUpdates);
        }
        else
        {
          urContext.ResultSet.CountFailed += urContext.RefUpdates.Count;
          urContext.RefUpdates.ForEach((Action<TfsGitRefUpdateRequest>) (ru => urContext.ResultSet.Results.Add(TfsGitRefUpdateResult.FromRefUpdate(ru, GitRefUpdateStatus.Unprocessed))));
          urContext.RefUpdates.Clear();
          return;
        }
      }
      if (gitRefUpdateResultList.Count <= 0)
        return;
      try
      {
        PushNotification notificationEvent = new PushNotification(urContext.Repo.Key.GetProjectUri(), urContext.Repo.Key.RepoId, urContext.Repo.Name, rc.UserContext, rc.AuthenticatedUserName, (IEnumerable<TfsGitRefUpdateResult>) gitRefUpdateResultList, (IEnumerable<Sha1Id>) urContext.MissingCommitWalker.MissingCommits, DateTime.MinValue, 0, rc.RemoteIPAddress());
        service.PublishDecisionPoint(rc, (object) notificationEvent);
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        TeamFoundationGitRefService.HandlePushRejectionByPlugIn(rc, urContext, gitRefUpdateResultList, ex, 1013145);
      }
    }

    private static TfsGitRefUpdateResult DenyRefUpdate(
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      TfsGitRefUpdateRequest refUpdate,
      string message)
    {
      urContext.WritePushReportLine(string.Format("ng {0} {1}", (object) refUpdate.Name, (object) message));
      TfsGitRefUpdateResult gitRefUpdateResult = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.RejectedByPolicy, customMessage: message);
      ++urContext.ResultSet.CountFailed;
      urContext.ResultSet.Results.Add(gitRefUpdateResult);
      return gitRefUpdateResult;
    }

    private static bool ApplyRefUpdates(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext)
    {
      bool flag1 = false;
      bool flag2 = true;
      int num = urContext.RefUpdates.Count;
      while (num > 0 && (!flag1 || urContext.UpdateMode != GitRefUpdateMode.AllOrNone))
      {
        if (!flag2)
          urContext.MissingCommitWalker.PruneForRemainingRefUpdates((IEnumerable<TfsGitRefUpdateRequest>) urContext.RefUpdates);
        flag2 = false;
        List<TfsGitRefUpdateResult> database = TeamFoundationGitRefService.WriteToDatabase(rc, urContext);
        num = 0;
        foreach (TfsGitRefUpdateResult result in database)
        {
          flag1 |= !TeamFoundationGitRefService.HandleRefUpdateResultStatus(rc, urContext, result);
          if (flag1 && urContext.UpdateMode == GitRefUpdateMode.AllOrNone)
          {
            urContext.ResultSet.CountFailed += urContext.RefUpdates.Count;
            urContext.RefUpdates.ForEach((Action<TfsGitRefUpdateRequest>) (ru => urContext.ResultSet.Results.Add(TfsGitRefUpdateResult.FromRefUpdate(ru, GitRefUpdateStatus.Unprocessed))));
            urContext.RefUpdates.Clear();
            return false;
          }
          if (result.Status == GitRefUpdateStatus.Unprocessed)
            ++num;
        }
        if (num > urContext.RefUpdates.Count)
        {
          foreach (TfsGitRefUpdateResult result in database)
          {
            rc.Trace(1013076, TraceLevel.Error, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Ref Unprocessed: {0}", (object) result.Name);
            urContext.WritePushReportLine(string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Format("InvalidGitRefName", (object) result.Name)));
            urContext.ResultSet.Results.Add(TfsGitRefUpdateResult.UpdateResult(result, GitRefUpdateStatus.RejectedByPlugin));
          }
          urContext.RefUpdates.Clear();
          break;
        }
        rc.RequestContextInternal().CheckCanceled();
      }
      return !flag1;
    }

    private static bool PublishNotifications(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      QueuedGitPushJobsContext queuedGitPushJobsContext)
    {
      rc.Trace(1013109, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Publishing notifications.");
      ITeamFoundationEventService service = rc.GetService<ITeamFoundationEventService>();
      if (urContext.ResultSet.Results.Count == 0)
        return false;
      List<TfsGitRefUpdateResult> refUpdateResults = new List<TfsGitRefUpdateResult>();
      foreach (TfsGitRefUpdateResult result in urContext.ResultSet.Results)
      {
        if (result.Succeeded && result.Status != GitRefUpdateStatus.SucceededNonExistentRef && (!(result.OldObjectId == result.NewObjectId) || result.NewObjectId.IsEmpty))
        {
          service.PublishNotification(rc, (object) new RefUpdateNotification(urContext.Repo.Key.GetProjectUri(), urContext.Repo.Key.RepoId, result.Name, rc.UserContext, rc.AuthenticatedUserName, result));
          refUpdateResults.Add(result);
        }
      }
      if (refUpdateResults.Count == 0)
        return false;
      rc.Trace(1013263, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Publishing a PushNotification for push {0} against repo {1} that included {2} commits: {3}", (object) urContext.ResultSet.PushId, (object) urContext.Repo.Key.RepoId, (object) urContext.AddedCommitIds.Count, (object) string.Join<Sha1Id>(",", urContext.AddedCommitIds.Take<Sha1Id>(5)));
      PushNotification pushNotification = new PushNotification(urContext.Repo.Key.GetProjectUri(), urContext.Repo.Key.RepoId, urContext.Repo.Name, rc.UserContext, rc.AuthenticatedUserName, (IEnumerable<TfsGitRefUpdateResult>) refUpdateResults, (IEnumerable<Sha1Id>) urContext.AddedCommitIds, urContext.ResultSet.PushTime, urContext.ResultSet.PushId.GetValueOrDefault(), rc.RemoteIPAddress());
      service.PublishNotification(rc, (object) pushNotification);
      if (!rc.IsFeatureEnabled("Git.NewOnIndexUpdateAndOnRefsUpdateJobs"))
        rc.GetService<ITeamFoundationGitPushJobService>().QueueOnIndexUpdateJobs(rc, urContext.Repo, pushNotification, queuedGitPushJobsContext);
      rc.GetService<ITeamFoundationGitPushJobService>().QueueOnRefUpdateJobs(rc, urContext.Repo, pushNotification, queuedGitPushJobsContext);
      rc.Trace(1013110, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Publishing notifications done.");
      return true;
    }

    private static bool UserHasRefUpdatePermissions(
      IVssRequestContext rc,
      TfsGitRefUpdateRequest refUpdate,
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      bool includesNewCommits,
      out string errorMessage,
      out TfsGitRefUpdateResult result)
    {
      result = (TfsGitRefUpdateResult) null;
      errorMessage = (string) null;
      bool flag1 = false;
      bool flag2 = false;
      ITfsGitRepository repo = urContext.Repo;
      bool flag3 = SecurityHelper.Instance.HasForcePushPermission(rc, (RepoScope) repo.Key, refUpdate.Name);
      urContext.AddCTData("HasForcePushPermission", (object) flag3);
      try
      {
        bool flag4 = SecurityHelper.Instance.IsForcePushRequired(rc, repo, refUpdate);
        urContext.AddCTData("ForcePush", (object) flag4);
        if (flag4)
        {
          urContext.AddCTData("PushIdBeforeForcePush", (object) refUpdate.OldObjectId);
          urContext.AddCTData("PushIdAfterForcePush", (object) refUpdate.NewObjectId);
        }
      }
      catch (GitObjectDoesNotExistException ex)
      {
      }
      if (rc.IsFeatureEnabled("Git.RequireGenericContributeForRefUpdatesIncludingCommits") & includesNewCommits)
      {
        try
        {
          if (refUpdate.Name.StartsWith("refs/tags/", StringComparison.Ordinal))
            repo.Permissions.CheckWrite();
        }
        catch (GitNeedsPermissionException ex)
        {
          string[] strArray = refUpdate.Name.Split('/');
          string format = "Rejected ref update on " + (strArray.Length > 1 ? strArray[1] : string.Empty) + " prefix due to missing GenericContribute permissions";
          rc.TraceAlways(1013890, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), format);
          errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.GenericContribute, GitPermissionScope.Repository);
          result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.WritePermissionRequired, customMessage: errorMessage);
          return false;
        }
      }
      if (refUpdate.OldObjectId.IsEmpty && !refUpdate.NewObjectId.IsEmpty)
      {
        if (refUpdate.Name.StartsWith("refs/tags/", StringComparison.Ordinal))
        {
          if (!repo.Permissions.CanCreateTag(refUpdate.Name))
          {
            errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.CreateTag, GitPermissionScope.Repository);
            result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.CreateTagPermissionRequired, customMessage: errorMessage);
          }
        }
        else if (refUpdate.Name.StartsWith("refs/notes/", StringComparison.Ordinal))
          flag1 = true;
        else if (refUpdate.Name.StartsWith("refs/pull/", StringComparison.Ordinal) || refUpdate.Name.StartsWith("refs/azure-repos/merges/", StringComparison.Ordinal))
          flag2 = true;
        else if (!repo.Permissions.CanCreateBranch(refUpdate.Name))
        {
          if (rc.IsFeatureEnabled("Git.RefNameConventionNotMetErrorMessage") && repo.Permissions.HasCreateBranch(considerAnyBranches: true))
          {
            errorMessage = Resources.Format("GitRefNameConventionNotMet", (object) refUpdate.Name, (object) rc.AuthenticatedUserName, (object) GitPermissionScope.Repository.GetCurrentFragmentForResource());
            result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.CreateBranchPermissionRequired, customMessage: errorMessage);
          }
          else
          {
            errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.CreateBranch, GitPermissionScope.Repository);
            result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.CreateBranchPermissionRequired, customMessage: errorMessage);
          }
        }
      }
      else if (!SecurityHelper.Instance.HasWritePermission(rc, (RepoScope) repo.Key, refUpdate.Name))
      {
        errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.GenericContribute, GitPermissionScope.Repository);
        result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.WritePermissionRequired, customMessage: errorMessage);
      }
      else if (refUpdate.Name.StartsWith("refs/notes/", StringComparison.Ordinal))
        flag1 = true;
      else if (refUpdate.Name.StartsWith("refs/pull/", StringComparison.Ordinal) || refUpdate.Name.StartsWith("refs/azure-repos/merges/", StringComparison.Ordinal))
        flag2 = true;
      if (errorMessage == null)
      {
        if (flag2 && !rc.IsSystemContext)
        {
          errorMessage = Resources.Get("GitNeedSystemPermission");
          result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.WritePermissionRequired, customMessage: errorMessage);
        }
        else if (flag1 && !SecurityHelper.Instance.HasManageNotePermission(rc, (RepoScope) repo.Key, refUpdate.Name))
        {
          errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.ManageNote, GitPermissionScope.Repository);
          result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.ManageNotePermissionRequired, customMessage: errorMessage);
        }
      }
      if (errorMessage == null && !flag3 && SecurityHelper.Instance.IsForcePushRequired(rc, repo, refUpdate))
      {
        errorMessage = TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.ForcePush, GitPermissionScope.Branch);
        result = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.ForcePushRequired, customMessage: errorMessage);
      }
      return errorMessage == null;
    }

    internal static bool HandleRefUpdateResultStatus(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      TfsGitRefUpdateResult result)
    {
      if (result.Status == GitRefUpdateStatus.Unprocessed)
        return true;
      if (result.Succeeded)
      {
        ++urContext.ResultSet.CountSucceeded;
        urContext.ResultSet.Results.Add(result);
        return true;
      }
      string line;
      switch (result.Status)
      {
        case GitRefUpdateStatus.ForcePushRequired:
          rc.Trace(1013039, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Force push required: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.ForcePush, GitPermissionScope.Branch));
          break;
        case GitRefUpdateStatus.StaleOldObjectId:
          rc.Trace(1013040, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Stale ref: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Format("GitReferenceStale", (object) result.Name));
          break;
        case GitRefUpdateStatus.InvalidRefName:
          rc.Trace(1013041, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Invalid ref name: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) TeamFoundationGitRefService.FormatInvalidRefNameMessage(result));
          break;
        case GitRefUpdateStatus.UnresolvableToCommit:
          rc.Trace(1013275, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Unresolvable to commit: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Format("GitUnresolvableToCommit", (object) result.NewObjectId.ToString()));
          break;
        case GitRefUpdateStatus.WritePermissionRequired:
          rc.Trace(1013177, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Write permissions required: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Get("GitNeedsWritePermission"));
          break;
        case GitRefUpdateStatus.ManageNotePermissionRequired:
          rc.Trace(1013232, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Note create required: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) TeamFoundationGitRefService.FormatNeedsPermissionsMessage(rc, GitRepositoryPermissions.ManageNote, GitPermissionScope.Repository));
          break;
        case GitRefUpdateStatus.Locked:
          string displayName = IdentityHelper.Instance.GetDisplayName(rc, result.IsLockedById ?? Guid.Empty);
          rc.Trace(1013259, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Ref {0} is locked by {1}", (object) result.Name, (object) displayName);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Format("GitRefLocked", (object) result.Name, (object) displayName));
          break;
        case GitRefUpdateStatus.RefNameConflict:
          rc.Trace(1013274, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Ref name conflict: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) TeamFoundationGitRefService.FormatInvalidRefNameMessage(result));
          break;
        case GitRefUpdateStatus.RejectedByPolicy:
          rc.Trace(1013329, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Policy Failure: {0}", (object) result.Name);
          line = string.Format("ng {0} {1}", (object) result.Name, (object) Resources.Format("GitRefPolicyFailure", (object) result.Name, (object) result.CustomMessage));
          break;
        default:
          throw new NotSupportedException(string.Format(Resources.Get("UnsupportedRefUpdateResultStatus"), (object) result.Status));
      }
      if (line != null)
        urContext.WritePushReportLine(line);
      urContext.RefUpdates.Remove(urContext.RefUpdates.First<TfsGitRefUpdateRequest>((Func<TfsGitRefUpdateRequest, bool>) (ru => ru.Name == result.Name)));
      urContext.ResultSet.Results.Add(result);
      ++urContext.ResultSet.CountFailed;
      return false;
    }

    private static string FormatNeedsPermissionsMessage(
      IVssRequestContext rc,
      GitRepositoryPermissions requiredPermission,
      GitPermissionScope currentScope)
    {
      return Resources.Format("GitNeedsPermission", (object) rc.AuthenticatedUserName, (object) Enum.GetName(typeof (GitRepositoryPermissions), (object) requiredPermission), (object) currentScope.GetCurrentFragmentForResource());
    }

    private static string FormatInvalidRefNameMessage(TfsGitRefUpdateResult result) => result.ConflictingName != null ? Resources.Format("RefConflict", (object) result.ConflictingName) : Resources.Format("InvalidGitRefName", (object) result.Name);

    private static void HandlePushRejectionByPlugIn(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext,
      List<TfsGitRefUpdateResult> resultsToBeProcessed,
      ActionDeniedBySubscriberException e,
      int tracePoint)
    {
      rc.Trace(tracePoint, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Rejected Push by plugin.");
      string subscriberName = TeamFoundationGitRefService.GetSubscriberName(e);
      foreach (TfsGitRefUpdateResult result in resultsToBeProcessed)
      {
        ++urContext.ResultSet.CountFailed;
        TeamFoundationGitRefService.UpdateRefsContext updateRefsContext = urContext;
        object[] objArray = new object[4]
        {
          (object) result.Name,
          (object) Resources.Get("RefRejectedByPlugin"),
          null,
          null
        };
        string str1;
        if (!subscriberName.IsNullOrEmpty<char>())
          str1 = Resources.Format("PluginName", (object) subscriberName);
        else
          str1 = string.Empty;
        objArray[2] = (object) str1;
        string str2;
        if (!e.Message.IsNullOrEmpty<char>())
          str2 = Resources.Format("CustomMessage", (object) e.Message);
        else
          str2 = string.Empty;
        objArray[3] = (object) str2;
        string line = string.Format("ng {0} {1} {2} {3}", objArray);
        updateRefsContext.WritePushReportLine(line);
        urContext.ResultSet.Results.Add(TfsGitRefUpdateResult.UpdateResult(result, GitRefUpdateStatus.RejectedByPlugin, subscriberName, e.Message));
      }
      urContext.RefUpdates.Clear();
    }

    private static List<TfsGitRefUpdateResult> WriteToDatabase(
      IVssRequestContext rc,
      TeamFoundationGitRefService.UpdateRefsContext urContext)
    {
      bool flag = false;
      List<TfsGitRefUpdateWithResolvedCommit> refUpdates = new List<TfsGitRefUpdateWithResolvedCommit>(urContext.RefUpdates.Count);
      Dictionary<string, TfsGitRefUpdateResult> dictionary = new Dictionary<string, TfsGitRefUpdateResult>(urContext.RefUpdates.Count, (IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (TfsGitRefUpdateRequest refUpdate in urContext.RefUpdates)
      {
        Sha1Id? resolvedCommitId = new Sha1Id?();
        if (!refUpdate.NewObjectId.IsEmpty)
        {
          TfsGitObject gitObject = urContext.Repo.TryLookupObject(refUpdate.NewObjectId);
          if (gitObject == null || refUpdate.Name.StartsWith("refs/heads/", StringComparison.Ordinal) && !(gitObject is TfsGitCommit))
          {
            dictionary[refUpdate.Name] = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.UnresolvableToCommit);
            flag = true;
            continue;
          }
          resolvedCommitId = gitObject.TryResolveToCommit()?.ObjectId;
          if (!resolvedCommitId.HasValue)
            rc.Trace(1013678, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), "Ref Name: {0}, NewObjectId {1} did not resolve to a commit", (object) refUpdate.Name, (object) refUpdate.NewObjectId);
        }
        if (refUpdate.OldObjectId == refUpdate.NewObjectId && refUpdate.NewObjectId != Sha1Id.Empty)
        {
          dictionary[refUpdate.Name] = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.Succeeded);
        }
        else
        {
          dictionary[refUpdate.Name] = TfsGitRefUpdateResult.FromRefUpdate(refUpdate, GitRefUpdateStatus.Unprocessed);
          refUpdates.Add(new TfsGitRefUpdateWithResolvedCommit(refUpdate, resolvedCommitId));
        }
      }
      if (!flag && refUpdates.Count > 0)
      {
        Guid teamFoundationId = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(rc).TeamFoundationId;
        string securable = GitUtils.CalculateSecurable(urContext.Repo.Key.ProjectId, urContext.Repo.Key.RepoId, (string) null);
        bool enforceConsistentCase = urContext.Repo.Settings.EnforceConsistentCase;
        urContext.MissingCommitWalker.Walk((Func<IEnumerable<Sha1Id>, List<Sha1Id>>) (x =>
        {
          using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(urContext.Repo.Key))
            return gitOdbComponent.QueryMissingCommits(x);
        }));
        DateTime pushTime;
        int? pushId;
        List<Sha1Id> addCommitIds;
        List<TfsGitRefUpdateResult> gitRefUpdateResultList;
        try
        {
          using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(urContext.Repo.Key))
            gitOdbComponent.WriteCommits((IEnumerable<Sha1Id>) urContext.MissingCommitWalker.MissingCommits, (IEnumerable<Sha1Id>) urContext.MissingCommitWalker.BoundaryCommitParents);
          urContext.MissingCommitWalker.Walk((Func<IEnumerable<Sha1Id>, List<Sha1Id>>) (x =>
          {
            using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
              return gitCoreComponent.GetCommitsMissingPushIds(urContext.Repo.Key, x);
          }));
          using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
            gitRefUpdateResultList = gitCoreComponent.WriteRefs(urContext.Repo.Key, teamFoundationId, (IEnumerable<TfsGitRefUpdateWithResolvedCommit>) refUpdates, (IEnumerable<Sha1Id>) urContext.MissingCommitWalker.MissingCommits, (IEnumerable<Sha1Id>) urContext.MissingCommitWalker.BoundaryCommitParents, securable, enforceConsistentCase, (IEnumerable<TfsGitRefUpdateRequest>) urContext.UpdatesToFailIfRefsExist, out pushTime, out pushId, out addCommitIds);
        }
        catch (GitInvalidParentSpecified ex)
        {
          rc.Trace(1013706, TraceLevel.Error, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), JsonConvert.SerializeObject((object) new
          {
            repoId = urContext.Repo.Key,
            refUpdatesToWrite = refUpdates,
            includedCommits = urContext.MissingCommitWalker.MissingCommits,
            boundaryCommits = urContext.MissingCommitWalker.BoundaryCommitParents
          }));
          throw;
        }
        urContext.ResultSet.PushId = pushId;
        urContext.ResultSet.PushTime = pushTime;
        urContext.AddedCommitIds = addCommitIds;
        foreach (TfsGitRefUpdateResult gitRefUpdateResult in gitRefUpdateResultList)
        {
          if ((gitRefUpdateResult.Status == GitRefUpdateStatus.InvalidRefName || gitRefUpdateResult.Status == GitRefUpdateStatus.RefNameConflict) && gitRefUpdateResult.ConflictingName != null)
            gitRefUpdateResult.CustomMessage = Resources.Format("RefConflict", (object) gitRefUpdateResult.ConflictingName);
          dictionary[gitRefUpdateResult.Name] = gitRefUpdateResult;
        }
      }
      List<TfsGitRefUpdateResult> database = new List<TfsGitRefUpdateResult>(dictionary.Count);
      foreach (TfsGitRefUpdateRequest refUpdate in urContext.RefUpdates)
      {
        TfsGitRefUpdateResult gitRefUpdateResult;
        if (!dictionary.TryGetValue(refUpdate.Name, out gitRefUpdateResult))
          throw new InvalidOperationException(string.Format(Resources.Get("MissingRefWriteResult"), (object) refUpdate.Name, (object) urContext.Repo.Key.RepoId));
        database.Add(gitRefUpdateResult);
      }
      return database;
    }

    private static string GetSubscriberName(ActionDeniedBySubscriberException ex)
    {
      string subscriberName = ex.PropertyCollection.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (property => property.Key.Equals("Microsoft.TeamFoundation.SubscriberName"))).Value.ToString();
      if (string.IsNullOrEmpty(subscriberName))
        subscriberName = ex.PropertyCollection.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (property => property.Key.Equals("Microsoft.TeamFoundation.SubscriberType"))).Value.ToString();
      return subscriberName;
    }

    internal static bool IsBranchNameValid(string refName)
    {
      int length;
      if (refName.StartsWith("refs/tags/"))
      {
        length = "refs/tags/".Length;
      }
      else
      {
        if (!refName.StartsWith("refs/heads/"))
          return true;
        length = "refs/heads/".Length;
      }
      if (refName.Length - length != 40)
        return true;
      for (int index = length; index < refName.Length; ++index)
      {
        char ch = refName[index];
        if ((ch < 'a' || ch > 'f') && (ch < 'A' || ch > 'F') && (ch < '0' || ch > '9'))
          return true;
      }
      return false;
    }

    internal class UpdateRefsContext : IDisposable
    {
      public ITeamFoundationGitRefUpdateValidator RefUpdateValidator { get; private set; }

      public IGitPushReporter PushReporter { get; private set; }

      public GitRefUpdateMode UpdateMode { get; private set; }

      public ClientTraceData CTData { get; private set; }

      public TfsGitRefUpdateResultSet ResultSet { get; private set; }

      public TeamFoundationGitRefService.MissingCommitWalker MissingCommitWalker { get; }

      public List<TfsGitRefUpdateRequest> RefUpdates { get; internal set; }

      public bool AnyFailures => this.ResultSet.CountFailed != 0;

      public ITfsGitRepository Repo { get; private set; }

      public List<Sha1Id> AddedCommitIds { get; internal set; }

      public List<TfsGitRefUpdateRequest> UpdatesToFailIfRefsExist { get; private set; }

      public UpdateRefsContext(
        IVssRequestContext rc,
        IProjectService projectService,
        Guid repoId,
        ITeamFoundationGitRefUpdateValidator refUpdateValidator,
        List<TfsGitRefUpdateRequest> refUpdates,
        List<TfsIncludedGitCommit> includedCommits,
        GitRefUpdateMode updateMode,
        IGitPushReporter pushReporter,
        ClientTraceData ctData)
      {
        ArgumentUtility.CheckForEmptyGuid(repoId, nameof (repoId));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) refUpdates, nameof (refUpdates));
        foreach (TfsGitRefUpdateRequest refUpdate in refUpdates)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(refUpdate.Name, "refUpdate.Name");
        includedCommits = includedCommits ?? new List<TfsIncludedGitCommit>(0);
        foreach (TfsIncludedGitCommit includedCommit in includedCommits)
        {
          ArgumentUtility.CheckForNull<IEnumerable<Sha1Id>>(includedCommit.ParentCommitIds, "includedCommit.ParentCommitIds");
          if (includedCommit.CommitId.IsEmpty)
            throw new GitCommitDoesNotExistException(includedCommit.CommitId);
          foreach (Sha1Id parentCommitId in includedCommit.ParentCommitIds)
          {
            if (parentCommitId.IsEmpty)
              throw new GitCommitDoesNotExistException(parentCommitId);
          }
        }
        this.RefUpdateValidator = refUpdateValidator;
        this.CTData = ctData;
        this.UpdateMode = updateMode;
        this.RefUpdates = refUpdates;
        this.PushReporter = pushReporter;
        this.ResultSet = new TfsGitRefUpdateResultSet();
        this.UpdatesToFailIfRefsExist = new List<TfsGitRefUpdateRequest>(0);
        try
        {
          this.Repo = rc.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(rc, repoId);
          this.MissingCommitWalker = new TeamFoundationGitRefService.MissingCommitWalker(rc, this.Repo, (IEnumerable<TfsIncludedGitCommit>) includedCommits, (IEnumerable<TfsGitRefUpdateRequest>) refUpdates);
        }
        catch
        {
          this.Dispose();
          throw;
        }
      }

      public void Dispose() => this.Repo?.Dispose();

      public void AddCTData(string name, int value)
      {
        if (this.CTData == null)
          return;
        this.CTData.Add(name, (object) value);
      }

      public void AddCTData(string name, object value)
      {
        if (this.CTData == null)
          return;
        this.CTData.Add(name, value);
      }

      public void WritePushReportLine(string line)
      {
        if (this.PushReporter == null)
          return;
        this.PushReporter.WritePushReportLine(line);
      }
    }

    internal sealed class MissingCommitWalker
    {
      private readonly IVssRequestContext m_rc;
      private readonly ITfsGitRepository m_repo;
      private readonly Lazy<IGitCommitGraph> m_commitGraph;
      private readonly AncestralGraphAlgorithm<int, Sha1Id> m_algo;

      public MissingCommitWalker(
        IVssRequestContext rc,
        ITfsGitRepository repo,
        IEnumerable<TfsIncludedGitCommit> requiredCommits,
        IEnumerable<TfsGitRefUpdateRequest> requiredRefUpdates)
      {
        this.m_rc = rc;
        this.m_repo = repo;
        this.m_algo = new AncestralGraphAlgorithm<int, Sha1Id>();
        this.MissingCommits = new HashSet<Sha1Id>();
        this.BoundaryCommitParents = new HashSet<Sha1Id>();
        this.MissingCommits.AddRange<Sha1Id, HashSet<Sha1Id>>(requiredCommits.Select<TfsIncludedGitCommit, Sha1Id>((Func<TfsIncludedGitCommit, Sha1Id>) (x => x.CommitId)));
        int count1 = this.BoundaryCommitParents.Count;
        this.BoundaryCommitParents.AddRange<Sha1Id, HashSet<Sha1Id>>(requiredCommits.SelectMany<TfsIncludedGitCommit, Sha1Id>((Func<TfsIncludedGitCommit, IEnumerable<Sha1Id>>) (x => x.ParentCommitIds)).Where<Sha1Id>((Func<Sha1Id, bool>) (x => !this.MissingCommits.Contains(x))));
        int count2 = this.MissingCommits.Count;
        foreach (TfsGitRefUpdateRequest requiredRefUpdate in requiredRefUpdates)
        {
          if (!requiredRefUpdate.NewObjectId.IsEmpty)
          {
            TfsGitCommit commit = this.m_repo.TryLookupObject(requiredRefUpdate.NewObjectId).TryResolveToCommit();
            if (commit != null)
              this.MissingCommits.Add(commit.ObjectId);
          }
        }
        this.m_commitGraph = new Lazy<IGitCommitGraph>((Func<IGitCommitGraph>) (() => this.m_repo.GetCommitGraph((IEnumerable<Sha1Id>) this.MissingCommits)));
        if (this.MissingCommits.Count == count2)
          return;
        this.ResetBoundaryParentsFromAllMissingCommits();
      }

      private void ResetBoundaryParentsFromAllMissingCommits()
      {
        this.BoundaryCommitParents.Clear();
        this.BoundaryCommitParents.AddRange<Sha1Id, HashSet<Sha1Id>>(this.MissingCommits.SelectMany<Sha1Id, Sha1Id>((Func<Sha1Id, IEnumerable<Sha1Id>>) (x => this.m_commitGraph.Value.OutNeighbors(x))).Where<Sha1Id>((Func<Sha1Id, bool>) (x => !this.MissingCommits.Contains(x))));
      }

      public void Walk(
        Func<IEnumerable<Sha1Id>, List<Sha1Id>> getMissingCommits)
      {
        int maxDistance = 1;
        bool flag = false;
        while (!this.m_commitGraph.IsValueCreated || maxDistance <= this.m_commitGraph.Value.NumVertices * 2)
        {
          List<Sha1Id> sha1IdList = getMissingCommits((IEnumerable<Sha1Id>) this.BoundaryCommitParents);
          if (sha1IdList.Count == 0)
          {
            if (!flag)
              break;
            this.ResetBoundaryParentsFromAllMissingCommits();
            break;
          }
          flag = true;
          this.MissingCommits.AddRange<Sha1Id, HashSet<Sha1Id>>((IEnumerable<Sha1Id>) sha1IdList);
          this.BoundaryCommitParents.Clear();
          this.BoundaryCommitParents.AddRange<Sha1Id, HashSet<Sha1Id>>(this.m_algo.GetReachable((IDirectedGraph<int, Sha1Id>) this.m_commitGraph.Value, (IEnumerable<Sha1Id>) sha1IdList, maxDistance: maxDistance).Where<Sha1Id>((Func<Sha1Id, bool>) (id => !this.MissingCommits.Contains(id))));
          this.m_rc.TraceAlways(1013707, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), JsonConvert.SerializeObject((object) new
          {
            repoId = this.m_repo.Key,
            walkDistance = maxDistance,
            numMissingCommits = sha1IdList.Count,
            initialMissingCommits = sha1IdList.Take<Sha1Id>(10),
            numBoundaryCommits = this.BoundaryCommitParents.Count,
            initialBoundaryCommits = this.BoundaryCommitParents.Take<Sha1Id>(10)
          }));
          checked { maxDistance *= 2; }
        }
      }

      public void PruneForRemainingRefUpdates(IEnumerable<TfsGitRefUpdateRequest> refUpdates)
      {
        this.m_rc.Trace(1013044, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), nameof (PruneForRemainingRefUpdates));
        int oldMissingCommitsCount = this.MissingCommits.Count;
        List<Sha1Id> reachableFrom = new List<Sha1Id>();
        foreach (TfsGitRefUpdateRequest refUpdate in refUpdates)
        {
          if (!refUpdate.NewObjectId.IsEmpty)
          {
            TfsGitCommit commit = this.m_repo.TryLookupObject(refUpdate.NewObjectId).TryResolveToCommit();
            if (commit != null)
              reachableFrom.Add(commit.ObjectId);
          }
        }
        VertexToLabelSet<int, Sha1Id> restrictedLabels = new VertexToLabelSet<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) this.m_commitGraph.Value, (IReadOnlySet<Sha1Id>) new ReadOnlySetWrapper<Sha1Id>((ISet<Sha1Id>) this.MissingCommits));
        this.MissingCommits = new HashSet<Sha1Id>(this.m_algo.GetReachable((IDirectedGraph<int, Sha1Id>) this.m_commitGraph.Value, (IEnumerable<Sha1Id>) reachableFrom, (IReadOnlySet<int>) restrictedLabels));
        this.ResetBoundaryParentsFromAllMissingCommits();
        this.m_rc.TraceConditionally(1013045, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRefService), (Func<string>) (() => string.Format("{0} reduced commits from {1} to {2}", (object) nameof (PruneForRemainingRefUpdates), (object) oldMissingCommitsCount, (object) this.MissingCommits.Count)));
      }

      public HashSet<Sha1Id> MissingCommits { get; private set; }

      public HashSet<Sha1Id> BoundaryCommitParents { get; }
    }
  }
}
