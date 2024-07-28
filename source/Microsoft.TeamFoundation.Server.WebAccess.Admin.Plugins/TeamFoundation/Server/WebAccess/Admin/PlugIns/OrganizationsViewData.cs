// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationsViewData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  [DataContract]
  public class OrganizationsViewData
  {
    [DataMember(EmitDefaultValue = false)]
    public IList<OrganizationData> OwnerAccounts { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<OrganizationData> MemberAccounts { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Guid> OwnerDeletedAccountIds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Guid> MemberDeletedAccountIds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsAadUser { get; set; }
  }
}
