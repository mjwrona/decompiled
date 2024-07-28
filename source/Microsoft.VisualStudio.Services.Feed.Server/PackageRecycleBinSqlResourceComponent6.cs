// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent6
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Binders;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRecycleBinSqlResourceComponent6 : PackageRecycleBinSqlResourceComponent5
  {
    public override IEnumerable<DeletedPackageVersion> GetAllPackageVersionsByFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      this.PrepareStoredProcedure("Feed.prc_GetAllRecycleBinPackageVersionsByFeed");
      this.BindFeedIdentity(feed.GetIdentity(), false, "@dataspaceId", "@feedId");
      return this.ReadDeletedPackageVersions();
    }

    protected virtual IEnumerable<DeletedPackageVersion> ReadDeletedPackageVersions()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeletedPackageVersion>((ObjectBinder<DeletedPackageVersion>) new BindOntoBinder<DeletedPackageVersion>((IBindOnto<DeletedPackageVersion>) new DeletedPackageVersionBinder()));
        return (IEnumerable<DeletedPackageVersion>) resultCollection.GetCurrent<DeletedPackageVersion>().Items;
      }
    }
  }
}
