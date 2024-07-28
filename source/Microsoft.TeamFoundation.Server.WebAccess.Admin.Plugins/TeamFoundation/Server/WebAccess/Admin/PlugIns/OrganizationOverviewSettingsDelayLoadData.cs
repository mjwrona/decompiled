// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationOverviewSettingsDelayLoadData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class OrganizationOverviewSettingsDelayLoadData
  {
    [DataMember(EmitDefaultValue = false)]
    public bool IsOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasDeletePermissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HasModifyPermissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Owner CurrentOwner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid CurrentUserId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public OrganizationTakeover OrganizationTakeover { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<TimeZoneData> AllTimeZones { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ShowDomainMigration { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool DisableDomainMigration { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool DevOpsDomainUrls { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TargetDomainUrl { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string AvatarUrl { get; set; }
  }
}
