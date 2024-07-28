// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent5
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
  public class PackageRecycleBinSqlResourceComponent5 : PackageRecycleBinSqlResourceComponent4
  {
    public override IEnumerable<ProtocolPackageVersionIdentity> GetTopPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deletedBefored,
      int top = 1000)
    {
      this.PrepareStoredProcedure("Feed.prc_GetTopRecycleBinPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      this.BindInt("@top", top);
      return this.ReadTopRecycleBinPackageVersions();
    }

    public override void BatchPermanentlyDeletePackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<ProtocolPackageVersionIdentity> identities)
    {
      IEnumerable<Tuple<Guid, Guid>> rows = identities.Select<ProtocolPackageVersionIdentity, Tuple<Guid, Guid>>((System.Func<ProtocolPackageVersionIdentity, Tuple<Guid, Guid>>) (x =>
      {
        Guid? nullable = x.PackageId;
        Guid guid1 = nullable.Value;
        nullable = x.PackageVersionId;
        Guid guid2 = nullable.Value;
        return new Tuple<Guid, Guid>(guid1, guid2);
      }));
      this.PrepareStoredProcedure("Feed.prc_PermanentlyDeletePackageVersions");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      this.BindGuidGuidTable("@versions", rows);
      this.ExecuteNonQuery();
    }

    protected virtual IEnumerable<ProtocolPackageVersionIdentity> ReadTopRecycleBinPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        ProtocolPackageVersionIdentityBinder.BindOptions bindOptions = ProtocolPackageVersionIdentityBinder.BindOptions.IncludePackageIdAndVersionId;
        resultCollection.AddBinder<ProtocolPackageVersionIdentity>((ObjectBinder<ProtocolPackageVersionIdentity>) new ProtocolPackageVersionIdentityBinder(bindOptions));
        return (IEnumerable<ProtocolPackageVersionIdentity>) resultCollection.GetCurrent<ProtocolPackageVersionIdentity>().Items;
      }
    }
  }
}
