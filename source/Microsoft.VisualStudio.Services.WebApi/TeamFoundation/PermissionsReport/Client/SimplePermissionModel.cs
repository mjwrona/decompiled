// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.SimplePermissionModel
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public class SimplePermissionModel
  {
    public SimplePermissionModel()
    {
    }

    public SimplePermissionModel(
      string permissionName,
      string effectivePermission,
      bool isPermissionInherited,
      IEnumerable<Microsoft.TeamFoundation.PermissionsReport.Client.InheritedPermissions> inheritedPermissions = null)
    {
      this.PermissionName = permissionName;
      this.EffectivePermission = effectivePermission;
      this.IsPermissionInherited = isPermissionInherited;
      this.InheritedPermissions = inheritedPermissions;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string PermissionName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string EffectivePermission { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsPermissionInherited { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<Microsoft.TeamFoundation.PermissionsReport.Client.InheritedPermissions> InheritedPermissions { get; set; }
  }
}
