// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.BlobEdgeCachingFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class BlobEdgeCachingFacade : IBlobEdgeCaching
  {
    private readonly IVssRequestContext requestContext;

    public BlobEdgeCachingFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    private IBlobEdgeCachingService BlobEdgeCachingService => (IBlobEdgeCachingService) this.requestContext.GetService<IPackageEdgeCachingService>();

    public Uri GetEdgeUri(Uri uri) => this.BlobEdgeCachingService.GetEdgeUri(uri);

    public Uri GetEdgeUri(Uri uri, DateTime expiry) => this.BlobEdgeCachingService.GetEdgeUri(uri, expiry);

    public Uri GetEdgeUri(Uri uri, TimeSpan duration) => this.BlobEdgeCachingService.GetEdgeUri(uri, duration);

    public bool UserIsExcluded() => this.BlobEdgeCachingService.UserIsExcluded(this.requestContext);
  }
}
