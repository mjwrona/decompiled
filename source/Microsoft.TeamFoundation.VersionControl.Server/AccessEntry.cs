// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AccessEntry
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class AccessEntry
  {
    internal int allowBits;
    internal int denyBits;
    internal int allowInheritedBits;
    internal int denyInheritedBits;
    private string m_identityName;
    private string m_displayName;
    private List<string> m_deny;
    private List<string> m_allow;
    private List<string> m_denyInherited;
    private List<string> m_allowInherited;

    public AccessEntry()
    {
    }

    public AccessEntry(
      int allow,
      int deny,
      int allowInherited,
      int denyInherited,
      string identityName)
      : this(allow, deny, allowInherited, denyInherited, identityName, (string) null)
    {
    }

    public AccessEntry(
      int allow,
      int deny,
      int allowInherited,
      int denyInherited,
      string identityName,
      string displayName)
    {
      this.allowBits = AccessEntry.GetLegacyPermission(allow);
      this.denyBits = AccessEntry.GetLegacyPermission(deny);
      this.allowInheritedBits = AccessEntry.GetLegacyPermission(allowInherited);
      this.denyInheritedBits = AccessEntry.GetLegacyPermission(denyInherited);
      this.m_identityName = identityName;
      this.m_displayName = displayName;
    }

    public List<string> Allow
    {
      get => this.m_allow;
      set => this.m_allow = value;
    }

    public List<string> Deny
    {
      get => this.m_deny;
      set => this.m_deny = value;
    }

    public List<string> AllowInherited
    {
      get => this.m_allowInherited;
      set => this.m_allowInherited = value;
    }

    public List<string> DenyInherited
    {
      get => this.m_denyInherited;
      set => this.m_denyInherited = value;
    }

    [XmlAttribute("ident")]
    public string IdentityName
    {
      get => this.m_identityName;
      set => this.m_identityName = value;
    }

    [XmlAttribute("disp")]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    public static int CompareIdentity(AccessEntry entry1, AccessEntry entry2) => VssStringComparer.IdentityName.Compare(entry1.IdentityName, entry2.IdentityName);

    internal static int GetLegacyPermission(int permission)
    {
      permission &= -12289;
      return permission;
    }
  }
}
