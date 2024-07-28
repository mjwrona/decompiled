// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ShelvesetNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ShelvesetNotification
  {
    private string m_shelvesetName;
    private string m_shelvesetOwnerName;
    private IEnumerable<string> m_shelvedItems;
    private string m_comment;
    private CheckinNote m_checkinNote;
    private string m_policyOverrideComment;
    private VersionControlLink[] m_links;
    private string m_workspaceName;
    private string m_workspaceOwnerName;
    private string m_computerName;
    private ShelvesetNotificationType m_shelvesetNotificationType;

    internal ShelvesetNotification(
      string shelvesetName,
      Microsoft.VisualStudio.Services.Identity.Identity shelvesetOwner,
      IEnumerable<string> shelvedItems,
      string comment,
      CheckinNote checkinNote,
      string policyOverrideComment,
      VersionControlLink[] links,
      string workspaceName,
      string workspaceOwnerName,
      string computerName,
      ShelvesetNotificationType shelvesetNotificationType)
    {
      this.m_shelvesetName = shelvesetName;
      this.m_shelvesetOwnerName = shelvesetOwner == null ? (string) null : IdentityHelper.GetDomainUserName(shelvesetOwner);
      this.ShelvesetOwner = shelvesetOwner;
      this.m_shelvedItems = shelvedItems;
      this.m_comment = comment;
      this.m_checkinNote = checkinNote;
      this.m_policyOverrideComment = policyOverrideComment;
      this.m_links = links;
      this.m_workspaceName = workspaceName;
      this.m_workspaceOwnerName = workspaceOwnerName;
      this.m_computerName = computerName;
      this.m_shelvesetNotificationType = shelvesetNotificationType;
    }

    public string ShelvesetName => this.m_shelvesetName;

    [Obsolete("Use the ShelvesetOwner property instead")]
    [Browsable(false)]
    public string ShelvesetOwnerName => this.m_shelvesetOwnerName;

    public Microsoft.VisualStudio.Services.Identity.Identity ShelvesetOwner { get; private set; }

    public string Comment => this.m_comment;

    public CheckinNote CheckinNote => this.m_checkinNote;

    public string PolicyOverrideComment => this.m_policyOverrideComment;

    public ReadOnlyCollection<VersionControlLink> Links => new ReadOnlyCollection<VersionControlLink>((IList<VersionControlLink>) this.m_links);

    public string WorkspaceName => this.m_workspaceName;

    public string WorkspaceOwnerName => this.m_workspaceOwnerName;

    public string ComputerName => this.m_computerName;

    public IEnumerable<string> ShelvedItems => this.m_shelvedItems;

    public ShelvesetNotificationType ShelvesetNotificationType
    {
      get => this.m_shelvesetNotificationType;
      internal set => this.m_shelvesetNotificationType = value;
    }
  }
}
