// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouter.SmartRouterTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server.SmartRouter
{
  internal static class SmartRouterTracePoints
  {
    private const int Start = 34005500;
    public const string Area = "SmartRouter";
    public const string Layer = "Server";

    internal enum IpValidatorTracePoints
    {
      SpoofingDetected = 34005501, // 0x0206E1FD
      TrustedIp = 34005502, // 0x0206E1FE
      HeadersMissing = 34005503, // 0x0206E1FF
      InvalidLocalHost = 34005504, // 0x0206E200
      InvalidUserHost = 34005505, // 0x0206E201
      CacheRetrieved = 34005506, // 0x0206E202
      CacheNotFound = 34005507, // 0x0206E203
      CacheAdded = 34005508, // 0x0206E204
    }

    internal enum IpAddressUtilityTracePoints
    {
      GetLocalhostIpAddress = 34005510, // 0x0206E206
      GetLocalhostIpAddressNone = 34005511, // 0x0206E207
      GetInstanceNetworkMetadataException = 34005512, // 0x0206E208
    }
  }
}
