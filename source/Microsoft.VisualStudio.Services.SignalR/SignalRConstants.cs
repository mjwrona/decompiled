// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.SignalRConstants
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public static class SignalRConstants
  {
    internal const string AlwaysPublishFeatureFlag = "VisualStudio.Services.SignalR.AlwaysPublish";
    internal const string FeatureDisconnectOnlyIfKeepAliveIsLost = "VisualStudio.Services.SignalR.DisconnectOnlyIfKeepAliveIsLost";
    internal static readonly Guid VssSignalRConnectionCleanupJobId = new Guid("9426ACB1-515A-426F-9151-CCD8F7FA0427");
    internal const string ConnectionRouteName = "signalr";
    public const string ConnectionRouteUrl = "signalr/{*operation}";
    internal const string HubsRouteName = "signalr.hubs";
    public const string HubsRouteUrl = "signalr/{signalrObject}";
    public const string ServerProcolQueryStringName = "serverProtocol";
    public const string UrlSigningProtocol = "urlSigning";
  }
}
