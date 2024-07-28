// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanPermissionsBitFlags
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using System;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  [Flags]
  public enum PlanPermissionsBitFlags
  {
    View = 1,
    Edit = 2,
    Delete = 4,
    Manage = 8,
    AllPermissions = Manage | Delete | Edit | View, // 0x0000000F
  }
}
