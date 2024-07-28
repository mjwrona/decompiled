// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Messaging.VssScaleoutConfiguration
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR.Messaging;
using System;

namespace Microsoft.VisualStudio.Services.SignalR.Messaging
{
  public class VssScaleoutConfiguration : ScaleoutConfiguration
  {
    public static readonly TimeSpan DefaultOpenRetryDelay = TimeSpan.FromSeconds(10.0);

    public VssScaleoutConfiguration() => this.OpenRetryDelay = VssScaleoutConfiguration.DefaultOpenRetryDelay;

    public TimeSpan OpenRetryDelay { get; set; }

    public bool SupportsClientConnections { get; set; }
  }
}
