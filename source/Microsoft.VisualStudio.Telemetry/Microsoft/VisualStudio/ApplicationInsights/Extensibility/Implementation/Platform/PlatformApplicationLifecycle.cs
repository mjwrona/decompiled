// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform.PlatformApplicationLifecycle
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform
{
  internal class PlatformApplicationLifecycle : IApplicationLifecycle
  {
    private static IApplicationLifecycle provider;

    public event Action<object, object> Started;

    public event EventHandler<ApplicationStoppingEventArgs> Stopping;

    public static IApplicationLifecycle Provider
    {
      get => LazyInitializer.EnsureInitialized<IApplicationLifecycle>(ref PlatformApplicationLifecycle.provider, new Func<IApplicationLifecycle>(PlatformApplicationLifecycle.CreateDefaultProvider));
      set => PlatformApplicationLifecycle.provider = value;
    }

    internal void Initialize() => AppDomain.CurrentDomain.DomainUnload += (EventHandler) ((sender, e) => this.OnStopping(new ApplicationStoppingEventArgs((Func<Func<Task>, Task>) (function => function()))));

    private static IApplicationLifecycle CreateDefaultProvider()
    {
      PlatformApplicationLifecycle defaultProvider = new PlatformApplicationLifecycle();
      defaultProvider.Initialize();
      return (IApplicationLifecycle) defaultProvider;
    }

    private void OnStarted(object eventArgs)
    {
      Action<object, object> started = this.Started;
      if (started == null)
        return;
      started((object) this, eventArgs);
    }

    private void OnStopping(ApplicationStoppingEventArgs eventArgs)
    {
      EventHandler<ApplicationStoppingEventArgs> stopping = this.Stopping;
      if (stopping == null)
        return;
      stopping((object) this, eventArgs);
    }
  }
}
