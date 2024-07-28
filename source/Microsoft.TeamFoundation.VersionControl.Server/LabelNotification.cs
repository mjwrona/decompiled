// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LabelNotification
  {
    private string m_computerName;
    private string m_ownerName;
    private string m_userName;
    private string m_workspaceName;
    private List<LabelResult> m_labelResults;

    internal LabelNotification(Microsoft.VisualStudio.Services.Identity.Identity user, List<LabelResult> labelResults)
    {
      this.User = user;
      this.m_userName = IdentityHelper.GetDomainUserName(user);
      this.m_labelResults = labelResults;
    }

    internal LabelNotification(
      Microsoft.VisualStudio.Services.Identity.Identity user,
      string workspaceName,
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      string computerName,
      List<LabelResult> labelResults)
    {
      this.Owner = owner;
      this.User = user;
      this.m_computerName = computerName;
      this.m_labelResults = labelResults;
      this.m_ownerName = owner == null ? (string) null : IdentityHelper.GetDomainUserName(owner);
      this.m_userName = user == null ? (string) null : IdentityHelper.GetDomainUserName(user);
      this.m_workspaceName = workspaceName;
    }

    public IEnumerable<LabelResult> AffectedLabels => (IEnumerable<LabelResult>) this.m_labelResults;

    public Microsoft.VisualStudio.Services.Identity.Identity Owner { get; private set; }

    [Obsolete("Use the Owner property instead")]
    [Browsable(false)]
    public string OwnerName => this.m_ownerName;

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; private set; }

    [Obsolete("Use the User property instead")]
    [Browsable(false)]
    public string UserName => this.m_userName;

    public string WorkspaceName => this.m_workspaceName;
  }
}
