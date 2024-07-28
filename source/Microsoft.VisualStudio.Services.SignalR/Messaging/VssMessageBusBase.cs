// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Messaging.VssMessageBusBase
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.SignalR.Messaging
{
  public abstract class VssMessageBusBase : ScaleoutMessageBus, IVssMessageBus, IMessageBus
  {
    private Task m_startupTask;
    private CancellationTokenSource m_startupTokenSource;
    private IVssDeploymentServiceHost m_serviceHost;

    protected VssMessageBusBase(
      IVssDeploymentServiceHost serviceHost,
      IDependencyResolver dependencyResolver,
      ScaleoutConfiguration configuration)
      : base(dependencyResolver, configuration)
    {
      ArgumentUtility.CheckForNull<ScaleoutConfiguration>(configuration, nameof (configuration));
      this.m_serviceHost = serviceHost;
    }

    public IVssDeploymentServiceHost Host => this.m_serviceHost;

    public Task StartupTask => this.m_startupTask;

    protected string Area { get; } = "SignalR";

    protected virtual string Layer { get; } = "MessageBus";

    protected abstract void OpenStreams(IVssRequestContext requestContext);

    protected abstract void CloseStreams(IVssRequestContext requestContext);

    protected abstract Task Send(
      IVssRequestContext requestContext,
      int streamIndex,
      IList<Message> messages);

    public void Attach(IVssDeploymentServiceHost serviceHost)
    {
      ArgumentUtility.CheckForNull<IVssDeploymentServiceHost>(serviceHost, nameof (serviceHost));
      TeamFoundationTracingService.TraceRaw(10017060, TraceLevel.Info, this.Area, this.Layer, "Entering Attach");
      if (serviceHost == this.m_serviceHost)
      {
        TeamFoundationTracingService.TraceRaw(10017061, TraceLevel.Info, this.Area, this.Layer, "Skipping attach because the service host reference matches");
        TeamFoundationTracingService.TraceRaw(10017069, TraceLevel.Info, this.Area, this.Layer, "Leaving Attach");
      }
      else
      {
        if (this.m_serviceHost != null && this.m_serviceHost.Status.HasFlag((Enum) TeamFoundationServiceHostStatus.Started))
        {
          TeamFoundationTracingService.TraceRaw(10017062, TraceLevel.Info, this.Area, this.Layer, "Closing existing subscribers in preparation for re-attach");
          this.Stop();
        }
        TeamFoundationTracingService.TraceRaw(10017063, TraceLevel.Info, this.Area, this.Layer, "Opening new subscribers in preparation for re-attach");
        this.m_serviceHost = serviceHost;
        this.StartInternal();
        TeamFoundationTracingService.TraceRaw(10017069, TraceLevel.Info, this.Area, this.Layer, "Leaving Attach");
      }
    }

    public void Detach(IVssDeploymentServiceHost serviceHost)
    {
      ArgumentUtility.CheckForNull<IVssDeploymentServiceHost>(serviceHost, nameof (serviceHost));
      TeamFoundationTracingService.TraceRaw(10017070, TraceLevel.Info, this.Area, this.Layer, "Entering Detach");
      if (serviceHost != this.m_serviceHost)
      {
        TeamFoundationTracingService.TraceRaw(10017071, TraceLevel.Info, this.Area, this.Layer, "Skipping detach because the service host reference does not match");
        TeamFoundationTracingService.TraceRaw(10017079, TraceLevel.Info, this.Area, this.Layer, "Leaving Detach");
      }
      TeamFoundationTracingService.TraceRaw(10017072, TraceLevel.Info, this.Area, this.Layer, "Closing exising subscribers");
      this.Stop();
      this.m_serviceHost = (IVssDeploymentServiceHost) null;
      TeamFoundationTracingService.TraceRaw(10017079, TraceLevel.Info, this.Area, this.Layer, "Leaving Detach");
    }

    public Task Start()
    {
      if (this.m_startupTask != null)
        throw new InvalidOperationException();
      this.m_startupTokenSource = new CancellationTokenSource();
      this.m_startupTask = Task.Factory.StartNew((Action) (() => this.StartInternal(this.m_startupTokenSource.Token)), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
      return this.m_startupTask;
    }

    protected override sealed Task Send(int streamIndex, IList<Message> messages) => new VssRequestSynchronizationContext().RunAsync((Func<Task>) (async () =>
    {
      IVssDeploymentServiceHost serviceHost = this.m_serviceHost;
      if (serviceHost == null)
      {
        TeamFoundationTracingService.TraceRaw(10017007, TraceLevel.Error, this.Area, this.Layer, "Unable to process messages because no service host is currently attached");
      }
      else
      {
        using (IVssRequestContext requestContext = serviceHost.CreateSystemContext())
          await this.Send(requestContext, streamIndex, messages);
      }
    }));

    private void Stop()
    {
      if (!this.m_startupTask.IsCompleted)
      {
        this.m_startupTokenSource.Cancel();
        this.m_startupTask.Wait();
      }
      using (IVssRequestContext systemContext = this.m_serviceHost.CreateSystemContext(false))
        this.CloseStreams(systemContext);
    }

    private void StartInternal(CancellationToken cancellationToken = default (CancellationToken))
    {
      using (IVssRequestContext requestContext = this.m_serviceHost.CreateSystemContext())
      {
        using (cancellationToken.Register((Action) (() => requestContext.Cancel("MessageBus startup has been cancelled"))))
          this.OpenStreams(requestContext);
      }
    }
  }
}
