// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent6
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent6 : PackageSqlResourceComponent5
  {
    public override IEnumerable<PackageVersion> GetPackageVersions(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      bool? isListed = null,
      string protocolType = null,
      bool? isDeleted = null)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersions");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindNullableGuid("@viewId", feed.View?.Id);
      this.BindGuid("@packageId", packageId);
      this.BindNullableBoolean("@isListed", isListed);
      this.BindNullableBoolean("@isDeleted", isDeleted);
      return this.ReadPackageVersions();
    }
  }
}
