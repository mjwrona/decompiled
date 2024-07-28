// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConnectionStateListener
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ConnectionStateListener : IConnectionStateListener
  {
    private readonly IAddressResolver addressResolver;

    public ConnectionStateListener(IAddressResolver addressResolver) => this.addressResolver = addressResolver;

    public void OnConnectionEvent(
      ConnectionEvent connectionEvent,
      DateTime eventTime,
      ServerKey serverKey)
    {
      DefaultTrace.TraceInformation("OnConnectionEvent fired, connectionEvent :{0}, eventTime: {1}, serverKey: {2}", (object) connectionEvent, (object) eventTime, (object) serverKey.ToString());
      if (connectionEvent != ConnectionEvent.ReadEof && connectionEvent != ConnectionEvent.ReadFailure)
        return;
      Task.Run((Func<Task>) (async () => await this.addressResolver.UpdateAsync(serverKey))).ContinueWith((Action<Task>) (task => DefaultTrace.TraceWarning("AddressCache update failed: {0}", (object) task.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
    }
  }
}
