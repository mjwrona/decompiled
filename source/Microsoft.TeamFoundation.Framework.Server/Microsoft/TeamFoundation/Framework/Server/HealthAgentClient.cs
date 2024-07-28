// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HealthAgentClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HealthAgentClient : ClientBase<IHealthAgentClient>, IHealthAgentClient, IDisposable
  {
    public static readonly Uri ServerAddress = new Uri("net.pipe://localhost/TeamFoundationServer/VssHealthAgent");

    internal HealthAgentClient()
      : base((Binding) new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport), new EndpointAddress(HealthAgentClient.ServerAddress, Array.Empty<AddressHeader>()))
    {
    }

    public void Dispose()
    {
      try
      {
        this.Close();
      }
      catch (CommunicationException ex)
      {
        this.Abort();
      }
      catch (TimeoutException ex)
      {
        this.Abort();
      }
    }

    public void RequestReset(int processId) => this.Channel.RequestReset(processId);

    public void SendHeartbeat(int processId) => this.Channel.SendHeartbeat(processId);

    public void ResetSession() => this.Channel.ResetSession();
  }
}
