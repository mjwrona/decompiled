// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.CacheUpstreamMetadataCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class CacheUpstreamMetadataCiData : PackagingCiData
  {
    public CacheUpstreamMetadataCiData(
      IVssRequestContext requestContext,
      IProtocol protocol,
      FeedCore feed,
      RefreshPackageResult refreshPackageResult)
      : base(requestContext, "CacheUpstreamMetadata", protocol, feed)
    {
      this.CiData.Add("PackageName", TelemetryHelpers.WrapStringWithNonEmailGDPRException(refreshPackageResult.PackageName));
      this.CiData.Add("UpstreamsRefreshNeeded", refreshPackageResult.RefreshNeeded);
      this.CiData.Add("UpstreamsVersionsCur", (object) refreshPackageResult.CurUpstreamVersions);
      this.CiData.Add("UpstreamsVersionsPrev", (object) refreshPackageResult.PrevUpstreamVersions);
      this.CiData.Add("LocalVersions", (object) refreshPackageResult.LocalVersions);
      this.CiData.Add("ShadowedVersions", (object) refreshPackageResult.ShadowedVersions);
    }
  }
}
