// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.AdminEngagement.WebApi.SubjectPermissions
// Assembly: Microsoft.TeamFoundation.AdminEngagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7DC52CF-50E9-4106-90C5-0EC98E836C71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.AdminEngagement.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.AdminEngagement.WebApi
{
  [DataContract]
  public class SubjectPermissions
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public List<SubjectPermission> subjectPermissions { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public IdentityDescriptor identityDescriptor { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool CanEditPermissions { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool UserHasReadAccess { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsProjectScope { get; set; }
  }
}
