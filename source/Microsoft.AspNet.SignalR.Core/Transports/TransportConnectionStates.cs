// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.TransportConnectionStates
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Transports
{
  [Flags]
  public enum TransportConnectionStates
  {
    None = 0,
    Added = 1,
    Removed = 2,
    Replaced = 4,
    QueueDrained = 8,
    HttpRequestEnded = 16, // 0x00000010
    Disconnected = 32, // 0x00000020
    Aborted = 64, // 0x00000040
    Disposed = 65536, // 0x00010000
  }
}
