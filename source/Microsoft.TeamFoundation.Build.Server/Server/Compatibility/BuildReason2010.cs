// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildReason2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientEnum(ClientVisibility.Internal)]
  [Flags]
  [XmlType("BuildReason")]
  public enum BuildReason2010
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
    Triggered = CheckInShelveset | UserCreated | ScheduleForced | Schedule | BatchedCI | IndividualCI | Manual, // 0x000000BF
    All = Triggered | ValidateShelveset, // 0x000000FF
  }
}
