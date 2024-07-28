// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.QueueStatus2010
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
  [XmlType("QueueStatus")]
  public enum QueueStatus2010
  {
    None = 0,
    InProgress = 1,
    Queued = 2,
    Postponed = 4,
    Completed = 8,
    Canceled = 16, // 0x00000010
    All = Canceled | Completed | Postponed | Queued | InProgress, // 0x0000001F
  }
}
