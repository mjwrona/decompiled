// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DefinitionTriggerType
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum DefinitionTriggerType
  {
    None = 1,
    ContinuousIntegration = 2,
    BatchedContinuousIntegration = 4,
    Schedule = 8,
    ScheduleForced = 16, // 0x00000010
    GatedCheckIn = 32, // 0x00000020
    BatchedGatedCheckIn = 64, // 0x00000040
    BuildCompletion = 128, // 0x00000080
    All = BuildCompletion | BatchedGatedCheckIn | GatedCheckIn | ScheduleForced | Schedule | BatchedContinuousIntegration | ContinuousIntegration | None, // 0x000000FF
  }
}
