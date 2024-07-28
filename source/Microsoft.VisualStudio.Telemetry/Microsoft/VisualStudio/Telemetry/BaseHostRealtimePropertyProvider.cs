// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.BaseHostRealtimePropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class BaseHostRealtimePropertyProvider : 
    IRealtimePropertyProvider,
    IPropertyProvider
  {
    private IHostInformationProvider hostInformationProvider;

    public BaseHostRealtimePropertyProvider(IHostInformationProvider hostInformationProvider)
    {
      hostInformationProvider.RequiresArgumentNotNull<IHostInformationProvider>(nameof (hostInformationProvider));
      this.hostInformationProvider = hostInformationProvider;
    }

    public void AddRealtimeSharedProperties(
      List<KeyValuePair<string, Func<object>>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, Func<object>>("VS.Core.IsDebuggerAttached", (Func<object>) (() => this.hostInformationProvider.IsDebuggerAttached ? (object) true : (object) null)));
    }

    public abstract void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext);

    public virtual void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
    }
  }
}
