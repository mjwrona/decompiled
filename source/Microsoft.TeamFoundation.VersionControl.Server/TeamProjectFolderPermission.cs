// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamProjectFolderPermission
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class TeamProjectFolderPermission
  {
    internal Guid IdentityId;
    internal VersionedItemPermissions allowBits;
    internal VersionedItemPermissions denyBits;
    private string m_identityName;
    private string[] m_allowPermission;
    private string[] m_denyPermission;

    [XmlAttribute("ident")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string IdentityName
    {
      get => this.m_identityName;
      set => this.m_identityName = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string[] AllowPermission
    {
      get => this.m_allowPermission;
      set => this.m_allowPermission = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string[] DenyPermission
    {
      get => this.m_denyPermission;
      set => this.m_denyPermission = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "TeamProjectPermission(Name:\"{0}\" Allow:{1} Deny:{2})", (object) this.IdentityName, (object) this.AllowPermission, (object) this.DenyPermission);
  }
}
