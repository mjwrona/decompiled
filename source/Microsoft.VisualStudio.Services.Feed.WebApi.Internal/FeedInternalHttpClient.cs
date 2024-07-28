// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.FeedInternalHttpClient
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  public class FeedInternalHttpClient : InternalPackagingHttpClientBase
  {
    public FeedInternalHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FeedInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FeedInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FeedInternalHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FeedInternalHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task PackageVersionViewOperationAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedInternalHttpClient internalHttpClient = this;
      Guid packageIdAsync = await internalHttpClient.GetPackageIdAsync(project, feedId, protocolType, normalizedPackageName, userState, cancellationToken);
      await internalHttpClient.PackageVersionViewOperationAsync(project, feedId, packageIdAsync, packageVersionId, views, removeViews, userState, cancellationToken);
    }

    public virtual Task PackageVersionViewOperationAsync(
      string project,
      string feedId,
      Guid packageId,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdatePackageVersionAsync(PackageVersionPatch.AddOrRemoveViews(views, removeViews), project, feedId, packageId.ToString(), Uri.EscapeDataString(packageVersionId), userState, cancellationToken);
    }

    public virtual async Task UpdatePackageVersionAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedInternalHttpClient internalHttpClient = this;
      Guid packageIdAsync = await internalHttpClient.GetPackageIdAsync(project, feedId, protocolType, normalizedPackageName, (object) null, cancellationToken);
      await internalHttpClient.UpdatePackageVersionAsync(project, feedId, packageIdAsync, packageVersionId, isListed, userState, cancellationToken);
    }

    public virtual Task UpdatePackageVersionAsync(
      string project,
      string feedId,
      Guid packageId,
      string packageVersionId,
      bool? isListed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdatePackageVersionAsync(PackageVersionPatch.ChangeListedState(isListed), project, feedId, packageId.ToString(), Uri.EscapeDataString(packageVersionId), userState, cancellationToken);
    }

    public virtual Task UpdatePackageVersionAsync(
      string project,
      string feedId,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdatePackageVersionAsync(PackageVersionPatch.AddFiles(files), project, feedId, packageId, Uri.EscapeDataString(packageVersionId), userState, cancellationToken);
    }

    public virtual Task RestorePackageVersionToFeedAsync(
      string project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateRecycleBinPackageVersionAsync(PackageVersionPatch.RestoreRecycleBinPackageToFeed(), project, feedId, packageId, packageVersionId, userState, cancellationToken);
    }
  }
}
