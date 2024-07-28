// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExtendedAccessControlListData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ExtendedAccessControlListData
  {
    private List<AccessControlEntryData> m_permissions = new List<AccessControlEntryData>();

    public ExtendedAccessControlListData() => this.m_permissions = new List<AccessControlEntryData>();

    public ExtendedAccessControlListData(
      string token,
      bool inherit,
      List<AccessControlEntryData> permissions)
    {
      this.Token = token;
      this.InheritPermissions = inherit;
      this.m_permissions = permissions;
    }

    public string Token { get; set; }

    public bool InheritPermissions { get; set; }

    public List<AccessControlEntryData> Permissions => this.m_permissions;

    public bool IsEditable { get; set; }

    public AccessControlListMetadata[] Metadata { get; set; }
  }
}
