// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildStatus2010
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
  [XmlType("BuildStatus")]
  public enum BuildStatus2010
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
