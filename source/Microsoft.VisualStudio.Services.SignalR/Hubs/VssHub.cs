// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Hubs.VssHub
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR.Hubs
{
  public abstract class VssHub : Hub, IVssHub, IHub, IUntrackedDisposable, IDisposable
  {
    public virtual bool RequiresHostContext => true;

    public IVssRequestContext VssRequestContext { get; set; }

    public override async Task OnConnected()
    {
      await base.OnConnected();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventName", nameof (OnConnected));
      this.PublishTelemetry(properties);
    }

    public override async Task OnDisconnected(bool stopCalled)
    {
      await base.OnDisconnected(stopCalled);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventName", nameof (OnDisconnected));
      properties.Add("IsClientTimeOut", !stopCalled);
      this.PublishTelemetry(properties);
    }

    public override async Task OnReconnected()
    {
      await base.OnReconnected();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventName", nameof (OnReconnected));
      this.PublishTelemetry(properties);
    }

    private void PublishTelemetry(CustomerIntelligenceData properties)
    {
      properties.Add("ConnectionId", this.Context.ConnectionId);
      this.VssRequestContext.GetService<CustomerIntelligenceService>().Publish(this.VssRequestContext, "SignalR", "Connection", properties);
    }

    IHubCallerConnectionContext<object> IHub.get_Clients() => this.Clients;

    void IHub.set_Clients(IHubCallerConnectionContext<object> value) => this.Clients = value;
  }
}
