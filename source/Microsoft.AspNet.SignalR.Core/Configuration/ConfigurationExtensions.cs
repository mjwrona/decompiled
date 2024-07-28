// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Configuration.ConfigurationExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Configuration
{
  internal static class ConfigurationExtensions
  {
    public const int MissedTimeoutsBeforeClientReconnect = 2;
    public const int HeartBeatsPerKeepAlive = 2;
    public const int HeartBeatsPerDisconnectTimeout = 6;

    public static TimeSpan? KeepAliveTimeout(this IConfigurationManager config) => config.KeepAlive.HasValue ? new TimeSpan?(TimeSpan.FromTicks(config.KeepAlive.Value.Ticks * 2L)) : new TimeSpan?();

    public static TimeSpan HeartbeatInterval(this IConfigurationManager config) => config.KeepAlive.HasValue ? TimeSpan.FromTicks(config.KeepAlive.Value.Ticks / 2L) : TimeSpan.FromTicks(config.DisconnectTimeout.Ticks / 6L);

    public static TimeSpan TopicTtl(this IConfigurationManager config)
    {
      TimeSpan timeSpan = config.KeepAliveTimeout() ?? TimeSpan.Zero;
      return TimeSpan.FromTicks((config.DisconnectTimeout.Ticks + timeSpan.Ticks) * 2L);
    }
  }
}
