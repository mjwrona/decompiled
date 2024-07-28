// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CdnRegistryConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CdnRegistryConstants
  {
    public const string CDNSettingsRoot = "/Configuration/WebAccess/CDN";
    public const string CDNEnabled = "/Configuration/WebAccess/CDN/Enabled";
    public const string CDNEndpointUrl = "/Configuration/WebAccess/CDN/EndpointUrl";
    public const string RegionalCDNEndpointUrls = "/Configuration/WebAccess/CDN/RegionalEndpointUrls";
    public const string CDNStorageContainerName = "/Configuration/WebAccess/CDN/StorageContainerName";
    public const string CDNRemoteBlobProviderName = "/Configuration/WebAccess/CDN/RemoteBlobProvider";
  }
}
