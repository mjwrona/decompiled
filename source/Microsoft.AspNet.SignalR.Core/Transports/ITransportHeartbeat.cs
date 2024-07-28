// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.ITransportHeartbeat
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Transports
{
  public interface ITransportHeartbeat
  {
    ITrackingConnection AddOrUpdateConnection(ITrackingConnection connection);

    void MarkConnection(ITrackingConnection connection);

    void RemoveConnection(ITrackingConnection connection);

    IList<ITrackingConnection> GetConnections();
  }
}
