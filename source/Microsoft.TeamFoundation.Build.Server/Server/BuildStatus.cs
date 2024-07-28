// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildStatus
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum BuildStatus
  {
    None = 0,
    InProgress = 1,
    Succeeded = 2,
    PartiallySucceeded = 4,
    Failed = 8,
    Stopped = 16, // 0x00000010
    NotStarted = 32, // 0x00000020
    All = NotStarted | Stopped | Failed | PartiallySucceeded | Succeeded | InProgress, // 0x0000003F
  }
}
