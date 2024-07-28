// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SignalR.TfsSignalRConstants
// Assembly: Microsoft.TeamFoundation.Server.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0378695-D47C-46CB-A501-9188B19EA4AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.SignalR.dll

namespace Microsoft.TeamFoundation.Server.SignalR
{
  public static class TfsSignalRConstants
  {
    public const string ConnectionProjectRouteName = "tfs.signalr.project";
    public const string ConnectionProjectRouteUrl = "_apis/{project}/signalr/{*operation}";
    public const string HubsProjectRouteName = "tfs.signalr.hubs.project";
    public const string HubsProjectRouteUrl = "_apis/{project}/signalr/{signalrObject}";
    public const string WebAccessSignalRSeparateAppPool = "WebAccess.SignalR.AppPool";
    public const string SignalRAppPoolName = "_signalr";
  }
}
