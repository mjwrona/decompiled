// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationAadSettingsData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class OrganizationAadSettingsData
  {
    [DataMember(EmitDefaultValue = false)]
    public TenantData OrgnizationTenantData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public UserData User { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OrganizationName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid OrganizationId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SpsSignoutUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasModifyPermissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsMicrosoftTenant { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool SupportServicePrincipals { get; set; }
  }
}
