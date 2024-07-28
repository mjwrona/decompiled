// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildReason
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum BuildReason
  {
    None = 0,
    Manual = 1,
    IndividualCI = 2,
    BatchedCI = 4,
    Schedule = 8,
    ScheduleForced = 16, // 0x00000010
    UserCreated = 32, // 0x00000020
    ValidateShelveset = 64, // 0x00000040
    CheckInShelveset = 128, // 0x00000080
    BuildCompletion = 256, // 0x00000100
    ResourceTrigger = 512, // 0x00000200
    Triggered = BuildCompletion | CheckInShelveset | UserCreated | ScheduleForced | Schedule | BatchedCI | IndividualCI | Manual, // 0x000001BF
    All = Triggered | ValidateShelveset, // 0x000001FF
  }
}
