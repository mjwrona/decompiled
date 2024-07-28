// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class CheckinNotification
  {
    private string m_computerName;
    private string m_workspaceName;
    private string m_workspaceOwnerName;
    private string m_changesetOwnerName;
    private CheckinType m_checkinType;
    private bool m_checkinContainsLocks;
    private IEnumerable<string> m_serverItems;
    private string m_comment;
    private int m_changeset;
    private CheckinNote m_checkinNote;
    private PolicyOverrideInfo m_policyOverrideInfo;
    private CheckinNotificationInfo m_checkinNotificationInfo;
    private CheckInOptions2 m_checkinOptions;
    private bool m_hasAllItems = true;
    private Microsoft.VisualStudio.Services.Identity.Identity m_changesetOwner;
    private Microsoft.VisualStudio.Services.Identity.Identity m_workspaceOwner;
    private bool m_isPaged;

    internal CheckinNotification(
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      Microsoft.VisualStudio.Services.Identity.Identity changesetOwner,
      IEnumerable<string> serverItems,
      string comment,
      CheckinNote checkinNote,
      PolicyOverrideInfo policyOverrideInfo,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions,
      string workspaceName,
      string computerName,
      CheckinType checkinType,
      bool checkinContainsLocks)
      : this(workspaceOwner, changesetOwner, serverItems, comment, checkinNote, policyOverrideInfo, checkinNotificationInfo, checkinOptions, workspaceName, computerName, checkinType, checkinContainsLocks, VersionSpec.UnknownChangeset, false)
    {
    }

    internal CheckinNotification(
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      Microsoft.VisualStudio.Services.Identity.Identity changesetOwner,
      IEnumerable<string> serverItems,
      string comment,
      CheckinNote checkinNote,
      PolicyOverrideInfo policyOverrideInfo,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions,
      string workspaceName,
      string computerName,
      CheckinType checkinType,
      bool checkinContainsLocks,
      int changeset,
      bool isPaged)
    {
      if (isPaged && changeset == VersionSpec.UnknownChangeset)
        throw new InvalidOperationException("CheckinNotification cannot be paged if the changeset id is unknown.");
      this.m_workspaceOwner = workspaceOwner;
      this.m_changesetOwner = changesetOwner;
      this.m_workspaceOwnerName = IdentityHelper.GetDomainUserName(workspaceOwner);
      this.m_changesetOwnerName = IdentityHelper.GetDomainUserName(changesetOwner);
      this.m_serverItems = serverItems;
      this.m_comment = comment;
      this.m_checkinNote = checkinNote;
      this.m_policyOverrideInfo = policyOverrideInfo;
      this.m_checkinNotificationInfo = checkinNotificationInfo;
      this.m_checkinOptions = checkinOptions;
      this.m_workspaceName = workspaceName;
      this.m_computerName = computerName;
      this.m_checkinType = checkinType;
      this.m_checkinContainsLocks = checkinContainsLocks;
      this.m_changeset = changeset;
      this.m_isPaged = isPaged;
    }

    public int Changeset => this.m_changeset;

    public string Comment => this.m_comment;

    public string ComputerName => this.m_computerName;

    public CheckinNotificationInfo NotificationInfo => this.m_checkinNotificationInfo;

    public CheckInOptions2 Options => this.m_checkinOptions;

    public CheckinNote CheckinNote => this.m_checkinNote;

    public PolicyOverrideInfo PolicyOverrideInfo => this.m_policyOverrideInfo;

    public Microsoft.VisualStudio.Services.Identity.Identity ChangesetOwner => this.m_changesetOwner;

    [Obsolete("Use the ChangesetOwner property instead")]
    [Browsable(false)]
    public string ChangesetOwnerName => this.m_changesetOwnerName;

    public Microsoft.VisualStudio.Services.Identity.Identity WorkspaceOwner => this.m_workspaceOwner;

    [Obsolete("Use the WorkspaceOwner property instead")]
    [Browsable(false)]
    public string WorkspaceOwnerName => this.m_workspaceOwnerName;

    public string WorkspaceName => this.m_workspaceName;

    public CheckinType CheckinType => this.m_checkinType;

    [Obsolete("Use the GetSubmittedItems method instead")]
    [Browsable(false)]
    public IEnumerable<string> SubmittedItems => this.m_serverItems;

    public bool HasAllItems
    {
      get => this.m_hasAllItems;
      internal set => this.m_hasAllItems = value;
    }

    public IEnumerable<string> GetSubmittedItems(IVssRequestContext requestContext)
    {
      if (!this.m_isPaged)
        return this.m_serverItems;
      VersionControlRequestContext controlRequestContext = requestContext.GetService<TeamFoundationVersionControlService>().GetVersionControlRequestContext(requestContext);
      CommandQueryChangesForChangeset changesForChangeset = (CommandQueryChangesForChangeset) null;
      try
      {
        changesForChangeset = new CommandQueryChangesForChangeset(controlRequestContext);
        changesForChangeset.Execute(this.m_changeset, false, int.MaxValue, (ItemSpec) null);
        return (IEnumerable<string>) new CheckinNotification.ChangeToStringEnumerableWrapper((IEnumerable<Change>) changesForChangeset.Changes);
      }
      catch (Exception ex)
      {
        changesForChangeset?.Dispose();
        throw;
      }
    }

    internal bool HasFailures { get; private set; }

    internal IList<Failure> Failures { get; private set; }

    internal int ServerItemsCount { get; set; }

    internal int UndoneChangesCount { get; set; }

    internal bool CheckInContainsLocks => this.m_checkinContainsLocks;

    internal string ConvertToShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      bool replace,
      out bool hasFailures)
    {
      VersionControlRequestContext versionControlRequestContext = new VersionControlRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity = versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentity(requestContext);
      if (this.m_checkinType != CheckinType.Workspace || this.m_changeset != VersionSpec.UnknownChangeset)
        throw new InvalidOperationException();
      Shelveset shelveset = new Shelveset();
      shelveset.CheckinNote = this.m_checkinNote;
      shelveset.Comment = this.m_comment;
      shelveset.Name = shelvesetName;
      string identityName;
      string displayName;
      versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(requestContext, identity.Id, out identityName, out displayName);
      shelveset.Owner = identityName;
      shelveset.OwnerDisplayName = displayName;
      if (this.m_policyOverrideInfo != null)
        shelveset.PolicyOverrideComment = this.m_policyOverrideInfo.Comment;
      ArtifactId artifactId = new ArtifactId();
      artifactId.Tool = "WorkItemTracking";
      artifactId.ArtifactType = "WorkItem";
      List<VersionControlLink> versionControlLinkList = new List<VersionControlLink>();
      if (this.m_checkinNotificationInfo != null && this.m_checkinNotificationInfo.WorkItemInfo != null)
      {
        foreach (CheckinNotificationWorkItemInfo notificationWorkItemInfo in this.m_checkinNotificationInfo.WorkItemInfo)
        {
          if (notificationWorkItemInfo.CheckinAction != CheckinWorkItemAction.None)
          {
            VersionControlLink versionControlLink = new VersionControlLink();
            switch (notificationWorkItemInfo.CheckinAction)
            {
              case CheckinWorkItemAction.Resolve:
                versionControlLink.LinkType = 1025;
                break;
              case CheckinWorkItemAction.Associate:
                versionControlLink.LinkType = 1026;
                break;
            }
            artifactId.ToolSpecificId = notificationWorkItemInfo.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            versionControlLink.Url = LinkingUtilities.EncodeUri(artifactId);
            versionControlLinkList.Add(versionControlLink);
          }
        }
      }
      shelveset.Links = versionControlLinkList.ToArray();
      Workspace workspace = Workspace.QueryWorkspace(versionControlRequestContext, this.m_workspaceName, this.m_workspaceOwner.Id.ToString());
      List<Failure> failures = new List<Failure>();
      if ((this.m_checkinOptions & CheckInOptions2.NoConflictsCheckForGatedCheckIn) == CheckInOptions2.None)
        PendingChange.CheckPendingChanges(versionControlRequestContext, workspace, this.m_serverItems, failures, true);
      hasFailures = failures.Count > 0;
      this.Failures = (IList<Failure>) failures;
      this.HasFailures = hasFailures;
      if (!hasFailures)
      {
        try
        {
          shelveset.Shelve(versionControlRequestContext, workspace, this.m_serverItems, replace, failures);
          return WorkspaceSpec.Combine(shelveset.Name, shelveset.Owner);
        }
        catch (NoChangesToShelveException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700096, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
          hasFailures = true;
        }
      }
      return (string) null;
    }

    internal void DeleteShelveset(IVssRequestContext requestContext, string shelvesetSpec)
    {
      VersionControlRequestContext versionControlRequestContext = new VersionControlRequestContext(requestContext);
      string workspaceName;
      string workspaceOwner;
      WorkspaceSpec.Parse(shelvesetSpec, requestContext.GetRequestingUserUniqueName(), out workspaceName, out workspaceOwner);
      string shelvesetName = workspaceName;
      string ownerName = workspaceOwner;
      Shelveset.Delete(versionControlRequestContext, shelvesetName, ownerName, -1);
    }

    private class ChangeToStringEnumerableWrapper : IEnumerable<string>, IEnumerable
    {
      private IEnumerable<Change> m_innerEnumerable;

      public ChangeToStringEnumerableWrapper(IEnumerable<Change> innerEnumerable) => this.m_innerEnumerable = innerEnumerable;

      public IEnumerator<string> GetEnumerator() => (IEnumerator<string>) new CheckinNotification.ChangeToStringEnumerableWrapper.ChangeToStringEnumerator(this.m_innerEnumerable.GetEnumerator());

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      private class ChangeToStringEnumerator : IEnumerator<string>, IDisposable, IEnumerator
      {
        private IEnumerator<Change> m_innerEnumerator;

        public ChangeToStringEnumerator(IEnumerator<Change> innerEnumerator) => this.m_innerEnumerator = innerEnumerator;

        public string Current => this.m_innerEnumerator.Current.Item.ServerItem;

        public void Dispose()
        {
          if (this.m_innerEnumerator == null)
            return;
          this.m_innerEnumerator.Dispose();
          this.m_innerEnumerator = (IEnumerator<Change>) null;
        }

        public bool MoveNext() => this.m_innerEnumerator.MoveNext();

        object IEnumerator.Current => (object) this.Current;

        void IEnumerator.Reset() => this.m_innerEnumerator.Reset();
      }
    }
  }
}
