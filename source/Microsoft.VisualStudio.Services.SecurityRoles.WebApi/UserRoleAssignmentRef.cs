// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.WebApi.UserRoleAssignmentRef
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74D9BC5A-4C7E-4BC3-9331-A0A75718A098
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.SecurityRoles.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.SecurityRoles.WebApi
{
  [DataContract]
  public class UserRoleAssignmentRef
  {
    [DataMember(Name = "userId")]
    public Guid UserId { get; set; }

    [DataMember(Name = "uniqueName")]
    public string UniqueName { get; set; }

    [DataMember(Name = "roleName")]
    public string RoleName { get; set; }
  }
}
