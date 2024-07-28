// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepoPackageMemoryCacheService`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public abstract class PublicRepoPackageMemoryCacheService<TDocument> : 
    VssMemoryCacheService<PublicRepoPackageCacheKey, EtagValue<TDocument>>,
    IPublicRepoPackageMemoryCacheService<TDocument>
    where TDocument : class, IVersionedDocument
  {
    private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
    private readonly Guid invalidationMessageId;
    private readonly IIdentityResolver identityResolver;

    public PublicRepoPackageMemoryCacheService(
      Guid invalidationMessageId,
      IIdentityResolver identityResolver)
      : base((IEqualityComparer<PublicRepoPackageCacheKey>) EqualityComparer<PublicRepoPackageCacheKey>.Default, MemoryCacheConfiguration<PublicRepoPackageCacheKey, EtagValue<TDocument>>.Default.Clone().WithExpiryInterval(TimeSpan.FromHours(1.0)))
    {
      this.invalidationMessageId = invalidationMessageId;
      this.identityResolver = identityResolver;
    }

    public bool TryGet(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      out EtagValue<TDocument> document)
    {
      requestContext.CheckDeploymentRequestContext();
      PublicRepoPackageCacheKey key = PublicRepoPackageCacheKey.Create(cacheUniverse, source, packageName);
      using (this.cacheLock.AcquireReadToken())
        return this.TryGetValue(requestContext, key, out document);
    }

    public bool TryAddOrReplace(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      EtagValue<TDocument> document,
      out EtagValue<TDocument> finalDocument)
    {
      requestContext.CheckDeploymentRequestContext();
      PublicRepoPackageCacheKey key = PublicRepoPackageCacheKey.Create(cacheUniverse, source, packageName);
      using (this.cacheLock.AcquireWriteToken())
      {
        EtagValue<TDocument> etagValue;
        if (this.TryGetValue(requestContext, key, out etagValue) && etagValue.Value.DocumentVersion >= document.Value.DocumentVersion)
        {
          finalDocument = etagValue;
          return false;
        }
        finalDocument = document;
        this.Set(requestContext, key, document);
        return true;
      }
    }

    public void Invalidate(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName)
    {
      this.BroadcastInvalidation(requestContext, cacheUniverse, source, packageName, new long?());
      this.InvalidateCore(requestContext, cacheUniverse, source, packageName, new long?());
    }

    public void Invalidate(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      long maxInvalidVersion)
    {
      this.BroadcastInvalidation(requestContext, cacheUniverse, source, packageName, new long?(maxInvalidVersion));
      this.InvalidateCore(requestContext, cacheUniverse, source, packageName, new long?(maxInvalidVersion));
    }

    private void InvalidateCore(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      long? maxInvalidVersion)
    {
      requestContext.CheckDeploymentRequestContext();
      PublicRepoPackageCacheKey key = PublicRepoPackageCacheKey.Create(cacheUniverse, source, packageName);
      using (this.cacheLock.AcquireWriteToken())
      {
        EtagValue<TDocument> etagValue;
        if (maxInvalidVersion.HasValue && this.TryGetValue(requestContext, key, out etagValue) && etagValue.Value.DocumentVersion > maxInvalidVersion.Value)
          return;
        this.Remove(requestContext, key);
      }
    }

    private void BroadcastInvalidation(
      IVssRequestContext requestContext,
      string cacheUniverse,
      WellKnownUpstreamSource source,
      IPackageName packageName,
      long? maxInvalidVersion)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      PublicRepoPackageMemoryCacheInvalidationMessage invalidationMessage = new PublicRepoPackageMemoryCacheInvalidationMessage()
      {
        CacheUniverse = cacheUniverse,
        SourceTagName = source.TagName,
        PackageDisplayName = packageName.DisplayName,
        MaxInvalidVersion = maxInvalidVersion
      };
      IVssRequestContext requestContext1 = requestContext;
      Guid invalidationMessageId = this.invalidationMessageId;
      string eventData = invalidationMessage.Serialize<PublicRepoPackageMemoryCacheInvalidationMessage>(true);
      service.SendNotification(requestContext1, invalidationMessageId, eventData);
    }

    private void OnInvalidationMessage(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      requestContext.CheckDeploymentRequestContext();
      PublicRepoPackageMemoryCacheInvalidationMessage invalidationMessage = args.Deserialize<PublicRepoPackageMemoryCacheInvalidationMessage>();
      WellKnownUpstreamSource tagNameOrDefault = WellKnownSourceProvider.Instance.GetWellKnownSourceByTagNameOrDefault(invalidationMessage.SourceTagName);
      if ((object) tagNameOrDefault == null)
        requestContext.TraceAlways(572906, TraceLevel.Warning, "PublicRepoCache", this.Name, "Ignoring invalidation message for nonexistent well-known source " + invalidationMessage.SourceTagName + ".");
      else
        this.InvalidateCore(requestContext, invalidationMessage.CacheUniverse, tagNameOrDefault, this.identityResolver.ResolvePackageName(invalidationMessage.PackageDisplayName), invalidationMessage.MaxInvalidVersion);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", this.invalidationMessageId, new SqlNotificationHandler(this.OnInvalidationMessage), true);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", this.invalidationMessageId, new SqlNotificationHandler(this.OnInvalidationMessage), false);
      base.ServiceEnd(systemRequestContext);
    }
  }
}
