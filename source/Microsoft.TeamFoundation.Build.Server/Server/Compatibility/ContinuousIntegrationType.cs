// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ContinuousIntegrationType
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [Flags]
  public enum ContinuousIntegrationType
  {
    None = 1,
    Individual = 2,
    Batch = 4,
    Schedule = 8,
    ScheduleForced = 16, // 0x00000010
    Gated = 32, // 0x00000020
    All = Gated | ScheduleForced | Schedule | Batch | Individual | None, // 0x0000003F
  }
}
