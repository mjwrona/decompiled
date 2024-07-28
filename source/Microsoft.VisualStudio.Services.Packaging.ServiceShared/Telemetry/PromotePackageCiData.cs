// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.PromotePackageCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class PromotePackageCiData : PackagingCiData
  {
    public PromotePackageCiData(
      IVssRequestContext requestContext,
      IProtocol protocol,
      FeedCore feed,
      FeedView promotedView,
      FeedView[] existingViews,
      string packageName,
      string packageVersion,
      long packageSize,
      string packageSource = "feed")
      : base(requestContext, "PromotePackage", protocol, feed)
    {
      string str = ((IEnumerable<FeedView>) existingViews).Select<FeedView, FeedView>((Func<FeedView, FeedView>) (v => new FeedView()
      {
        Id = v.Id,
        Name = v.Name,
        Type = v.Type
      })).Serialize<IEnumerable<FeedView>>();
      this.CiData.Add("PackageName", packageName);
      this.CiData.Add("PackageVersion", packageVersion);
      this.CiData.Add("PackageSizeBytes", (double) packageSize);
      this.CiData.Add("PackageSource", packageSource);
      this.CiData.Add("PromotedViewId", (object) promotedView.Id);
      this.CiData.Add("PromotedViewName", promotedView.Name);
      this.CiData.Add("PromotedViewType", promotedView.Type.ToString());
      this.CiData.Add("ExistingReleaseViews", str);
    }
  }
}
