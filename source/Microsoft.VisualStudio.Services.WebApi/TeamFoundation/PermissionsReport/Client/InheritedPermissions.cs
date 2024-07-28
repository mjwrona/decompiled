// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.InheritedPermissions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public class InheritedPermissions
  {
    public InheritedPermissions()
    {
    }

    public InheritedPermissions(string inheritedFrom, string permissionValue)
    {
      this.InheritedFrom = inheritedFrom;
      this.PermissionValue = permissionValue;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string InheritedFrom { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string PermissionValue { get; set; }
  }
}
