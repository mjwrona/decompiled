// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent8
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent8 : PackageSqlResourceComponent7
  {
    public override int DeletePackageVersion(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      this.PrepareStoredProcedure("Feed.prc_DeletePackageVersion");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      this.BindDateTime2("@DeletedDate", deletedDate);
      return this.GetPackageVersionDeletedRowCount();
    }

    protected int GetPackageVersionDeletedRowCount()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SingleInt32ValueBinder("DeletedCount"));
        return resultCollection.GetCurrent<int>().Items[0];
      }
    }
  }
}
