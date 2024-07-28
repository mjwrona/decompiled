// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders.PoliciesData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders
{
  [DataContract]
  public class PoliciesData
  {
    [DataMember(EmitDefaultValue = false)]
    public PolicyCollection Policies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int PermissionBits { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsOrganizationActivated { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<string> InvertedPolicies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsMicrosoftTenant { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsGuestUser { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HostId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestAccessUrl { get; set; }
  }
}
