// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.TracingFeedIndexInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class TracingFeedIndexInternalClientService
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedIndexInternalClientService backingService;
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.TracingFeedIndexInternalClient.TraceData;

    public TracingFeedIndexInternalClientService(IVssRequestContext requestContext)
    {
      this.backingService = requestContext.GetService<IFeedIndexInternalClientService>();
      this.requestContext = requestContext;
    }

    public Task DeletePackageVersionAsync(
      FeedCore feed,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726310, nameof (DeletePackageVersionAsync)))
        return this.backingService.DeletePackageVersionAsync(this.requestContext, feed, packageId, packageVersionId, deletedDate, scheduledPermanentDeleteDate);
    }

    public Task PackageVersionViewOperationAsync(
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726340, nameof (PackageVersionViewOperationAsync)))
        return this.backingService.PackageVersionViewOperationAsync(this.requestContext, feed, protocolType, normalizedPackageName, packageVersionId, views, removeViews);
    }

    public Task<PackageIndexEntryResponse> SetIndexEntryAsync(
      FeedCore feed,
      PackageIndexEntry indexEntry)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726350, nameof (SetIndexEntryAsync)))
        return this.backingService.SetIndexEntryAsync(this.requestContext, feed, indexEntry);
    }

    public Task UpdatePackageVersionsAsync(
      FeedCore feed,
      List<PackageVersionIndexEntryUpdate> updates)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726370, nameof (UpdatePackageVersionsAsync)))
        return this.backingService.UpdatePackageVersionsAsync(this.requestContext, feed, updates);
    }

    public Task UpdatePackageVersion(
      FeedCore feed,
      string protocolType,
      IPackageName packageName,
      IPackageVersion packageVersion,
      bool? isListed)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726380, nameof (UpdatePackageVersion)))
        return this.backingService.UpdatePackageVersion(this.requestContext, feed, protocolType, packageName, packageVersion, isListed);
    }

    public Task RestorePackageVersionToFeedAsync(
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726380, nameof (RestorePackageVersionToFeedAsync)))
        return this.backingService.RestorePackageVersionToFeedAsync(this.requestContext, feed, packageId, packageVersionId);
    }

    public Task PermanentlyDeletePackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(this.requestContext, TracingFeedIndexInternalClientService.TraceData, 5726380, nameof (PermanentlyDeletePackageVersionAsync)))
        return this.backingService.PermanentlyDeletePackageVersionAsync(this.requestContext, feed, packageId, packageVersionId);
    }
  }
}
