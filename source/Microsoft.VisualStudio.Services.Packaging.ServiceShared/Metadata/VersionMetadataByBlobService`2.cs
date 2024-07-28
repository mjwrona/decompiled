// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Metadata.VersionMetadataByBlobService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Metadata
{
  public class VersionMetadataByBlobService<TPackageIdentity, TMetadataEntry> : 
    IMetadataDocumentService<
    #nullable disable
    TPackageIdentity, TMetadataEntry>,
    IMetadataService<
    #nullable enable
    TPackageIdentity, TMetadataEntry>,
    IReadMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadSingleVersionMetadataService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry>,
    IReadMetadataDocumentService
    where TPackageIdentity : 
    #nullable disable
    IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly ContainerAddress containerAddress;
    private readonly IFactory<PackageNameQuery<TMetadataEntry>, ISerializer<MetadataDocument<TMetadataEntry>>> serializerFactory;
    private readonly IMetadataEntryOpApplierFactory<TMetadataEntry, MetadataDocument<TMetadataEntry>> opApplierFactory;
    private readonly IMetadataDocumentOpApplierFactory<MetadataDocument<TMetadataEntry>> metadataDocumentOpApplierFactory;
    private readonly IConverter<IPackageRequest<TPackageIdentity>, Locator> pathResolver;
    private readonly IComparer<IPackageVersion> versionComparer;
    private readonly ITracerService tracerService;
    private readonly ICache<string, object> telemetryCache;
    private readonly IMetadataChangeValidator<TMetadataEntry> changeValidator;

    public VersionMetadataByBlobService(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      ContainerAddress containerAddress,
      IFactory<PackageNameQuery<TMetadataEntry>, ISerializer<MetadataDocument<TMetadataEntry>>> serializerFactory,
      IMetadataEntryOpApplierFactory<TMetadataEntry, MetadataDocument<TMetadataEntry>> opApplierFactory,
      IMetadataDocumentOpApplierFactory<MetadataDocument<TMetadataEntry>> metadataDocumentOpApplierFactory,
      IConverter<IPackageRequest<TPackageIdentity>, Locator> pathResolver,
      IComparer<IPackageVersion> versionComparer,
      ITracerService tracerService,
      ICache<string, object> telemetryCache,
      IMetadataChangeValidator<TMetadataEntry> changeValidator)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.containerAddress = containerAddress;
      this.serializerFactory = serializerFactory;
      this.opApplierFactory = opApplierFactory;
      this.metadataDocumentOpApplierFactory = metadataDocumentOpApplierFactory;
      this.pathResolver = pathResolver;
      this.versionComparer = versionComparer;
      this.tracerService = tracerService;
      this.telemetryCache = telemetryCache;
      this.changeValidator = changeValidator;
    }

    public async Task ApplyCommitsAsync(
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IReadOnlyList<PackageNameCommit<IPackageName>> packageNameCommits)
    {
      VersionMetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ApplyCommitsAsync)))
      {
        IBlobService blobService = sendInTheThisObject.blobServiceFactory.Get(sendInTheThisObject.containerAddress);
        Dictionary<PackageRequest<TPackageIdentity>, PackageAndPackageNameCommits<TPackageIdentity>> source = new Dictionary<PackageRequest<TPackageIdentity>, PackageAndPackageNameCommits<TPackageIdentity>>();
        foreach (PackageCommit<TPackageIdentity> packageCommit in (IEnumerable<PackageCommit<TPackageIdentity>>) packageCommits)
        {
          PackageRequest<TPackageIdentity> key = new PackageRequest<TPackageIdentity>((IFeedRequest) packageCommit.PackageRequest, packageCommit.PackageRequest.PackageId);
          if (!source.ContainsKey(key))
            source[key] = new PackageAndPackageNameCommits<TPackageIdentity>();
          source[key].PackageCommits.Add(packageCommit);
        }
        // ISSUE: reference to a compiler-generated field
        await Task.WhenAll((IEnumerable<Task>) source.Select<KeyValuePair<PackageRequest<TPackageIdentity>, PackageAndPackageNameCommits<TPackageIdentity>>, Task>((Func<KeyValuePair<PackageRequest<TPackageIdentity>, PackageAndPackageNameCommits<TPackageIdentity>>, Task>) (x => this.\u003C\u003E4__this.ApplyCommitsForOneVersionAsync(x.Key, (IReadOnlyList<PackageCommit<TPackageIdentity>>) x.Value.PackageCommits, blobService))).ToList<Task>());
      }
    }

    public async Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> packageRequest)
    {
      VersionMetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      TMetadataEntry versionStateAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStateAsync)))
      {
        PackageNameQuery<TMetadataEntry> packageNameQuery = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>((IFeedRequest) packageRequest, packageRequest.PackageId.Name));
        QueryOptions<TMetadataEntry> queryOptions = new QueryOptions<TMetadataEntry>();
        TPackageIdentity packageId = packageRequest.PackageId;
        queryOptions.VersionUpper = packageId.Version;
        packageId = packageRequest.PackageId;
        queryOptions.VersionLower = packageId.Version;
        packageNameQuery.Options = queryOptions;
        PackageNameQuery<TMetadataEntry> input = packageNameQuery;
        Locator pathOfPackageMetadata = sendInTheThisObject.pathResolver.Convert(packageRequest);
        IBlobService blobService = sendInTheThisObject.blobServiceFactory.Get(sendInTheThisObject.containerAddress);
        ISerializer<MetadataDocument<TMetadataEntry>> serializer = sendInTheThisObject.serializerFactory.Get(input);
        MetadataDocument<TMetadataEntry> metadataDocument = (await sendInTheThisObject.GetPackageVersionStatesDocumentCoreAsync(pathOfPackageMetadata, blobService, serializer)).Item2;
        versionStateAsync = metadataDocument != null ? metadataDocument.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
      }
      return versionStateAsync;
    }

    public Task<List<TMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      throw new InvalidOperationException("GetPackageVersionStatesAsync is not allowed in VersionMetadataByBlobService");
    }

    public Task<MetadataDocument<TMetadataEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TMetadataEntry> packageNameRequest)
    {
      throw new InvalidOperationException("GetPackageVersionStatesDocumentAsync is not allowed in VersionMetadataByBlobService");
    }

    private async Task ApplyCommitsForOneVersionAsync(
      PackageRequest<TPackageIdentity> packageRequest,
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IBlobService blobService)
    {
      FeedCore feed = packageRequest.Feed;
      TPackageIdentity packageId1 = packageRequest.PackageId;
      IPackageName packageName = packageId1.Name;
      packageId1 = packageRequest.PackageId;
      IPackageVersion version = packageId1.Version;
      Locator pathOfPackageMetadata = this.pathResolver.Convert((IPackageRequest<TPackageIdentity>) packageRequest);
      string str = (string) null;
      PackageRequest<TPackageIdentity> packageRequest1 = packageRequest;
      packageId1 = packageRequest.PackageId;
      IPackageName name = packageId1.Name;
      PackageNameQuery<TMetadataEntry> input = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>((IFeedRequest) packageRequest1, name));
      QueryOptions<TMetadataEntry> queryOptions = new QueryOptions<TMetadataEntry>();
      packageId1 = packageRequest.PackageId;
      queryOptions.VersionUpper = packageId1.Version;
      packageId1 = packageRequest.PackageId;
      queryOptions.VersionLower = packageId1.Version;
      input.Options = queryOptions;
      ISerializer<MetadataDocument<TMetadataEntry>> serializer = this.serializerFactory.Get(input);
      Random r = new Random();
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        (string, MetadataDocument<TMetadataEntry>) documentCoreAsync = await this.GetPackageVersionStatesDocumentCoreAsync(pathOfPackageMetadata, blobService, serializer);
        string etag = documentCoreAsync.Item1;
        MetadataDocument<TMetadataEntry> metadataDocument1 = documentCoreAsync.Item2 ?? new MetadataDocument<TMetadataEntry>();
        bool flag = false;
        foreach (PackageCommit<TPackageIdentity> packageCommit in (IEnumerable<PackageCommit<TPackageIdentity>>) packageCommits)
        {
          IMetadataDocumentOpApplier<MetadataDocument<TMetadataEntry>> documentOpApplier = this.metadataDocumentOpApplierFactory.Get(packageCommit.Commit.CommitOperationData, false);
          if (documentOpApplier != null)
          {
            MetadataDocument<TMetadataEntry> metadataDocument2 = documentOpApplier.Apply(packageCommit.Commit, metadataDocument1);
            if (metadataDocument2 != metadataDocument1)
            {
              flag = true;
              metadataDocument1 = metadataDocument2;
            }
          }
          IMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>> metadataEntryOpApplier = this.opApplierFactory.Get(packageCommit.Commit.CommitOperationData);
          TPackageIdentity packageId2 = packageCommit.PackageRequest.PackageId;
          TMetadataEntry metadataEntry1 = metadataDocument1 != null ? metadataDocument1.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
          ICommitLogEntry commit = packageCommit.Commit;
          TMetadataEntry currentState = metadataEntry1;
          MetadataDocument<TMetadataEntry> metadataDocument3 = metadataDocument1;
          TMetadataEntry metadataEntry2 = metadataEntryOpApplier.Apply(commit, currentState, metadataDocument3);
          if ((object) metadataEntry1 != (object) metadataEntry2)
          {
            flag = true;
            if ((object) metadataEntry1 != null)
              metadataDocument1.Entries[0] = metadataEntry2;
            else
              metadataDocument1.Entries.Insert(0, metadataEntry2);
          }
        }
        if (!flag)
        {
          feed = (FeedCore) null;
          packageName = (IPackageName) null;
          pathOfPackageMetadata = (Locator) null;
          serializer = (ISerializer<MetadataDocument<TMetadataEntry>>) null;
          r = (Random) null;
          return;
        }
        str = await blobService.PutBlobAsync(pathOfPackageMetadata, serializer.Serialize(metadataDocument1).AsArraySegment(), etag);
      }
      if (str == null)
        throw new ChangeConflictException(string.Format("Could not apply metadata changes to feed: {0}, package name: {1}.", (object) feed.Id, (object) packageName));
      feed = (FeedCore) null;
      packageName = (IPackageName) null;
      pathOfPackageMetadata = (Locator) null;
      serializer = (ISerializer<MetadataDocument<TMetadataEntry>>) null;
      r = (Random) null;
    }

    private async Task<(string eTag, MetadataDocument<TMetadataEntry> metadataDocument)> GetPackageVersionStatesDocumentCoreAsync(
      Locator pathOfPackageMetadata,
      IBlobService blobService,
      ISerializer<MetadataDocument<TMetadataEntry>> serializer)
    {
      using (Stream blobStream = (Stream) new MemoryStream())
      {
        string blobAsync = await blobService.GetBlobAsync(pathOfPackageMetadata, blobStream);
        if (blobAsync == null)
        {
          this.telemetryCache.Set("Packaging.Properties.MetadataLastBlobSize", (object) null);
          return ((string) null, (MetadataDocument<TMetadataEntry>) null);
        }
        this.telemetryCache.Set("Packaging.Properties.MetadataLastBlobSize", (object) blobStream.Length);
        return (blobAsync, serializer.Deserialize(blobStream));
      }
    }

    public async Task<MetadataDocument<IMetadataEntry>> GetGenericUnfilteredPackageVersionStatesDocumentWithoutRefreshAsync(
      IPackageNameRequest packageNameRequest)
    {
      MetadataDocument<TMetadataEntry> statesDocumentAsync = await this.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<TMetadataEntry>(packageNameRequest));
      return statesDocumentAsync != null ? new MetadataDocument<IMetadataEntry>(statesDocumentAsync.Entries.Cast<IMetadataEntry>().ToList<IMetadataEntry>(), statesDocumentAsync.Properties) : (MetadataDocument<IMetadataEntry>) null;
    }
  }
}
