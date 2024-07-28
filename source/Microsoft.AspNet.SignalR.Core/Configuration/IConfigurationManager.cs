// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Configuration.IConfigurationManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Configuration
{
  public interface IConfigurationManager
  {
    TimeSpan TransportConnectTimeout { get; set; }

    TimeSpan ConnectionTimeout { get; set; }

    TimeSpan DisconnectTimeout { get; set; }

    TimeSpan? KeepAlive { get; set; }

    int DefaultMessageBufferSize { get; set; }

    int? MaxIncomingWebSocketMessageSize { get; set; }

    TimeSpan LongPollDelay { get; set; }

    int MaxScaleoutMappingsPerStream { get; set; }
  }
}
