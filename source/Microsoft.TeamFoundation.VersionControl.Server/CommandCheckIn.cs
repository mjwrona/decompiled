// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandCheckIn
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Telemetry;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal abstract class CommandCheckIn : VersionControlCommand
  {
    private bool m_successful = true;
    private StreamingCollection<Failure> m_conflicts;
    private StreamingCollection<Failure> m_failures;
    private ObjectBinder<PendingChangeConflict> m_pendingChangeConflictBinder;
    private ResultCollection m_rc;
    private VersionedItemComponent m_db;
    private ObjectBinder<GetOperation> m_getOpBinder;
    private CheckinResult m_result;
    private bool m_returnLocalVersionUpdates;

    public CommandCheckIn(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public StreamingCollection<Failure> Conflicts => this.m_conflicts;

    public StreamingCollection<Failure> Failures => this.m_failures;

    protected CheckinResult ExecuteInternal(
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      string workspaceName,
      int workspaceVersion,
      PendingSetType workspaceType,
      string computer,
      Workspace localWorkspace,
      string[] serverItems,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions,
      bool deferCheckIn,
      int checkInTicket)
    {
      this.RequestContext.Trace(700352, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, string.Format("{0},{1},{2},{3}", (object) info.Committer, (object) info.Owner, (object) workspaceName, (object) info.Comment));
      Microsoft.VisualStudio.Services.Identity.Identity changesetOwner = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      info.CreationDate = info.CreationDate.ToUniversalTime();
      this.m_returnLocalVersionUpdates = false;
      if (localWorkspace != null)
        this.m_returnLocalVersionUpdates = localWorkspace.IsLocal || (localWorkspace.Options & 1) != 0;
      if (string.IsNullOrEmpty(info.Owner))
      {
        changesetOwner = owner;
      }
      else
      {
        SecurityManager securityWrapper = this.SecurityWrapper;
        if ((checkinOptions & CheckInOptions2.ValidateCheckInOwner) == CheckInOptions2.None)
          changesetOwner = this.RequestContext.GetService<IVssIdentitySearchService>().FindActiveMembers(this.RequestContext, IdentitySearchFilter.General, info.Owner, "Microsoft.TeamFoundation.UnauthenticatedIdentity").FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (changesetOwner == null || (checkinOptions & CheckInOptions2.ValidateCheckInOwner) != CheckInOptions2.None)
        {
          try
          {
            changesetOwner = TfvcIdentityHelper.FindIdentity(this.RequestContext, info.Owner);
          }
          catch (IdentityNotFoundException ex1)
          {
            if ((checkinOptions & CheckInOptions2.ValidateCheckInOwner) != CheckInOptions2.None)
            {
              throw;
            }
            else
            {
              this.RequestContext.TraceException(700046, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, (Exception) ex1);
              string name;
              string domain;
              UserNameUtil.Parse(info.Owner, out name, out domain);
              try
              {
                changesetOwner = this.RequestContext.GetService<IVssIdentitySystemUserService>().CreateVersionControlIdentity(this.RequestContext, domain, name);
              }
              catch (AccessCheckException ex2)
              {
                this.RequestContext.TraceException(700047, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, (Exception) ex2);
                throw new CannotCreateNewUserDuringCheckinException(name);
              }
            }
          }
        }
      }
      this.m_result = new CheckinResult();
      VersionedItemPermissions permissionRequired = VersionedItemPermissions.Checkin;
      bool checkinContainsLocks = false;
      IdentityDescriptor userContext = this.RequestContext.UserContext;
      if (!IdentityDescriptorComparer.Instance.Equals(changesetOwner.Descriptor, userContext) || !VersionControlSqlResourceComponent.IsDateNull(info.CreationDate) || (checkinOptions & CheckInOptions2.SuppressEvent) != CheckInOptions2.None)
        permissionRequired |= VersionedItemPermissions.CheckinOther;
      bool isPaged = deferCheckIn || checkInTicket != 0;
      ICheckinItemManager checkinItemManager;
      if (!isPaged)
      {
        checkinItemManager = (ICheckinItemManager) new CheckinItemManager();
      }
      else
      {
        serverItems = serverItems ?? Array.Empty<string>();
        checkinItemManager = (ICheckinItemManager) new WorkspacePendingItemDbPagingManager(this.m_versionControlRequestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WKS:{0}/{1}/{2}", (object) owner.Id, (object) ((int) workspaceType).ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) workspaceName), checkInTicket);
      }
      Set<string> set = (Set<string>) null;
      if (serverItems != null)
      {
        set = new Set<string>(serverItems.Length, (IEqualityComparer<string>) VersionControlPath.FullPathComparer);
        for (int index = 0; index < serverItems.Length; ++index)
        {
          if (serverItems[index] != null)
            set.Add(serverItems[index]);
        }
      }
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      using (checkinItemManager as IDisposable)
      {
        this.m_rc = this.m_db.QueryPendingChangesForCheckin(owner.Id, workspaceName, workspaceVersion, workspaceType);
        bool flag = false;
        List<PendingChangeLight> items = this.m_rc.GetCurrent<PendingChangeLight>().Items;
        if (set == null)
        {
          foreach (PendingChangeLight pendingChangeLight in items)
          {
            this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, pendingChangeLight.ItemPathPair);
            if ((pendingChangeLight.ChangeType & ChangeType.Rename) != ChangeType.None)
              this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, pendingChangeLight.SourceItemPathPair);
          }
          items.Clear();
        }
        this.m_rc.NextResult();
        ObjectBinder<PendingChangeLight> current1 = this.m_rc.GetCurrent<PendingChangeLight>();
        int num1 = 0;
        int num2 = 0;
        while (current1.MoveNext())
        {
          PendingChangeLight current2 = current1.Current;
          if (set == null || set.Contains(current2.ServerItem))
          {
            this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, permissionRequired, current2.ItemPathPair);
            if ((current2.ChangeType & ChangeType.Lock) != ChangeType.None)
              checkinContainsLocks = true;
            if ((current2.ChangeType & ChangeType.Rename) != ChangeType.None && current2.SourceServerItem != null)
            {
              this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, permissionRequired, current2.SourceItemPathPair);
              if (current2.ItemType == ItemType.Folder)
                this.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, permissionRequired, current2.SourceItemPathPair);
            }
            if ((current2.ChangeType & ChangeType.Rename) != ChangeType.None || (current2.ChangeType & ChangeType.Branch) != ChangeType.None)
              flag = true;
            if (current2.ItemType == ItemType.Folder && ((current2.ChangeType & ChangeType.Rename) != ChangeType.None || (current2.ChangeType & ChangeType.Delete) != ChangeType.None))
            {
              foreach (PendingChangeLight pendingChangeLight in items)
              {
                if (VersionControlPath.IsSubItem(pendingChangeLight.ServerItem, current2.ServerItem))
                {
                  this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, pendingChangeLight.ItemPathPair);
                  if ((current2.ChangeType & ChangeType.Rename) != ChangeType.None)
                    this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, pendingChangeLight.SourceItemPathPair);
                }
              }
              this.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, permissionRequired, current2.ItemPathPair);
            }
            if ((current2.ChangeType & (ChangeType.Edit | ChangeType.Encoding)) == ChangeType.Edit && !current2.HasContentChange)
              ++num2;
            ++num1;
            if (serverItems == null && workspaceType == PendingSetType.Workspace && (current2.ChangeType & ChangeType.Edit) == ChangeType.Edit && (checkinOptions & CheckInOptions2.AllContentUploaded) == CheckInOptions2.None)
              throw new CannotCheckinAllWithEditException();
            try
            {
              checkinItemManager.Enqueue(current2.ItemPathPair);
            }
            catch (InvalidLobParameterException ex)
            {
              this.RequestContext.TraceException(700048, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, (Exception) ex);
              throw new InvalidCheckinTicketException(checkInTicket);
            }
            set?.Remove(current2.ServerItem);
          }
        }
        set?.Clear();
        if (isPaged)
        {
          bool isLastPage = !deferCheckIn && checkInTicket != 0;
          try
          {
            checkinItemManager.Flush(isLastPage);
          }
          catch (InvalidLobParameterException ex)
          {
            this.RequestContext.TraceException(700049, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, (Exception) ex);
            throw new InvalidCheckinTicketException(checkInTicket);
          }
          this.m_result.CheckInTicket = checkinItemManager.ParameterId;
          this.m_result.CheckInState = 1;
          if (!isLastPage)
            return this.m_result;
        }
        if (checkinItemManager.TotalCount == 0 && !isPaged)
          return this.m_result;
        this.m_failures = new StreamingCollection<Failure>((Command) this)
        {
          HandleExceptions = false
        };
        this.m_conflicts = new StreamingCollection<Failure>((Command) this)
        {
          HandleExceptions = false
        };
        this.m_result.LocalVersionUpdates = new StreamingCollection<GetOperation>((Command) this)
        {
          HandleExceptions = false
        };
        CheckinNotification notificationEvent1 = new CheckinNotification(owner, changesetOwner, checkinItemManager.Select<ItemPathPair, string>((Func<ItemPathPair, string>) (x => x.ProjectNamePath)), info.Comment, info.CheckinNote, info.PolicyOverride, checkinNotificationInfo, checkinOptions, workspaceName, computer, workspaceType == PendingSetType.Workspace ? CheckinType.Workspace : CheckinType.Shelveset, checkinContainsLocks);
        notificationEvent1.HasAllItems = checkinItemManager.FirstPage.Count == checkinItemManager.TotalCount && !isPaged;
        notificationEvent1.ServerItemsCount = num1;
        notificationEvent1.UndoneChangesCount = num2;
        ITeamFoundationEventService service = this.RequestContext.GetService<ITeamFoundationEventService>();
        service.PublishDecisionPoint(this.RequestContext, (object) notificationEvent1);
        if (notificationEvent1.HasFailures)
        {
          foreach (Failure failure in (IEnumerable<Failure>) notificationEvent1.Failures)
          {
            if (string.IsNullOrEmpty(failure.ServerItem))
              this.m_failures.Enqueue(failure);
            else
              this.m_conflicts.Enqueue(failure);
          }
          this.CompleteCheckIn(false);
          this.m_conflicts.IsComplete = true;
          this.m_failures.IsComplete = true;
          this.m_result.LocalVersionUpdates.IsComplete = true;
          return this.m_result;
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.SecurityWrapper.FindIdentity(this.RequestContext);
        this.m_rc = this.m_db.Checkin(owner.Id, workspaceName, workspaceType, (IEnumerable<ItemPathPair>) checkinItemManager, info.Comment, info.CreationDate, changesetOwner.Id, info.CheckinNote, info.PolicyOverride, identity.Id, (checkinOptions & CheckInOptions2.AllowUnchangedContent) != 0, true, this.m_returnLocalVersionUpdates, checkinItemManager.TotalCount, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
        if (flag)
          this.RepositorySecurity.OnDataChanged(this.RequestContext);
        ObjectBinder<int> current3 = this.m_rc.GetCurrent<int>();
        current3.MoveNext();
        this.m_result.ChangesetId = current3.Current;
        this.m_rc.NextResult();
        this.m_result.UndoneServerItems.AddRange((IEnumerable<string>) this.m_rc.GetCurrent<string>().Items);
        this.m_rc.NextResult();
        ObjectBinder<DateTime> current4 = this.m_rc.GetCurrent<DateTime>();
        current4.MoveNext();
        this.m_result.CreationDate = current4.Current;
        this.m_rc.NextResult();
        this.m_pendingChangeConflictBinder = this.m_rc.GetCurrent<PendingChangeConflict>();
        if (this.m_result.ChangesetId > 0)
        {
          CheckinNotification notificationEvent2 = new CheckinNotification(owner, changesetOwner, checkinItemManager.Select<ItemPathPair, string>((Func<ItemPathPair, string>) (x => x.ProjectNamePath)), info.Comment, info.CheckinNote, info.PolicyOverride, checkinNotificationInfo, checkinOptions, workspaceName, computer, workspaceType == PendingSetType.Workspace ? CheckinType.Workspace : CheckinType.Shelveset, checkinContainsLocks, this.m_result.ChangesetId, isPaged);
          service.PublishNotification(this.RequestContext, (object) notificationEvent2);
          this.m_result.CheckInState = 2;
        }
        else
          this.m_result.CheckInState = 3;
      }
      this.ContinueExecution();
      if (this.IsCacheFull)
        this.RequestContext.PartialResultsReady();
      return this.m_result;
    }

    public override void ContinueExecution()
    {
      bool flag;
      while (flag = this.m_pendingChangeConflictBinder.MoveNext())
      {
        this.m_successful = false;
        this.m_conflicts.Enqueue(new Failure((Exception) this.m_pendingChangeConflictBinder.Current.toException()));
        if (this.IsCacheFull)
          return;
      }
      if (this.m_returnLocalVersionUpdates)
      {
        if (this.m_getOpBinder == null)
        {
          this.m_rc.NextResult();
          this.m_getOpBinder = this.m_rc.GetCurrent<GetOperation>();
        }
        while (flag = this.m_getOpBinder.MoveNext())
        {
          this.m_result.LocalVersionUpdates.Enqueue(this.m_getOpBinder.Current);
          if (this.IsCacheFull)
            return;
        }
      }
      if (flag)
        return;
      this.CompleteCheckIn(this.m_successful);
      this.m_conflicts.IsComplete = true;
      this.m_failures.IsComplete = true;
      this.m_result.LocalVersionUpdates.IsComplete = true;
    }

    protected virtual void CompleteCheckIn(bool successful)
    {
    }

    protected void PublishCustomerIntelligence(CheckinResult checkinResult)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.Add("ChangesetId", (double) checkinResult.ChangesetId);
      CustomerIntelligence.Publish(this.RequestContext, "CheckIn", ciData);
      ClientTrace.Publish(this.RequestContext, "CheckIn", ciData);
      this.RequestContext.PublishAppInsightsTelemetry("TFS/Tfvc/CheckIn", ciData, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "ChangesetId",
          "TFS.SourceControl.ChangesetId"
        }
      });
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_rc != null)
      {
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
      }
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (VersionedItemComponent) null;
    }
  }
}
