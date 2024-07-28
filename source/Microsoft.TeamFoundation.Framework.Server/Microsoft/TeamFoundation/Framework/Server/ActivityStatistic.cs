// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityStatistic
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class ActivityStatistic
  {
    public int StatisticId { get; internal set; }

    public int ProcessId { get; internal set; }

    public int ThreadId { get; internal set; }

    public Guid ActivityId { get; internal set; }

    public double RelativeTimestamp { get; internal set; }

    public string EventName { get; set; }

    public string ActivityMessage { get; set; }

    public Guid DataId { get; set; }
  }
}
