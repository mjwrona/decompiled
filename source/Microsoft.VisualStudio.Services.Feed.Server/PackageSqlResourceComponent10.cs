// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent10
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent10 : PackageSqlResourceComponent9
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
      this.BindDateTime2("@scheduledPermanentDeleteDate", scheduledPermanentDeleteDate);
      return this.GetPackageVersionDeletedRowCount();
    }
  }
}
