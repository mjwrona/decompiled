// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedRecycleBinService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedRecycleBinService))]
  public interface IFeedRecycleBinService : IVssFrameworkService
  {
    IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsFromRecycleBin(
      IVssRequestContext requestContext,
      ProjectReference project);

    void PermanentDeleteFeed(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId);

    void RestoreDeletedFeed(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId);
  }
}
