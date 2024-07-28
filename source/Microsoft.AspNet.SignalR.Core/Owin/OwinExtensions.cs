// Decompiled with JetBrains decompiler
// Type: Owin.OwinExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Owin.Middleware;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Owin
{
  public static class OwinExtensions
  {
    public static IAppBuilder MapSignalR(this IAppBuilder builder) => builder.MapSignalR(new HubConfiguration());

    public static IAppBuilder MapSignalR(this IAppBuilder builder, HubConfiguration configuration) => builder.MapSignalR("/signalr", configuration);

    public static IAppBuilder MapSignalR(
      this IAppBuilder builder,
      string path,
      HubConfiguration configuration)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      return builder.Map(path, (Action<IAppBuilder>) (subApp => subApp.RunSignalR(configuration)));
    }

    public static void RunSignalR(this IAppBuilder builder) => builder.RunSignalR(new HubConfiguration());

    public static void RunSignalR(this IAppBuilder builder, HubConfiguration configuration) => builder.UseSignalRMiddleware<HubDispatcherMiddleware>((object) configuration);

    public static IAppBuilder MapSignalR<TConnection>(this IAppBuilder builder, string path) where TConnection : PersistentConnection => builder.MapSignalR(path, typeof (TConnection), new ConnectionConfiguration());

    public static IAppBuilder MapSignalR<TConnection>(
      this IAppBuilder builder,
      string path,
      ConnectionConfiguration configuration)
      where TConnection : PersistentConnection
    {
      return builder.MapSignalR(path, typeof (TConnection), configuration);
    }

    public static IAppBuilder MapSignalR(
      this IAppBuilder builder,
      string path,
      Type connectionType,
      ConnectionConfiguration configuration)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      return builder.Map(path, (Action<IAppBuilder>) (subApp => subApp.RunSignalR(connectionType, configuration)));
    }

    public static void RunSignalR<TConnection>(this IAppBuilder builder) where TConnection : PersistentConnection => builder.RunSignalR<TConnection>(new ConnectionConfiguration());

    public static void RunSignalR<TConnection>(
      this IAppBuilder builder,
      ConnectionConfiguration configuration)
      where TConnection : PersistentConnection
    {
      builder.RunSignalR(typeof (TConnection), configuration);
    }

    public static void RunSignalR(
      this IAppBuilder builder,
      Type connectionType,
      ConnectionConfiguration configuration)
    {
      builder.UseSignalRMiddleware<PersistentConnectionMiddleware>((object) connectionType, (object) configuration);
    }

    private static IAppBuilder UseSignalRMiddleware<T>(
      this IAppBuilder builder,
      params object[] args)
    {
      OwinExtensions.EnsureValidCulture();
      SignatureConversions.AddConversions(builder);
      if (args.Length != 0)
      {
        if (!(args[args.Length - 1] is ConnectionConfiguration connectionConfiguration))
          throw new ArgumentException(Resources.Error_NoConfiguration);
        IDependencyResolver resolver = connectionConfiguration.Resolver;
        if (resolver == null)
          throw new ArgumentException(Resources.Error_NoDependencyResolver);
        IDictionary<string, object> properties = builder.Properties;
        CancellationToken shutdownToken = properties.GetShutdownToken();
        string str = properties.GetAppInstanceName() ?? Guid.NewGuid().ToString();
        IDataProtectionProvider provider = builder.GetDataProtectionProvider();
        IProtectedData protectedData;
        if (provider == null && MonoUtility.IsRunningMono)
        {
          protectedData = (IProtectedData) new DefaultProtectedData();
        }
        else
        {
          if (provider == null)
            provider = (IDataProtectionProvider) new DpapiDataProtectionProvider(str);
          protectedData = (IProtectedData) new DataProtectionProviderProtectedData(provider);
        }
        resolver.Register(typeof (IProtectedData), (Func<object>) (() => (object) protectedData));
        TextWriter traceOutput = properties.GetTraceOutput();
        if (traceOutput != null)
        {
          TraceManager traceManager = new TraceManager(new TextWriterTraceListener(traceOutput));
          resolver.Register(typeof (ITraceManager), (Func<object>) (() => (object) traceManager));
        }
        IEnumerable<Assembly> referenceAssemblies = properties.GetReferenceAssemblies();
        if (referenceAssemblies != null)
        {
          EnumerableOfAssemblyLocator assemblyLocator = new EnumerableOfAssemblyLocator(referenceAssemblies);
          resolver.Register(typeof (IAssemblyLocator), (Func<object>) (() => (object) assemblyLocator));
        }
        resolver.InitializeHost(str, shutdownToken);
      }
      builder.Use((object) typeof (T), args);
      builder.UseStageMarker(PipelineStage.PostAuthorize);
      return builder;
    }

    private static void EnsureValidCulture()
    {
      CultureInfo cultureInfo = CultureInfo.CurrentCulture;
      while (!cultureInfo.Equals((object) CultureInfo.InvariantCulture))
        cultureInfo = cultureInfo.Parent;
      if (cultureInfo == CultureInfo.InvariantCulture)
        return;
      Thread currentThread = Thread.CurrentThread;
      currentThread.CurrentCulture = CultureInfo.GetCultureInfo(currentThread.CurrentCulture.Name);
      currentThread.CurrentUICulture = CultureInfo.GetCultureInfo(currentThread.CurrentUICulture.Name);
    }
  }
}
