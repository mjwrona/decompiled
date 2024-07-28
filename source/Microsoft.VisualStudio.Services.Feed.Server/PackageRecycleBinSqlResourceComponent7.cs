// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent7
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRecycleBinSqlResourceComponent7 : PackageRecycleBinSqlResourceComponent6
  {
    public override DateTime ScheduleImmediatePermanentDeletion(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, int batchSize = 1000)
    {
      this.PrepareStoredProcedure("Feed.prc_ScheduleImmediatePermanentDeletion");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => reader.GetDateTime(0))));
        return resultCollection.GetCurrent<DateTime>().First<DateTime>();
      }
    }

    public override IEnumerable<ProtocolPackageVersionIdentity> GetTopPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deletedBefore,
      int top = 1000)
    {
      this.PrepareStoredProcedure("Feed.prc_GetTopRecycleBinPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      this.BindInt("@top", top);
      this.BindDateTime("@deletedBefore", deletedBefore);
      return this.ReadTopRecycleBinPackageVersions();
    }
  }
}
