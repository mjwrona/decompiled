// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.AdminEngagement.WebApi.SubjectPermission
// Assembly: Microsoft.TeamFoundation.AdminEngagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7DC52CF-50E9-4106-90C5-0EC98E836C71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.AdminEngagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.AdminEngagement.WebApi
{
  [DataContract]
  public class SubjectPermission
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid NamespaceId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Token { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public int Bit { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool CanEdit { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public PermissionValue EffectivePermissionValue { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public PermissionValue ExplicitPermissionValue { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool InheritDenyOverride { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string PermissionDisplayString { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool IsPermissionInherited { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public bool IsPermissionGrantedBySystem { get; set; }
  }
}
