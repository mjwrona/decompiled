// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.WebApi.SecurityRole
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74D9BC5A-4C7E-4BC3-9331-A0A75718A098
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.SecurityRoles.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.SecurityRoles.WebApi
{
  [DataContract]
  public class SecurityRole
  {
    public SecurityRole(
      string displayName,
      string name,
      int allowPermissions,
      int denyPermissions,
      string scopeId,
      string description = "")
    {
      this.DisplayName = displayName;
      this.Name = name;
      this.AllowPermissions = allowPermissions;
      this.DenyPermissions = denyPermissions;
      this.Description = description;
      this.Scope = scopeId;
    }

    [DataMember(Name = "displayName")]
    public string DisplayName { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "allowPermissions")]
    public int AllowPermissions { get; set; }

    [DataMember(Name = "denyPermissions")]
    public int DenyPermissions { get; set; }

    [DataMember(Name = "identifier")]
    public string Identifier => string.Format("{0}.{1}", (object) this.Scope, (object) this.Name);

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "scope")]
    public string Scope { get; set; }

    public static bool Equals(SecurityRole x, SecurityRole y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      if (x == y)
        return true;
      return string.Equals(x.Scope, y.Scope, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }
  }
}
