// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.DefaultContextPropertyManager
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class DefaultContextPropertyManager : 
    TelemetryDisposableObject,
    IContextPropertyManager,
    IDisposable
  {
    private readonly List<IPropertyProvider> propertyProviders;
    private readonly CancellationTokenSource cancellationTokenSource;

    public DefaultContextPropertyManager(IEnumerable<IPropertyProvider> propertyProviders)
    {
      propertyProviders.RequiresArgumentNotNull<IEnumerable<IPropertyProvider>>(nameof (propertyProviders));
      this.cancellationTokenSource = new CancellationTokenSource();
      this.propertyProviders = propertyProviders.ToList<IPropertyProvider>();
    }

    public void AddPropertyProvider(IPropertyProvider propertyProvider)
    {
      propertyProvider.RequiresArgumentNotNull<IPropertyProvider>(nameof (propertyProvider));
      this.propertyProviders.Add(propertyProvider);
    }

    public void AddDefaultContextProperties(TelemetryContext telemetryContext)
    {
      telemetryContext.RequiresArgumentNotNull<TelemetryContext>(nameof (telemetryContext));
      List<KeyValuePair<string, object>> sharedProperties = new List<KeyValuePair<string, object>>();
      this.propertyProviders.ForEach((Action<IPropertyProvider>) (x => x.AddSharedProperties(sharedProperties, telemetryContext)));
      foreach (KeyValuePair<string, object> keyValuePair in sharedProperties)
        telemetryContext.SharedProperties[keyValuePair.Key] = keyValuePair.Value;
    }

    public void PostDefaultContextProperties(TelemetryContext telemetryContext)
    {
      CancellationToken token = this.cancellationTokenSource.Token;
      Task.Run((Action) (() => this.propertyProviders.ForEach((Action<IPropertyProvider>) (x => x.PostProperties(telemetryContext, token)))), token);
    }

    protected override void DisposeManagedResources() => this.cancellationTokenSource.Cancel();

    public void AddRealtimeDefaultContextProperties(TelemetryContext telemetryContext)
    {
      telemetryContext.RequiresArgumentNotNull<TelemetryContext>(nameof (telemetryContext));
      List<KeyValuePair<string, Func<object>>> sharedProperties = new List<KeyValuePair<string, Func<object>>>();
      foreach (IRealtimePropertyProvider propertyProvider in this.propertyProviders.OfType<IRealtimePropertyProvider>())
        propertyProvider.AddRealtimeSharedProperties(sharedProperties, telemetryContext);
      foreach (KeyValuePair<string, Func<object>> keyValuePair in sharedProperties)
        telemetryContext.RealtimeSharedProperties[keyValuePair.Key] = keyValuePair.Value;
    }
  }
}
