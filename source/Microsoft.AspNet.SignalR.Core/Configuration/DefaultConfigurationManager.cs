// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Configuration.DefaultConfigurationManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Configuration
{
  public class DefaultConfigurationManager : IConfigurationManager
  {
    private static readonly TimeSpan _minimumKeepAlive = TimeSpan.FromSeconds(2.0);
    private const int _minimumKeepAlivesPerDisconnectTimeout = 3;
    internal const int DefaultMaxScaleoutMappingsPerStream = 65535;
    private static readonly TimeSpan _minimumDisconnectTimeout = TimeSpan.FromTicks(DefaultConfigurationManager._minimumKeepAlive.Ticks * 3L);
    private bool _keepAliveConfigured;
    private TimeSpan? _keepAlive;
    private TimeSpan _disconnectTimeout;
    private int _maxScaleoutMappingPerStream;

    public DefaultConfigurationManager()
    {
      this.ConnectionTimeout = TimeSpan.FromSeconds(110.0);
      this.DisconnectTimeout = TimeSpan.FromSeconds(30.0);
      this.DefaultMessageBufferSize = 1000;
      this.MaxIncomingWebSocketMessageSize = new int?(65536);
      this.TransportConnectTimeout = TimeSpan.FromSeconds(5.0);
      this.LongPollDelay = TimeSpan.Zero;
      this.MaxScaleoutMappingsPerStream = (int) ushort.MaxValue;
    }

    public TimeSpan ConnectionTimeout { get; set; }

    public TimeSpan DisconnectTimeout
    {
      get => this._disconnectTimeout;
      set
      {
        if (value < DefaultConfigurationManager._minimumDisconnectTimeout)
          throw new ArgumentOutOfRangeException(nameof (value), Resources.Error_DisconnectTimeoutMustBeAtLeastSixSeconds);
        if (this._keepAliveConfigured)
          throw new InvalidOperationException(Resources.Error_DisconnectTimeoutCannotBeConfiguredAfterKeepAlive);
        this._disconnectTimeout = value;
        this._keepAlive = new TimeSpan?(TimeSpan.FromTicks(this._disconnectTimeout.Ticks / 3L));
      }
    }

    public TimeSpan? KeepAlive
    {
      get => this._keepAlive;
      set
      {
        TimeSpan? nullable1 = value;
        TimeSpan minimumKeepAlive = DefaultConfigurationManager._minimumKeepAlive;
        if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() < minimumKeepAlive ? 1 : 0) : 0) != 0)
          throw new ArgumentOutOfRangeException(nameof (value), Resources.Error_KeepAliveMustBeGreaterThanTwoSeconds);
        TimeSpan? nullable2 = value;
        TimeSpan timeSpan = TimeSpan.FromTicks(this._disconnectTimeout.Ticks / 3L);
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > timeSpan ? 1 : 0) : 0) != 0)
          throw new ArgumentOutOfRangeException(nameof (value), Resources.Error_KeepAliveMustBeNoMoreThanAThirdOfTheDisconnectTimeout);
        this._keepAlive = value;
        this._keepAliveConfigured = true;
      }
    }

    public int DefaultMessageBufferSize { get; set; }

    public int? MaxIncomingWebSocketMessageSize { get; set; }

    public TimeSpan TransportConnectTimeout { get; set; }

    public TimeSpan LongPollDelay { get; set; }

    public int MaxScaleoutMappingsPerStream
    {
      get => this._maxScaleoutMappingPerStream;
      set => this._maxScaleoutMappingPerStream = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof (value), Resources.Error_MaxScaleoutMappingsPerStreamMustBeNonNegative);
    }
  }
}
