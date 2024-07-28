// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageRecycleBinSqlResourceComponent2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageRecycleBinSqlResourceComponent2 : PackageRecycleBinSqlResourceComponent
  {
    public override IEnumerable<Package> GetPackages(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      string packageNameQuery,
      PagingOptions pagingOptions,
      DateTime now,
      bool includeAllVersions)
    {
      this.PrepareStoredProcedure("Feed.prc_GetRecycleBinPackages");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@packageNameQuery", packageNameQuery, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@top", pagingOptions.Top);
      this.BindInt("@skip", pagingOptions.Skip);
      this.BindDateTime2("@now", now);
      this.BindBoolean("@includeAllVersions", includeAllVersions);
      return this.ReadPackages();
    }
  }
}
