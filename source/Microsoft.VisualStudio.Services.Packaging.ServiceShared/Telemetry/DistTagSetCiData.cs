// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.DistTagSetCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class DistTagSetCiData : PackagingCiData
  {
    public DistTagSetCiData(
      IVssRequestContext requestContext,
      IProtocol protocol,
      string protocolVersion,
      FeedCore feed,
      string packageName,
      string packageVersion,
      string tag)
      : base(requestContext, "PushPackage", protocol, feed)
    {
      this.CiData.Add("PackageName", packageName);
      this.CiData.Add("PackageVersion", packageVersion);
      this.CiData.Add("ProtocolVersion", protocolVersion);
      this.CiData.Add("DistTag", tag);
    }
  }
}
