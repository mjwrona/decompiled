// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DestroyRequest
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class DestroyRequest
  {
    private VersionControlRequestContext m_versionControlRequestContext;

    internal DestroyRequest(
      VersionControlRequestContext versionControlRequestContext)
    {
      this.m_versionControlRequestContext = versionControlRequestContext;
    }

    public void RequestCollection(
      VersionControlRequestContext versionControlRequestContext)
    {
      Guid guid = new Guid("1BC83577-2640-44A1-ADAF-57DD8CD74912");
      versionControlRequestContext.RequestContext.GetService<TeamFoundationJobService>().QueueJobsNow(versionControlRequestContext.RequestContext, (IEnumerable<Guid>) new List<Guid>()
      {
        guid
      }, false);
    }

    public Item[] DestroyItems(
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags,
      out Failure[] failures,
      out StreamingCollection<PendingSet> pendingChanges,
      out StreamingCollection<PendingSet> shelvedChanges)
    {
      pendingChanges = new StreamingCollection<PendingSet>()
      {
        HandleExceptions = false
      };
      shelvedChanges = new StreamingCollection<PendingSet>()
      {
        HandleExceptions = false
      };
      bool keepHistory = (flags & 4) != 0;
      bool preview = (flags & 1) != 0;
      bool flag1 = (flags & 2) != 0;
      bool flag2 = (flags & 8) != 0;
      bool affectedChanges = (flags & 16) != 0;
      if (!keepHistory && stopAtSpec != null)
        throw new ArgumentException(Resources.Get("StopAtSpecMustBeNullOrLastestIfNotKeepingHistory"), nameof (stopAtSpec));
      if (itemSpec.RecursionType != RecursionType.Full)
        throw new ArgumentException(Resources.Get("DestroyRecursionTypeMustBeFull"), nameof (itemSpec));
      if (!itemSpec.isServerItem)
        throw new ArgumentException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("ServerItemRequiredException", (object) itemSpec.Item), nameof (itemSpec));
      if (stopAtSpec == null)
        stopAtSpec = (VersionSpec) new LatestVersionSpec();
      if (stopAtSpec is LabelVersionSpec || stopAtSpec is WorkspaceVersionSpec)
        throw new ArgumentException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("InvalidStopAtSpec"), nameof (stopAtSpec));
      if (TFStringComparer.VersionControlPath.Compare("$/", itemSpec.Item) == 0)
        throw new CannotDestroyRootException("$/");
      if (affectedChanges && !preview)
        throw new ArgumentException(Resources.Get("AffectedChangesOnlyInPreviewMode"));
      if (affectedChanges)
        flag2 = true;
      bool flag3 = flag2;
      bool silent = flag2 & preview;
      failures = Array.Empty<Failure>();
      int changeset = stopAtSpec.ToChangeset(this.m_versionControlRequestContext.RequestContext);
      List<Item> items = new List<Item>(1);
      itemSpec.RecursionType = RecursionType.None;
      versionSpec.QueryItems(this.m_versionControlRequestContext, itemSpec, (Workspace) null, itemSpec.DeletionId != 0 ? DeletedState.Deleted : DeletedState.Any, ItemType.Any, (IList) items, out string _, out string _, 8);
      if (items.Count == 0 || items[0].DeletionId != itemSpec.DeletionId && itemSpec.DeletionId != 0)
        throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, itemSpec, versionSpec);
      if (!this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, items[0].ItemPathPair))
        throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, itemSpec, versionSpec);
      this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, items[0].ItemPathPair);
      this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, items[0].ItemPathPair);
      ITeamFoundationEventService service = this.m_versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>();
      if (!preview)
      {
        DestroyNotification notificationEvent = new DestroyNotification(this.m_versionControlRequestContext.RequestContext.GetUserIdentity(), itemSpec, versionSpec, stopAtSpec, flags);
        service.PublishDecisionPoint(this.m_versionControlRequestContext.RequestContext, (object) notificationEvent);
      }
      List<Item> objList;
      int destroyedItemsCount;
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.Destroy(items[0], changeset, keepHistory, preview, silent, affectedChanges, Resources.Get("DestroyChangesetComment"), (flags & 32) != 0);
        if (flag1)
          this.RequestCollection(this.m_versionControlRequestContext);
        VssStringComparer versionControlPath = TFStringComparer.VersionControlPath;
        string teamProjectPath = VersionControlPath.GetTeamProject(items[0].ServerItem);
        string x = teamProjectPath;
        string serverItem = items[0].ServerItem;
        if (versionControlPath.Compare(x, serverItem) != 0)
          teamProjectPath = (string) null;
        if (!preview && teamProjectPath != null)
          this.m_versionControlRequestContext.VersionControlService.GetTeamProjectFolder(this.m_versionControlRequestContext).RemoveProject(this.m_versionControlRequestContext, teamProjectPath);
        if (!silent)
        {
          objList = resultCollection.GetCurrent<Item>().Items;
          resultCollection.NextResult();
          destroyedItemsCount = objList.Count;
        }
        else
        {
          ObjectBinder<int> current = resultCollection.GetCurrent<int>();
          current.MoveNext();
          destroyedItemsCount = current.Current;
          resultCollection.NextResult();
          objList = new List<Item>();
        }
        failures = resultCollection.GetCurrent<Failure>().Items.ToArray();
        if (affectedChanges)
        {
          resultCollection.NextResult();
          ObjectBinder<PendingChange> current1 = resultCollection.GetCurrent<PendingChange>();
          PendingSetType type1 = PendingSetType.Workspace;
          DestroyRequest.PopulatePendingSets(pendingChanges, current1, type1);
          resultCollection.NextResult();
          ObjectBinder<PendingChange> current2 = resultCollection.GetCurrent<PendingChange>();
          PendingSetType type2 = PendingSetType.Shelveset;
          DestroyRequest.PopulatePendingSets(shelvedChanges, current2, type2);
        }
      }
      if (!preview && destroyedItemsCount > 0)
      {
        string str = (string) null;
        if (string.IsNullOrEmpty(this.m_versionControlRequestContext.RequestContext.DomainUserName))
        {
          object obj = (object) null;
          if (this.m_versionControlRequestContext.RequestContext.Items.TryGetValue(ServicingTokenConstants.RequestingUserName, out obj))
            str = (string) obj;
        }
        else
          str = this.m_versionControlRequestContext.RequestContext.GetRequestingUserDisplayName();
        if (this.m_versionControlRequestContext.VersionControlService.GetLogDestroyEvents(this.m_versionControlRequestContext))
          TeamFoundationEventLog.Default.Log(Resources.Format("DestroyEventLogMessageBrief", (object) items[0].ServerItem), Resources.Format("DestroyEventLogMessage", (object) str, (object) items[0].ServerItem), TeamFoundationEventId.DestroyItemEventId, EventLogEntryType.Warning);
        this.m_versionControlRequestContext.GetRepositorySecurity().OnDataChanged(this.m_versionControlRequestContext.RequestContext);
      }
      if (!preview)
      {
        DestroyNotification notificationEvent = new DestroyNotification(this.m_versionControlRequestContext.RequestContext.GetUserIdentity(), itemSpec, versionSpec, stopAtSpec, flags, destroyedItemsCount);
        service.PublishNotification(this.m_versionControlRequestContext.RequestContext, (object) notificationEvent);
      }
      return !flag3 ? objList.ToArray() : Array.Empty<Item>();
    }

    private static void PopulatePendingSets(
      StreamingCollection<PendingSet> pendingSets,
      ObjectBinder<PendingChange> pendingChangeBinder,
      PendingSetType type)
    {
      int num = -1;
      PendingSet pendingSet = (PendingSet) null;
      while (pendingChangeBinder.MoveNext())
      {
        PendingChange current = pendingChangeBinder.Current;
        if (current.pendingSet.workspaceId != num)
        {
          pendingSet = current.pendingSet;
          pendingSet.Type = type;
          pendingSet.PendingChanges = new StreamingCollection<PendingChange>()
          {
            HandleExceptions = false
          };
          pendingSets.Enqueue(pendingSet);
          num = current.pendingSet.workspaceId;
        }
        pendingSet.PendingChanges.Enqueue(current);
      }
    }
  }
}
