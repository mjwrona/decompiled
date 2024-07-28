// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.WebApi.RoleAssignment
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74D9BC5A-4C7E-4BC3-9331-A0A75718A098
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.SecurityRoles.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.SecurityRoles.WebApi
{
  [DataContract]
  public class RoleAssignment
  {
    [DataMember(Name = "identity")]
    public IdentityRef Identity { get; set; }

    [DataMember(Name = "role")]
    public SecurityRole Role { get; set; }

    [DataMember(Name = "access")]
    public RoleAccess Access { get; set; }

    [DataMember(Name = "accessDisplayName")]
    public string AccessDisplayName
    {
      get
      {
        switch (this.Access)
        {
          case RoleAccess.Assigned:
            return SecurityRolesResources.AccessAssigned();
          case RoleAccess.Inherited:
            return SecurityRolesResources.AccessInherited();
          default:
            return "";
        }
      }
    }
  }
}
