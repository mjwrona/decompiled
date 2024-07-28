// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.QueueStatus
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum QueueStatus
  {
    None = 0,
    InProgress = 1,
    Retry = 2,
    Queued = 4,
    Postponed = 8,
    Completed = 16, // 0x00000010
    Canceled = 32, // 0x00000020
    All = Canceled | Completed | Postponed | Queued | Retry | InProgress, // 0x0000003F
  }
}
