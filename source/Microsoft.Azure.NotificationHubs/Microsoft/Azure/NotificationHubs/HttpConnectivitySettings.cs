// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.HttpConnectivitySettings
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs
{
  internal class HttpConnectivitySettings
  {
    public HttpConnectivitySettings(HttpConnectivityMode option) => this.Mode = option;

    public HttpConnectivityMode Mode { get; private set; }
  }
}
