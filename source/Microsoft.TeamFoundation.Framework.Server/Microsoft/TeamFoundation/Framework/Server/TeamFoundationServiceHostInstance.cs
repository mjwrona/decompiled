// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostInstance
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationServiceHostInstance
  {
    public Guid HostId { get; set; }

    public Guid ProcessId { get; set; }

    public DateTime StartTime { get; set; }

    public TeamFoundationServiceHostStatus Status { get; set; }

    public TeamFoundationServiceHostProperties HostProperties { get; set; }

    public TeamFoundationServiceHostProcess Process { get; set; }
  }
}
