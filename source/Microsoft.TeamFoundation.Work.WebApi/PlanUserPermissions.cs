// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.PlanUserPermissions
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [Flags]
  [DataContract]
  public enum PlanUserPermissions
  {
    None = 0,
    View = 1,
    Edit = 2,
    Delete = 4,
    Manage = 8,
    AllPermissions = Manage | Delete | Edit | View, // 0x0000000F
  }
}
