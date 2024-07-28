// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PerformanceCounters.ClientPerformanceCounterScope
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.PerformanceCounters
{
  internal sealed class ClientPerformanceCounterScope
  {
    public ClientPerformanceCounterScope()
      : this(ClientPerformanceCounterLevel.Endpoint)
    {
    }

    public ClientPerformanceCounterScope(ClientPerformanceCounterLevel level)
      : this(level, ClientPerformanceCounterDetail.Verbose)
    {
    }

    public ClientPerformanceCounterScope(
      ClientPerformanceCounterLevel level,
      ClientPerformanceCounterDetail detail)
    {
      this.Level = level;
      this.Detail = detail;
    }

    public ClientPerformanceCounterLevel Level { get; internal set; }

    public ClientPerformanceCounterDetail Detail { get; internal set; }

    public static bool IsValid(
      ClientPerformanceCounterLevel level,
      ClientPerformanceCounterDetail detail)
    {
      return Enum.IsDefined(typeof (ClientPerformanceCounterLevel), (object) level) & Enum.IsDefined(typeof (ClientPerformanceCounterDetail), (object) detail);
    }
  }
}
