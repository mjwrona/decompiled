// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandBranch
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandBranch : VersionControlCommand
  {
    private StreamingCollection<Failure> m_failures;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private CheckinResult m_result;
    private ObjectBinder<Failure> m_failureBinder;
    private bool m_hasFailures;

    public CommandBranch(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings,
      bool returnFailures)
    {
      Validation validation = this.m_versionControlRequestContext.Validation;
      PathLength serverPathLength = this.m_versionControlRequestContext.MaxSupportedServerPathLength;
      validation.checkServerItem(ref sourcePath, nameof (sourcePath), false, false, true, false, serverPathLength);
      validation.checkServerItem(ref targetPath, nameof (targetPath), false, false, false, true, serverPathLength);
      validation.check((IValidatable) version, nameof (version), false);
      if (info == null)
        info = new Changeset();
      validation.check((IValidatable) info, nameof (info), false);
      Mapping.ValidateSingleRootMappings(mappings, targetPath, this.m_versionControlRequestContext);
      Mapping.OptimizeSingleRootMappings(mappings);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("source", (object) sourcePath);
      ctData.Add("target", (object) targetPath);
      ctData.Add(nameof (version), (object) version);
      ClientTrace.Publish(this.RequestContext, "CreateBranch", ctData);
      this.m_result = new CheckinResult();
      Microsoft.VisualStudio.Services.Identity.Identity changesetOwner = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      SecurityManager securityWrapper = this.SecurityWrapper;
      Microsoft.VisualStudio.Services.Identity.Identity identity = securityWrapper.FindIdentity(this.RequestContext);
      if (!string.IsNullOrEmpty(info.Owner))
        changesetOwner = TfvcIdentityHelper.FindIdentity(this.RequestContext, info.Owner);
      if (changesetOwner == null)
        changesetOwner = identity;
      securityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, sourcePath);
      securityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, sourcePath);
      VersionedItemPermissions permissionRequired = VersionedItemPermissions.Checkin;
      if (!identity.Id.Equals(changesetOwner.Id) || !VersionControlSqlResourceComponent.IsDateNull(info.CreationDate))
        permissionRequired |= VersionedItemPermissions.CheckinOther;
      securityWrapper.CheckItemPermission(this.m_versionControlRequestContext, permissionRequired, targetPath);
      securityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, permissionRequired, targetPath);
      string teamProject = VersionControlPath.GetTeamProject(targetPath);
      if (!this.m_versionControlRequestContext.VersionControlService.GetTeamProjectFolder(this.m_versionControlRequestContext).IsValidTeamProject(this.m_versionControlRequestContext, teamProject))
        throw new TeamProjectNotFoundException(VersionControlPath.GetTeamProjectName(teamProject));
      securityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Merge, targetPath);
      securityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Merge, targetPath);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      using (ResultCollection resultCollection = this.m_db.QueryBranchObjectsByPath(ItemPathPair.FromServerItem(sourcePath), version))
      {
        ObjectBinder<BranchObject> current = resultCollection.GetCurrent<BranchObject>();
        while (current.MoveNext())
        {
          if (current.Current.Properties.RootItem.DeletionId == 0)
          {
            securityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, current.Current.Properties.RootItem.ItemPathPair);
            string relative = VersionControlPath.MakeRelative(sourcePath, current.Current.Properties.RootItem.Item);
            string itemPath = VersionControlPath.Combine(targetPath, relative, serverPathLength);
            securityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.ManageBranch, itemPath);
            if (VersionControlPath.Compare(current.Current.Properties.RootItem.Item, sourcePath) != 0 && VersionControlPath.IsSubItem(current.Current.Properties.RootItem.Item, sourcePath))
              throw new CreateBranchObjectException(Resources.Format("BranchObjectNotRootOfOperationException", (object) current.Current.Properties.RootItem.Item));
          }
        }
      }
      if (VersionControlPath.IsSubItem(targetPath, sourcePath))
        throw new TargetIsChildException(sourcePath, targetPath);
      ITeamFoundationEventService service = this.RequestContext.GetService<ITeamFoundationEventService>();
      service.PublishDecisionPoint(this.RequestContext, (object) new CheckinNotification(identity, changesetOwner, (IEnumerable<string>) new string[1]
      {
        targetPath
      }, info.Comment, (CheckinNote) null, (PolicyOverrideInfo) null, checkinNotificationInfo, CheckInOptions2.OverrideGatedCheckIn, (string) null, (string) null, CheckinType.Branch, false)
      {
        HasAllItems = false
      });
      this.m_failures = new StreamingCollection<Failure>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_results = this.m_db.CreateBranch(changesetOwner.Id, identity.Id, sourcePath, targetPath, version, info, info.CheckinNote, mappings, returnFailures, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
      if (returnFailures)
        this.m_failureBinder = this.m_results.GetCurrent<Failure>();
      this.ContinueExecution();
      if (this.m_hasFailures)
        return;
      if (returnFailures)
        this.m_results.NextResult();
      ObjectBinder<int> current1 = this.m_results.GetCurrent<int>();
      current1.MoveNext();
      this.m_result.ChangesetId = current1.Current;
      this.RepositorySecurity.OnDataChanged(this.RequestContext);
      CheckinNotification notificationEvent = new CheckinNotification(identity, changesetOwner, (IEnumerable<string>) new string[1]
      {
        targetPath
      }, info.Comment, (CheckinNote) null, (PolicyOverrideInfo) null, checkinNotificationInfo, CheckInOptions2.OverrideGatedCheckIn, (string) null, (string) null, CheckinType.Branch, false, this.m_result.ChangesetId, false);
      service.PublishNotification(this.RequestContext, (object) notificationEvent);
    }

    public override void ContinueExecution()
    {
      if (this.m_failureBinder != null)
      {
        while (!this.IsCacheFull && this.m_failureBinder.MoveNext())
        {
          this.m_failures.Enqueue(this.m_failureBinder.Current);
          this.m_hasFailures = true;
        }
        if (this.IsCacheFull)
          return;
        this.m_failureBinder = (ObjectBinder<Failure>) null;
      }
      this.m_failures.IsComplete = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (VersionedItemComponent) null;
    }

    public StreamingCollection<Failure> Failures => this.m_failures;

    public CheckinResult Result => this.m_result;
  }
}
