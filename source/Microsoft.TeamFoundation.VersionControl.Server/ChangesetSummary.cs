// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetSummary
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class ChangesetSummary
  {
    private string m_comment;
    private string m_owner;
    private string m_ownerDisplayName;
    private string m_committer;
    private string m_committerDisplayName;
    private DateTime m_creationDate;
    private int m_changesetId;
    internal Guid ownerId;
    internal Guid committerId;

    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    public string Owner
    {
      get => this.m_owner;
      set => this.m_owner = value;
    }

    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    public string Committer
    {
      get => this.m_committer;
      set => this.m_committer = value;
    }

    public string CommitterDisplayName
    {
      get => this.m_committerDisplayName;
      set => this.m_committerDisplayName = value;
    }

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    internal void LookupDisplayNames(
      VersionControlRequestContext versionControlRequestContext)
    {
      string identityName1;
      string displayName1;
      versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, this.ownerId, out identityName1, out displayName1);
      this.Owner = identityName1;
      this.OwnerDisplayName = displayName1;
      if (this.committerId == this.ownerId)
      {
        this.Committer = this.Owner;
        this.CommitterDisplayName = this.OwnerDisplayName;
      }
      else
      {
        string identityName2;
        string displayName2;
        versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, this.committerId, out identityName2, out displayName2);
        this.Committer = identityName2;
        this.CommitterDisplayName = displayName2;
      }
    }
  }
}
