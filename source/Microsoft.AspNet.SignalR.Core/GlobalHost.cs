// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.GlobalHost
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;

namespace Microsoft.AspNet.SignalR
{
  public static class GlobalHost
  {
    private static readonly Lazy<IDependencyResolver> _defaultResolver = new Lazy<IDependencyResolver>((Func<IDependencyResolver>) (() => (IDependencyResolver) new DefaultDependencyResolver()));
    private static IDependencyResolver _resolver;

    public static IDependencyResolver DependencyResolver
    {
      get => GlobalHost._resolver ?? GlobalHost._defaultResolver.Value;
      set => GlobalHost._resolver = value;
    }

    public static IConfigurationManager Configuration => GlobalHost.DependencyResolver.Resolve<IConfigurationManager>();

    public static IConnectionManager ConnectionManager => GlobalHost.DependencyResolver.Resolve<IConnectionManager>();

    public static ITraceManager TraceManager => GlobalHost.DependencyResolver.Resolve<ITraceManager>();

    public static IHubPipeline HubPipeline => GlobalHost.DependencyResolver.Resolve<IHubPipeline>();
  }
}
