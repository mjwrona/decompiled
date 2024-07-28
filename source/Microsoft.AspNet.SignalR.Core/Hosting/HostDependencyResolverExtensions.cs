// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hosting.HostDependencyResolverExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Hosting
{
  public static class HostDependencyResolverExtensions
  {
    public static void InitializeHost(
      this IDependencyResolver resolver,
      string instanceName,
      CancellationToken hostShutdownToken)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentNullException(nameof (instanceName));
      if (!MonoUtility.IsRunningMono)
        resolver.InitializePerformanceCounters(instanceName, hostShutdownToken);
      resolver.InitializeResolverDispose(hostShutdownToken);
    }

    private static void InitializePerformanceCounters(
      this IDependencyResolver resolver,
      string instanceName,
      CancellationToken hostShutdownToken)
    {
      resolver.Resolve<IPerformanceCounterManager>()?.Initialize(instanceName, hostShutdownToken);
    }

    private static void InitializeResolverDispose(
      this IDependencyResolver resolver,
      CancellationToken hostShutdownToken)
    {
      hostShutdownToken.SafeRegister((Action<object>) (state => ((IDisposable) state).Dispose()), (object) resolver);
    }
  }
}
