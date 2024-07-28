// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.UserTenantData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models
{
  [DataContract]
  public class UserTenantData
  {
    [DataMember(EmitDefaultValue = false)]
    public List<TenantData> UserTenants { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UserContext { get; set; }
  }
}
