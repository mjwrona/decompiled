// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCacheInfoStore
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCacheInfoStore : IUpstreamMetadataCacheInfoStore
  {
    private readonly IBlobService blobService;
    private readonly ISerializer<UpstreamMetadataCacheInfo> serializer;
    private readonly ITracerService tracerService;
    private readonly ICache<string, object> telemetryCache;

    public UpstreamMetadataCacheInfoStore(
      IBlobService blobService,
      ISerializer<UpstreamMetadataCacheInfo> serializer,
      ITracerService tracerService,
      ICache<string, object> telemetryCache)
    {
      this.blobService = blobService;
      this.serializer = serializer;
      this.tracerService = tracerService;
      this.telemetryCache = telemetryCache;
    }

    public async Task<UpstreamMetadataCacheInfo> GetMetadataCacheInfoAsync(FeedCore feed)
    {
      UpstreamMetadataCacheInfoStore sendInTheThisObject = this;
      UpstreamMetadataCacheInfo metadataCacheInfoAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetMetadataCacheInfoAsync)))
      {
        Locator locator = UpstreamMetadataCacheInfoStore.CalculateLocator(feed.Id);
        metadataCacheInfoAsync = (await sendInTheThisObject.GetMetadataCacheInfoCoreAsync(locator)).Item2;
      }
      return metadataCacheInfoAsync;
    }

    private static Locator CalculateLocator(Guid feedId) => new Locator(new string[2]
    {
      string.Format("{0:D}", (object) feedId),
      "upstreamMetadataCacheInfo.txt"
    });

    public async Task AddPackageIfNotExistsAsync(FeedCore feed, IPackageName packageName)
    {
      UpstreamMetadataCacheInfoStore sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (AddPackageIfNotExistsAsync)))
        await sendInTheThisObject.ModifyHelperAsync(feed, (IReadOnlyCollection<IPackageName>) new IPackageName[1]
        {
          packageName
        }, (Func<UpstreamMetadataCacheInfo, TransformResult>) (info => !info.PackageNames.Add(packageName) ? TransformResult.NoOp : TransformResult.DirtyEdit));
    }

    public async Task RemovePackagesIfExistsAsync(
      FeedCore feed,
      IReadOnlyCollection<IPackageName> packageNames)
    {
      UpstreamMetadataCacheInfoStore sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RemovePackagesIfExistsAsync)))
        await sendInTheThisObject.ModifyHelperAsync(feed, packageNames, (Func<UpstreamMetadataCacheInfo, TransformResult>) (info =>
        {
          bool flag = false;
          foreach (IPackageName packageName in (IEnumerable<IPackageName>) packageNames)
            flag |= info.PackageNames.Remove(packageName);
          return !flag ? TransformResult.NoOp : TransformResult.DirtyEdit;
        }));
    }

    public async Task RemoveAllPackagesExceptAsync(
      FeedCore feed,
      IReadOnlyCollection<IPackageName> packageNamesToKeep)
    {
      UpstreamMetadataCacheInfoStore sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RemoveAllPackagesExceptAsync)))
        await sendInTheThisObject.ModifyHelperAsync(feed, packageNamesToKeep, (Func<UpstreamMetadataCacheInfo, TransformResult>) (info =>
        {
          List<IPackageName> originalList = info.PackageNames.ToList<IPackageName>();
          info.PackageNames.IntersectWith((IEnumerable<IPackageName>) packageNamesToKeep);
          int num = originalList.Count - info.PackageNames.Count;
          tracer.TraceInfo(string.Format("Feed {0}: {1} removed {2} entries, leaving {3} entries", (object) feed.Id, (object) nameof (RemoveAllPackagesExceptAsync), (object) num, (object) info.PackageNames.Count));
          tracer.TraceConditionally(new string[1]
          {
            "RemoveAllPackagesExceptAsyncDetails"
          }, (Func<string>) (() => string.Format("Feed {0}: {1} names in original list: {2}", (object) feed.Id, (object) originalList.Count, (object) string.Join(", ", originalList.Select<IPackageName, string>((Func<IPackageName, string>) (x => x.NormalizedName))))));
          tracer.TraceConditionally(new string[1]
          {
            "RemoveAllPackagesExceptAsyncDetails"
          }, closure_2 ?? (closure_2 = (Func<string>) (() => string.Format("Feed {0}: {1} names in to-keep list: {2}", (object) feed.Id, (object) packageNamesToKeep.Count, (object) string.Join(", ", packageNamesToKeep.Select<IPackageName, string>((Func<IPackageName, string>) (x => x.NormalizedName)))))));
          tracer.TraceConditionally(new string[1]
          {
            "RemoveAllPackagesExceptAsyncDetails"
          }, (Func<string>) (() => string.Format("Feed {0}: {1} names in updated list: {2}", (object) feed.Id, (object) info.PackageNames.Count, (object) string.Join(", ", info.PackageNames.Select<IPackageName, string>((Func<IPackageName, string>) (x => x.NormalizedName))))));
          return num == 0 ? TransformResult.NoOp : TransformResult.DirtyEdit;
        }));
    }

    private async Task ModifyHelperAsync(
      FeedCore feed,
      IReadOnlyCollection<IPackageName> packageNames,
      Func<UpstreamMetadataCacheInfo, TransformResult> editFunc)
    {
      Locator locator = UpstreamMetadataCacheInfoStore.CalculateLocator(feed.Id);
      string str = (string) null;
      Random r = new Random();
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        (string, UpstreamMetadataCacheInfo) cacheInfoCoreAsync = await this.GetMetadataCacheInfoCoreAsync(locator);
        string etag = cacheInfoCoreAsync.Item1;
        UpstreamMetadataCacheInfo input = cacheInfoCoreAsync.Item2 ?? new UpstreamMetadataCacheInfo();
        if (editFunc(input) == TransformResult.NoOp)
        {
          locator = (Locator) null;
          r = (Random) null;
          return;
        }
        str = await this.blobService.PutBlobAsync(locator, this.serializer.Serialize(input).AsArraySegment(), etag);
      }
      if (str == null)
        throw new ChangeConflictException(string.Format("Could not apply upstream metadata cache changes: {0}, package name(s): {1}", (object) feed.Id, (object) string.Join<IPackageName>(",", (IEnumerable<IPackageName>) packageNames)));
      locator = (Locator) null;
      r = (Random) null;
    }

    private async Task<(string etag, UpstreamMetadataCacheInfo upstreamInfo)> GetMetadataCacheInfoCoreAsync(
      Locator locator)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        string blobAsync = await this.blobService.GetBlobAsync(locator, (Stream) ms);
        if (blobAsync == null)
        {
          this.telemetryCache.Set("Packaging.Properties.UpstreamMetadataLastBlobSize", (object) null);
          return ((string) null, (UpstreamMetadataCacheInfo) null);
        }
        this.telemetryCache.Set("Packaging.Properties.UpstreamMetadataLastBlobSize", (object) ms.Length);
        return (blobAsync, this.serializer.Deserialize((Stream) ms));
      }
    }
  }
}
