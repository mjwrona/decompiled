// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamProjectFolderOptions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class TeamProjectFolderOptions
  {
    private string m_teamProject;
    private string m_sourceProject;
    private string m_comment;
    private bool m_exclusiveCheckout;
    private TeamProjectFolderPermission[] m_permissions;
    private bool m_keepExistingPermissions;
    private CheckinNoteFieldDefinition[] m_checkinNoteDefinition;
    private bool m_getLatestOnCheckout;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string SourceProject
    {
      get => this.m_sourceProject;
      set => this.m_sourceProject = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute("exc")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool ExclusiveCheckout
    {
      get => this.m_exclusiveCheckout;
      set => this.m_exclusiveCheckout = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public TeamProjectFolderPermission[] Permissions
    {
      get => this.m_permissions;
      set => this.m_permissions = value;
    }

    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool KeepExistingPermissions
    {
      get => this.m_keepExistingPermissions;
      set => this.m_keepExistingPermissions = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public CheckinNoteFieldDefinition[] CheckinNoteDefinition
    {
      get => this.m_checkinNoteDefinition;
      set => this.m_checkinNoteDefinition = value;
    }

    [XmlAttribute("gloc")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool GetLatestOnCheckout
    {
      get => this.m_getLatestOnCheckout;
      set => this.m_getLatestOnCheckout = value;
    }
  }
}
