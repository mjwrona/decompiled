// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PublicRepoCacheCore`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.Internal.WebApi.Types;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PublicRepoCacheCore<TPackageName, TDoc, TCursor> : 
    IPublicRepoCacheCore<TPackageName, TDoc, TCursor>
    where TPackageName : class, IPackageName
    where TDoc : class, IVersionedDocument, IHaveGenerationCursor<TCursor>, IHaveIsDefaultInitialized
    where TCursor : class, IComparable<TCursor>
  {
    public PublicRepoCacheCore(
      IAggregationDocumentProvider<TDoc, TPackageName> docProvider,
      IETaggedDocumentUpdater docUpdater,
      ITracerService tracerService,
      IPublicRepoPackageMemoryCache<TDoc> memoryCache,
      ICacheUniverseProvider cacheUniverseProvider,
      IConcurrencyConsolidator<PublicRepoUpdateConcurrencyKey<TCursor>, TDoc> concurrencyConsolidator,
      IDirectPublicRepoDataFetcher<TPackageName, TDoc, TCursor> dataFetcher,
      WellKnownUpstreamSource wellKnownUpstreamSource,
      IEmptyDocumentProvider<TDoc> emptyDocProvider)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CdocProvider\u003EP = docProvider;
      // ISSUE: reference to a compiler-generated field
      this.\u003CdocUpdater\u003EP = docUpdater;
      // ISSUE: reference to a compiler-generated field
      this.\u003CtracerService\u003EP = tracerService;
      // ISSUE: reference to a compiler-generated field
      this.\u003CmemoryCache\u003EP = memoryCache;
      // ISSUE: reference to a compiler-generated field
      this.\u003CcacheUniverseProvider\u003EP = cacheUniverseProvider;
      // ISSUE: reference to a compiler-generated field
      this.\u003CconcurrencyConsolidator\u003EP = concurrencyConsolidator;
      // ISSUE: reference to a compiler-generated field
      this.\u003CdataFetcher\u003EP = dataFetcher;
      // ISSUE: reference to a compiler-generated field
      this.\u003CwellKnownUpstreamSource\u003EP = wellKnownUpstreamSource;
      // ISSUE: reference to a compiler-generated field
      this.\u003CemptyDocProvider\u003EP = emptyDocProvider;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public async Task InvalidatePackageVersionDataAsync(
      TPackageName packageName,
      TCursor? minValidCursor,
      bool allowRefresh)
    {
      PublicRepoCacheCore<TPackageName, TDoc, TCursor> sendInTheThisObject = this;
      // ISSUE: reference to a compiler-generated field
      ITracerBlock tracer = sendInTheThisObject.\u003CtracerService\u003EP.Enter((object) sendInTheThisObject, nameof (InvalidatePackageVersionDataAsync));
      try
      {
        if (allowRefresh)
        {
          // ISSUE: explicit non-virtual call
          TDoc doc = await __nonvirtual (sendInTheThisObject.UpdatePackageMetadataAsync(packageName, minValidCursor));
          tracer = (ITracerBlock) null;
        }
        else
        {
          await sendInTheThisObject.InvalidatePackageMetadataStorageCacheAsync(packageName);
          sendInTheThisObject.InvalidatePackageMetadataMemoryCache(packageName);
          tracer = (ITracerBlock) null;
        }
      }
      finally
      {
        tracer?.Dispose();
      }
    }

    public async Task<TDoc> GetPackageMetadataAsync(TPackageName packageName)
    {
      PublicRepoCacheCore<TPackageName, TDoc, TCursor> sendInTheThisObject = this;
      TDoc packageMetadataAsync;
      // ISSUE: reference to a compiler-generated field
      using (sendInTheThisObject.\u003CtracerService\u003EP.Enter((object) sendInTheThisObject, nameof (GetPackageMetadataAsync)))
      {
        EtagValue<TDoc> etagValue = sendInTheThisObject.GetPackageMetadataFromMemoryCacheCore(packageName);
        if (!etagValue.Value.IsDefaultInitialized())
        {
          EtagValue<TDoc> document = await sendInTheThisObject.GetPackageMetadataFromStorageCacheCoreAsync(packageName);
          if (!document.Value.IsDefaultInitialized())
            document = await sendInTheThisObject.UpdatePackageMetadataStorageCacheCoreAsync(packageName, new EtagValue<TDoc>?(document), default (TCursor));
          etagValue = sendInTheThisObject.SetPackageMetadataMemoryCacheCore(packageName, document);
        }
        packageMetadataAsync = etagValue.Value;
      }
      return packageMetadataAsync;
    }

    public async Task<TDoc?> GetMetadataForDiagnosticsAsync(
      PublicRepositoryCacheType? cacheType,
      TPackageName typedName)
    {
      PublicRepoCacheCore<TPackageName, TDoc, TCursor> sendInTheThisObject = this;
      TDoc diagnosticsAsync;
      // ISSUE: reference to a compiler-generated field
      using (sendInTheThisObject.\u003CtracerService\u003EP.Enter((object) sendInTheThisObject, nameof (GetMetadataForDiagnosticsAsync)))
      {
        TDoc doc;
        if (cacheType.HasValue)
        {
          switch (cacheType.GetValueOrDefault())
          {
            case PublicRepositoryCacheType.None:
              doc = await sendInTheThisObject.GetPackageMetadataFromUpstreamAsync(typedName);
              break;
            case PublicRepositoryCacheType.Storage:
              doc = await sendInTheThisObject.GetPackageMetadataFromStorageCacheAsync(typedName);
              break;
            case PublicRepositoryCacheType.Memory:
              doc = sendInTheThisObject.GetPackageMetadataFromMemoryCache(typedName);
              break;
            default:
              throw new ArgumentOutOfRangeException(nameof (cacheType));
          }
        }
        else
        {
          // ISSUE: explicit non-virtual call
          doc = await __nonvirtual (sendInTheThisObject.GetPackageMetadataAsync(typedName));
        }
        diagnosticsAsync = doc;
      }
      return diagnosticsAsync;
    }

    public async Task<TDoc> UpdatePackageMetadataAsync(
      TPackageName packageName,
      TCursor? minValidCursor)
    {
      PublicRepoCacheCore<TPackageName, TDoc, TCursor> sendInTheThisObject = this;
      // ISSUE: reference to a compiler-generated field
      using (sendInTheThisObject.\u003CtracerService\u003EP.Enter((object) sendInTheThisObject, nameof (UpdatePackageMetadataAsync)))
      {
        EtagValue<TDoc> initialDoc = sendInTheThisObject.GetPackageMetadataFromMemoryCacheCore(packageName);
        // ISSUE: variable of a compiler-generated type
        PublicRepoCacheCore<TPackageName, TDoc, TCursor>.\u003C\u003Ec__DisplayClass13_0 cDisplayClass130;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return (object) minValidCursor != null && (object) initialDoc.Value.GenerationCursorPosition != null && initialDoc.Value.GenerationCursorPosition.CompareTo(minValidCursor) > 0 ? initialDoc.Value : await sendInTheThisObject.\u003CconcurrencyConsolidator\u003EP.RunOnceAsync(new PublicRepoUpdateConcurrencyKey<TCursor>(PublicRepoPackageCacheKey.Create(sendInTheThisObject.\u003CcacheUniverseProvider\u003EP.GetCacheUniverseName(), sendInTheThisObject.\u003CwellKnownUpstreamSource\u003EP, (IPackageName) packageName), minValidCursor, new long?(initialDoc.Value.DocumentVersion)), (Func<Task<TDoc>>) (async () =>
        {
          // ISSUE: reference to a compiler-generated field
          EtagValue<TDoc> document = await cDisplayClass130.\u003C\u003E4__this.UpdatePackageMetadataStorageCacheCoreAsync(packageName, new EtagValue<TDoc>?(initialDoc), minValidCursor);
          // ISSUE: reference to a compiler-generated field
          return cDisplayClass130.\u003C\u003E4__this.SetPackageMetadataMemoryCacheCore(packageName, document).Value;
        }));
      }
    }

    private TDoc? GetPackageMetadataFromMemoryCache(TPackageName typedName)
    {
      TDoc doc1;
      this.GetPackageMetadataFromMemoryCacheCore(typedName).Deconstruct<TDoc>(out doc1, out string _);
      TDoc doc2 = doc1;
      return !doc2.IsDefaultInitialized() ? default (TDoc) : doc2;
    }

    private EtagValue<TDoc> GetPackageMetadataFromMemoryCacheCore(TPackageName packageName)
    {
      EtagValue<TDoc> document;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.\u003CmemoryCache\u003EP.TryGet(this.\u003CcacheUniverseProvider\u003EP.GetCacheUniverseName(), this.\u003CwellKnownUpstreamSource\u003EP, (IPackageName) packageName, out document) ? document : new EtagValue<TDoc>(this.\u003CemptyDocProvider\u003EP.GetEmptyDocument(), (string) null);
    }

    private EtagValue<TDoc> SetPackageMetadataMemoryCacheCore(
      TPackageName packageName,
      EtagValue<TDoc> document)
    {
      EtagValue<TDoc> finalDocument;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.\u003CmemoryCache\u003EP.TryAddOrReplace(this.\u003CcacheUniverseProvider\u003EP.GetCacheUniverseName(), this.\u003CwellKnownUpstreamSource\u003EP, (IPackageName) packageName, document, out finalDocument);
      return finalDocument;
    }

    private void InvalidatePackageMetadataMemoryCache(TPackageName packageName) => this.\u003CmemoryCache\u003EP.Invalidate(this.\u003CcacheUniverseProvider\u003EP.GetCacheUniverseName(), this.\u003CwellKnownUpstreamSource\u003EP, (IPackageName) packageName);

    private async Task<TDoc?> GetPackageMetadataFromStorageCacheAsync(TPackageName packageName)
    {
      TDoc doc1;
      (await this.GetPackageMetadataFromStorageCacheCoreAsync(packageName)).Deconstruct<TDoc>(out doc1, out string _);
      TDoc doc2 = doc1;
      TDoc storageCacheAsync;
      if (!doc2.IsDefaultInitialized())
      {
        doc1 = default (TDoc);
        storageCacheAsync = doc1;
      }
      else
        storageCacheAsync = doc2;
      return storageCacheAsync;
    }

    private async Task<EtagValue<TDoc>> GetPackageMetadataFromStorageCacheCoreAsync(
      TPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      return await this.\u003CdocProvider\u003EP.GetDocumentAsync(UnusableFeedRequest.Instance, packageName);
    }

    private async Task<EtagValue<TDoc>> UpdatePackageMetadataStorageCacheCoreAsync(
      TPackageName packageName,
      EtagValue<TDoc>? initialDoc,
      TCursor? minValidCursor)
    {
      // ISSUE: variable of a compiler-generated type
      PublicRepoCacheCore<TPackageName, TDoc, TCursor>.\u003C\u003Ec__DisplayClass20_0 cDisplayClass200;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (await this.\u003CdocUpdater\u003EP.UpdateETaggedDocumentAsync<TDoc, TPackageName>(initialDoc, this.\u003CdocProvider\u003EP, UnusableFeedRequest.Instance, packageName, (Func<TDoc, Task<(TDoc, bool)>>) (async existingDoc => await cDisplayClass200.\u003C\u003E4__this.\u003CdataFetcher\u003EP.FetchFromUpstreamAndApplyToDocAsync(packageName, existingDoc, minValidCursor)), (Func<string>) (() => "Failed to update public repo cache"))).Item1;
    }

    private Task InvalidatePackageMetadataStorageCacheAsync(TPackageName packageName) => this.\u003CdocProvider\u003EP.RemoveDocumentAsync(UnusableFeedRequest.Instance, packageName);

    private async Task<TDoc> GetPackageMetadataFromUpstreamAsync(TPackageName packageName) => (await this.\u003CdataFetcher\u003EP.FetchFromUpstreamAndApplyToDocAsync(packageName, this.\u003CemptyDocProvider\u003EP.GetEmptyDocument(), default (TCursor))).Item1;
  }
}
