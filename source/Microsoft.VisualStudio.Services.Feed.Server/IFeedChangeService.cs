// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedChangeService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedChangeService))]
  public interface IFeedChangeService : IVssFrameworkService
  {
    IEnumerable<FeedChange> GetFeedChanges(
      IVssRequestContext requestContext,
      ProjectReference project,
      bool includeDeleted = false,
      long continuationToken = 0,
      int batchSize = 1000);

    FeedChange GetFeedChange(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId);

    IEnumerable<PackageChange> GetPackageChanges(
      IVssRequestContext requestContext,
      FeedCore feed,
      long continuationToken = 0,
      int batchSize = 1000);
  }
}
