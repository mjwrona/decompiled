// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.PushPackageCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class PushPackageCiData : PackagingCiData
  {
    public PushPackageCiData(
      IVssRequestContext requestContext,
      IProtocol protocol,
      string protocolVersion,
      FeedCore feed,
      string packageName,
      string packageVersion,
      long packageSize,
      bool fromUpstream = false,
      string storageType = null)
      : base(requestContext, "PushPackage", protocol, feed)
    {
      this.CiData.Add("PackageName", packageName);
      this.CiData.Add("PackageVersion", packageVersion);
      this.CiData.Add("ProtocolVersion", protocolVersion);
      this.CiData.Add("PackageSizeBytes", (double) packageSize);
      this.CiData.Add("FromUpstream", fromUpstream);
      if (storageType == null)
        return;
      this.CiData.Add("StorageType", storageType);
    }
  }
}
