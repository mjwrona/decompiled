// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.ApplicationLifecycle
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform;
using System;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  public class ApplicationLifecycle : IApplicationLifecycle
  {
    private static readonly object SyncRoot = new object();
    private static ApplicationLifecycle service;
    private IApplicationLifecycle provider;

    protected ApplicationLifecycle()
    {
    }

    public event Action<object, object> Started;

    public event EventHandler<ApplicationStoppingEventArgs> Stopping;

    public static IApplicationLifecycle Service
    {
      get
      {
        if (ApplicationLifecycle.service == null)
        {
          lock (ApplicationLifecycle.SyncRoot)
          {
            if (ApplicationLifecycle.service == null)
              ApplicationLifecycle.service = new ApplicationLifecycle()
              {
                Provider = PlatformApplicationLifecycle.Provider
              };
          }
        }
        return (IApplicationLifecycle) ApplicationLifecycle.service;
      }
      internal set => ApplicationLifecycle.service = (ApplicationLifecycle) value;
    }

    private IApplicationLifecycle Provider
    {
      set
      {
        if (this.provider != null)
        {
          this.provider.Started -= new Action<object, object>(this.OnStarted);
          this.provider.Stopping -= new EventHandler<ApplicationStoppingEventArgs>(this.OnStopping);
        }
        this.provider = value;
        if (this.provider == null)
          return;
        this.provider.Started += new Action<object, object>(this.OnStarted);
        this.provider.Stopping += new EventHandler<ApplicationStoppingEventArgs>(this.OnStopping);
      }
    }

    public static void SetProvider(IApplicationLifecycle provider) => ((ApplicationLifecycle) ApplicationLifecycle.Service).Provider = provider != null ? provider : throw new ArgumentNullException("applicationLifecycleProvider");

    private void OnStarted(object sender, object eventArgs)
    {
      Action<object, object> started = this.Started;
      if (started == null)
        return;
      started((object) this, eventArgs);
    }

    private void OnStopping(object sender, ApplicationStoppingEventArgs eventArgs)
    {
      EventHandler<ApplicationStoppingEventArgs> stopping = this.Stopping;
      if (stopping == null)
        return;
      stopping((object) this, eventArgs);
    }
  }
}
