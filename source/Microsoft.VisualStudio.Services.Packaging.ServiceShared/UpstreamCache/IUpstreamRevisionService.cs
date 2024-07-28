// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.IUpstreamRevisionService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache
{
  public interface IUpstreamRevisionService : IVssFrameworkService
  {
    uint GetUpstreamPackagesRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      string upstream);

    uint CreateNewUpstreamPackagesRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      string upstream);

    List<UpstreamRevision> GetStaleUpstreamRevisions(
      IVssRequestContext requestContext,
      FeedCore feed);

    void RemoveStaleUpstreamRevision(
      IVssRequestContext requestContext,
      FeedCore feed,
      UpstreamRevision revision);
  }
}
