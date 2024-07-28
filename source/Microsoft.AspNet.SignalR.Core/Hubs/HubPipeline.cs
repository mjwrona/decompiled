// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubPipeline
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubPipeline : IHubPipeline, IHubPipelineInvoker
  {
    private readonly Stack<IHubPipelineModule> _modules;
    private readonly Lazy<HubPipeline.ComposedPipeline> _pipeline;

    public HubPipeline()
    {
      this._modules = new Stack<IHubPipelineModule>();
      this._pipeline = new Lazy<HubPipeline.ComposedPipeline>((Func<HubPipeline.ComposedPipeline>) (() => new HubPipeline.ComposedPipeline(this._modules)));
    }

    public IHubPipeline AddModule(IHubPipelineModule pipelineModule)
    {
      if (this._pipeline.IsValueCreated)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UnableToAddModulePiplineAlreadyInvoked));
      this._modules.Push(pipelineModule);
      return (IHubPipeline) this;
    }

    private HubPipeline.ComposedPipeline Pipeline => this._pipeline.Value;

    public Task<object> Invoke(IHubIncomingInvokerContext context) => this.Pipeline.Invoke(context);

    public Task Connect(IHub hub) => this.Pipeline.Connect(hub);

    public Task Reconnect(IHub hub) => this.Pipeline.Reconnect(hub);

    public Task Disconnect(IHub hub, bool stopCalled) => this.Pipeline.Disconnect(hub, stopCalled);

    public bool AuthorizeConnect(HubDescriptor hubDescriptor, IRequest request) => this.Pipeline.AuthorizeConnect(hubDescriptor, request);

    public IList<string> RejoiningGroups(
      HubDescriptor hubDescriptor,
      IRequest request,
      IList<string> groups)
    {
      return this.Pipeline.RejoiningGroups(hubDescriptor, request, groups);
    }

    public Task Send(IHubOutgoingInvokerContext context) => this.Pipeline.Send(context);

    private class ComposedPipeline
    {
      public Func<IHubIncomingInvokerContext, Task<object>> Invoke;
      public Func<IHub, Task> Connect;
      public Func<IHub, Task> Reconnect;
      public Func<IHub, bool, Task> Disconnect;
      public Func<HubDescriptor, IRequest, bool> AuthorizeConnect;
      public Func<HubDescriptor, IRequest, IList<string>, IList<string>> RejoiningGroups;
      public Func<IHubOutgoingInvokerContext, Task> Send;

      public ComposedPipeline(Stack<IHubPipelineModule> modules)
      {
        this.Invoke = HubPipeline.ComposedPipeline.Compose<Func<IHubIncomingInvokerContext, Task<object>>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<IHubIncomingInvokerContext, Task<object>>, Func<IHubIncomingInvokerContext, Task<object>>>) ((m, f) => m.BuildIncoming(f)))(new Func<IHubIncomingInvokerContext, Task<object>>(HubDispatcher.Incoming));
        this.Connect = HubPipeline.ComposedPipeline.Compose<Func<IHub, Task>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<IHub, Task>, Func<IHub, Task>>) ((m, f) => m.BuildConnect(f)))(new Func<IHub, Task>(HubDispatcher.Connect));
        this.Reconnect = HubPipeline.ComposedPipeline.Compose<Func<IHub, Task>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<IHub, Task>, Func<IHub, Task>>) ((m, f) => m.BuildReconnect(f)))(new Func<IHub, Task>(HubDispatcher.Reconnect));
        this.Disconnect = HubPipeline.ComposedPipeline.Compose<Func<IHub, bool, Task>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<IHub, bool, Task>, Func<IHub, bool, Task>>) ((m, f) => m.BuildDisconnect(f)))(new Func<IHub, bool, Task>(HubDispatcher.Disconnect));
        this.AuthorizeConnect = HubPipeline.ComposedPipeline.Compose<Func<HubDescriptor, IRequest, bool>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<HubDescriptor, IRequest, bool>, Func<HubDescriptor, IRequest, bool>>) ((m, f) => m.BuildAuthorizeConnect(f)))((Func<HubDescriptor, IRequest, bool>) ((h, r) => true));
        this.RejoiningGroups = HubPipeline.ComposedPipeline.Compose<Func<HubDescriptor, IRequest, IList<string>, IList<string>>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<HubDescriptor, IRequest, IList<string>, IList<string>>, Func<HubDescriptor, IRequest, IList<string>, IList<string>>>) ((m, f) => m.BuildRejoiningGroups(f)))((Func<HubDescriptor, IRequest, IList<string>, IList<string>>) ((h, r, g) => g));
        this.Send = HubPipeline.ComposedPipeline.Compose<Func<IHubOutgoingInvokerContext, Task>>((IEnumerable<IHubPipelineModule>) modules, (Func<IHubPipelineModule, Func<IHubOutgoingInvokerContext, Task>, Func<IHubOutgoingInvokerContext, Task>>) ((m, f) => m.BuildOutgoing(f)))(new Func<IHubOutgoingInvokerContext, Task>(HubDispatcher.Outgoing));
      }

      private static Func<T, T> Compose<T>(
        IEnumerable<IHubPipelineModule> modules,
        Func<IHubPipelineModule, T, T> method)
      {
        return modules.Aggregate<IHubPipelineModule, Func<T, T>>((Func<T, T>) (x => x), (Func<Func<T, T>, IHubPipelineModule, Func<T, T>>) ((a, b) => (Func<T, T>) (x => method(b, a(x)))));
      }
    }
  }
}
