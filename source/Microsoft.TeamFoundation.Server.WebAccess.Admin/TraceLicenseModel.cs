// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.TraceLicenseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class TraceLicenseModel
  {
    public TeamFoundationIdentity Identity { get; set; }

    public bool IsDefault { get; set; }

    public ILicenseType[] Licenses { get; set; }

    public List<KeyValuePair<TeamFoundationIdentity, ILicenseType>> AffectingIdentities { get; set; }
  }
}
