// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ProtocolVersioningHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs
{
  internal static class ProtocolVersioningHelper
  {
    private const string NetServices = "netservices";
    public const string ServiceBusNetService = "servicebus";
    public const string UsageService = "usage";
    public const string VersionYear = "2009";
    public const string VersionMonth = "05";
    public const string ConfigNamePostfix = "2009.05";
    public const int CurrentVersionOrder = 1;
    private const string NameSpacePrefix = "http://schemas.microsoft.com/netservices/2009/05";
    public const string ServiceBusNameSpace = "http://schemas.microsoft.com/netservices/2009/05/servicebus";
    public const string UsageNameSpace = "http://schemas.microsoft.com/netservices/2009/05/usage";
  }
}
