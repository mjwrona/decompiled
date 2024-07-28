// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PermissionUpdates
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [Serializable]
  public class PermissionUpdates
  {
    public List<PermissionUpdate> Updates { get; set; }

    public Guid PermissionSetId { get; set; }

    public string PermissionSetToken { get; set; }

    public Guid TeamFoundationId { get; set; }

    public string TokenDisplayName { get; set; }

    public string DescriptorIdentityType { get; set; }

    public string DescriptorIdentifier { get; set; }

    public bool IsRemovingIdentity { get; set; }
  }
}
