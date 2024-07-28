// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.DefaultDependencyResolver
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.AspNet.SignalR.Transports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.AspNet.SignalR
{
  public class DefaultDependencyResolver : IDependencyResolver, IDisposable
  {
    private readonly Dictionary<Type, IList<Func<object>>> _resolvers = new Dictionary<Type, IList<Func<object>>>();
    private readonly HashSet<IDisposable> _trackedDisposables = new HashSet<IDisposable>();
    private int _disposed;

    public DefaultDependencyResolver()
    {
      this.RegisterDefaultServices();
      this.RegisterHubExtensions();
    }

    private void RegisterDefaultServices()
    {
      Lazy<TraceManager> traceManager = new Lazy<TraceManager>((Func<TraceManager>) (() => new TraceManager()));
      this.Register(typeof (ITraceManager), (Func<object>) (() => (object) traceManager.Value));
      Lazy<IMessageBus> newMessageBus = new Lazy<IMessageBus>((Func<IMessageBus>) (() => (IMessageBus) new MessageBus((IDependencyResolver) this)));
      this.Register(typeof (IMessageBus), (Func<object>) (() => (object) newMessageBus.Value));
      Lazy<IStringMinifier> stringMinifier = new Lazy<IStringMinifier>((Func<IStringMinifier>) (() => (IStringMinifier) new StringMinifier()));
      this.Register(typeof (IStringMinifier), (Func<object>) (() => (object) stringMinifier.Value));
      Lazy<JsonSerializer> jsonSerializer = new Lazy<JsonSerializer>();
      this.Register(typeof (JsonSerializer), (Func<object>) (() => (object) jsonSerializer.Value));
      Lazy<TransportManager> transportManager = new Lazy<TransportManager>((Func<TransportManager>) (() => new TransportManager((IDependencyResolver) this)));
      this.Register(typeof (ITransportManager), (Func<object>) (() => (object) transportManager.Value));
      DefaultConfigurationManager configurationManager = new DefaultConfigurationManager();
      this.Register(typeof (IConfigurationManager), (Func<object>) (() => (object) configurationManager));
      Lazy<TransportHeartbeat> transportHeartbeat = new Lazy<TransportHeartbeat>((Func<TransportHeartbeat>) (() => new TransportHeartbeat((IDependencyResolver) this)));
      this.Register(typeof (ITransportHeartbeat), (Func<object>) (() => (object) transportHeartbeat.Value));
      Lazy<ConnectionManager> connectionManager = new Lazy<ConnectionManager>((Func<ConnectionManager>) (() => new ConnectionManager((IDependencyResolver) this)));
      this.Register(typeof (IConnectionManager), (Func<object>) (() => (object) connectionManager.Value));
      Lazy<AckHandler> ackHandler = new Lazy<AckHandler>();
      this.Register(typeof (IAckHandler), (Func<object>) (() => (object) ackHandler.Value));
      Lazy<AckSubscriber> serverMessageHandler = new Lazy<AckSubscriber>((Func<AckSubscriber>) (() => new AckSubscriber((IDependencyResolver) this)));
      this.Register(typeof (AckSubscriber), (Func<object>) (() => (object) serverMessageHandler.Value));
      Lazy<PerformanceCounterManager> perfCounterWriter = new Lazy<PerformanceCounterManager>((Func<PerformanceCounterManager>) (() => new PerformanceCounterManager(this)));
      this.Register(typeof (IPerformanceCounterManager), (Func<object>) (() => (object) perfCounterWriter.Value));
      PrincipalUserIdProvider userIdProvider = new PrincipalUserIdProvider();
      this.Register(typeof (IUserIdProvider), (Func<object>) (() => (object) userIdProvider));
      MemoryPool pool = new MemoryPool();
      this.Register(typeof (IMemoryPool), (Func<object>) (() => (object) pool));
    }

    private void RegisterHubExtensions()
    {
      Lazy<ReflectedMethodDescriptorProvider> methodDescriptorProvider = new Lazy<ReflectedMethodDescriptorProvider>();
      this.Register(typeof (IMethodDescriptorProvider), (Func<object>) (() => (object) methodDescriptorProvider.Value));
      Lazy<ReflectedHubDescriptorProvider> hubDescriptorProvider = new Lazy<ReflectedHubDescriptorProvider>((Func<ReflectedHubDescriptorProvider>) (() => new ReflectedHubDescriptorProvider((IDependencyResolver) this)));
      this.Register(typeof (IHubDescriptorProvider), (Func<object>) (() => (object) hubDescriptorProvider.Value));
      Lazy<DefaultParameterResolver> parameterBinder = new Lazy<DefaultParameterResolver>();
      this.Register(typeof (IParameterResolver), (Func<object>) (() => (object) parameterBinder.Value));
      Lazy<DefaultHubActivator> activator = new Lazy<DefaultHubActivator>((Func<DefaultHubActivator>) (() => new DefaultHubActivator((IDependencyResolver) this)));
      this.Register(typeof (IHubActivator), (Func<object>) (() => (object) activator.Value));
      Lazy<DefaultHubManager> hubManager = new Lazy<DefaultHubManager>((Func<DefaultHubManager>) (() => new DefaultHubManager((IDependencyResolver) this)));
      this.Register(typeof (IHubManager), (Func<object>) (() => (object) hubManager.Value));
      Lazy<DefaultJavaScriptProxyGenerator> proxyGenerator = new Lazy<DefaultJavaScriptProxyGenerator>((Func<DefaultJavaScriptProxyGenerator>) (() => new DefaultJavaScriptProxyGenerator((IDependencyResolver) this)));
      this.Register(typeof (IJavaScriptProxyGenerator), (Func<object>) (() => (object) proxyGenerator.Value));
      Lazy<HubRequestParser> requestParser = new Lazy<HubRequestParser>();
      this.Register(typeof (IHubRequestParser), (Func<object>) (() => (object) requestParser.Value));
      Lazy<DefaultAssemblyLocator> assemblyLocator = new Lazy<DefaultAssemblyLocator>((Func<DefaultAssemblyLocator>) (() => new DefaultAssemblyLocator()));
      this.Register(typeof (IAssemblyLocator), (Func<object>) (() => (object) assemblyLocator.Value));
      Lazy<IHubPipeline> dispatcher = new Lazy<IHubPipeline>((Func<IHubPipeline>) (() => new HubPipeline().AddModule((IHubPipelineModule) new AuthorizeModule())));
      this.Register(typeof (IHubPipeline), (Func<object>) (() => (object) dispatcher.Value));
      this.Register(typeof (IHubPipelineInvoker), (Func<object>) (() => (object) dispatcher.Value));
    }

    public virtual object GetService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      IList<Func<object>> funcList;
      if (!this._resolvers.TryGetValue(serviceType, out funcList))
        return (object) null;
      if (funcList.Count == 0)
        return (object) null;
      return funcList.Count <= 1 ? this.Track(funcList[0]) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_MultipleActivatorsAreaRegisteredCallGetServices, new object[1]
      {
        (object) serviceType.FullName
      }));
    }

    public virtual IEnumerable<object> GetServices(Type serviceType)
    {
      IList<Func<object>> source;
      if (!this._resolvers.TryGetValue(serviceType, out source))
        return (IEnumerable<object>) null;
      return source.Count == 0 ? (IEnumerable<object>) null : (IEnumerable<object>) source.Select<Func<object>, object>(new Func<Func<object>, object>(this.Track)).ToList<object>();
    }

    public virtual void Register(Type serviceType, Func<object> activator)
    {
      IList<Func<object>> funcList;
      if (!this._resolvers.TryGetValue(serviceType, out funcList))
      {
        funcList = (IList<Func<object>>) new List<Func<object>>();
        this._resolvers.Add(serviceType, funcList);
      }
      else
        funcList.Clear();
      funcList.Add(activator);
    }

    public virtual void Register(Type serviceType, IEnumerable<Func<object>> activators)
    {
      if (activators == null)
        throw new ArgumentNullException(nameof (activators));
      IList<Func<object>> funcList;
      if (!this._resolvers.TryGetValue(serviceType, out funcList))
      {
        funcList = (IList<Func<object>>) new List<Func<object>>();
        this._resolvers.Add(serviceType, funcList);
      }
      else
        funcList.Clear();
      foreach (Func<object> activator in activators)
        funcList.Add(activator);
    }

    private object Track(Func<object> creator)
    {
      object obj = creator();
      if (this._disposed == 0 && obj is IDisposable disposable && !(disposable is IUntrackedDisposable))
      {
        lock (this._trackedDisposables)
        {
          if (this._disposed == 0)
            this._trackedDisposables.Add(disposable);
        }
      }
      return obj;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || Interlocked.Exchange(ref this._disposed, 1) != 0)
        return;
      lock (this._trackedDisposables)
      {
        foreach (IDisposable trackedDisposable in this._trackedDisposables)
          trackedDisposable.Dispose();
        this._trackedDisposables.Clear();
      }
    }

    public void Dispose() => this.Dispose(true);
  }
}
