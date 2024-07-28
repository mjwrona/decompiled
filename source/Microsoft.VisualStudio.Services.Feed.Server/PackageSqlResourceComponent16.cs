// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent16
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent16 : PackageSqlResourceComponent15
  {
    public override IEnumerable<ProtocolPackageVersionIdentity> GetVersionsExceedingRetentionLimitV2(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      int countLimit,
      IEnumerable<Guid> packageIds,
      int daysToKeepRecentlyDownloadedPackages,
      DateTime packageMetricsEnabledTimestamp)
    {
      this.PrepareStoredProcedure("Feed.prc_GetVersionsExceedingRetentionLimitV2");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindInt("@countLimit", countLimit);
      this.BindGuidTable("@packageIds", packageIds);
      this.BindInt("@daysToKeepRecentlyDownloadedPackages", daysToKeepRecentlyDownloadedPackages);
      this.BindDateTime("@packageMetricsEnabledTimestamp", packageMetricsEnabledTimestamp);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProtocolPackageVersionIdentity>((ObjectBinder<ProtocolPackageVersionIdentity>) new ProtocolPackageVersionIdentityBinder());
        return (IEnumerable<ProtocolPackageVersionIdentity>) resultCollection.GetCurrent<ProtocolPackageVersionIdentity>().Items;
      }
    }
  }
}
