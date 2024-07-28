// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataByBlobService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MetadataByBlobService<TPackageIdentity, TMetadataEntry> : 
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
    private readonly IConverter<IPackageNameRequest, Locator> pathResolver;
    private readonly IComparer<IPackageVersion> versionComparer;
    private readonly ITracerService tracerService;
    private readonly ICache<string, object> telemetryCache;
    private readonly IMetadataChangeValidator<TMetadataEntry> changeValidator;

    public MetadataByBlobService(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      ContainerAddress containerAddress,
      IFactory<PackageNameQuery<TMetadataEntry>, ISerializer<MetadataDocument<TMetadataEntry>>> serializerFactory,
      IMetadataEntryOpApplierFactory<TMetadataEntry, MetadataDocument<TMetadataEntry>> opApplierFactory,
      IMetadataDocumentOpApplierFactory<MetadataDocument<TMetadataEntry>> metadataDocumentOpApplierFactory,
      IConverter<IPackageNameRequest, Locator> pathResolver,
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
      MetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ApplyCommitsAsync)))
      {
        IBlobService blobService = sendInTheThisObject.blobServiceFactory.Get(sendInTheThisObject.containerAddress);
        Dictionary<PackageNameRequest<IPackageName>, PackageAndPackageNameCommits<TPackageIdentity>> source = new Dictionary<PackageNameRequest<IPackageName>, PackageAndPackageNameCommits<TPackageIdentity>>();
        foreach (PackageCommit<TPackageIdentity> packageCommit in (IEnumerable<PackageCommit<TPackageIdentity>>) packageCommits)
        {
          PackageNameRequest<IPackageName> key = new PackageNameRequest<IPackageName>((IFeedRequest) packageCommit.PackageRequest, packageCommit.PackageRequest.PackageId.Name);
          if (!source.ContainsKey(key))
            source[key] = new PackageAndPackageNameCommits<TPackageIdentity>();
          source[key].PackageCommits.Add(packageCommit);
        }
        foreach (PackageNameCommit<IPackageName> packageNameCommit in (IEnumerable<PackageNameCommit<IPackageName>>) packageNameCommits)
        {
          PackageNameRequest<IPackageName> packageNameRequest = packageNameCommit.PackageNameRequest;
          if (!source.ContainsKey(packageNameCommit.PackageNameRequest))
            source[packageNameRequest] = new PackageAndPackageNameCommits<TPackageIdentity>();
          source[packageNameRequest].PackageNameCommits.Add(packageNameCommit);
        }
        // ISSUE: reference to a compiler-generated field
        await Task.WhenAll((IEnumerable<Task>) source.Select<KeyValuePair<PackageNameRequest<IPackageName>, PackageAndPackageNameCommits<TPackageIdentity>>, Task>((Func<KeyValuePair<PackageNameRequest<IPackageName>, PackageAndPackageNameCommits<TPackageIdentity>>, Task>) (x => this.\u003C\u003E4__this.ApplyCommitsForOneNameAsync(x.Key, (IReadOnlyList<PackageCommit<TPackageIdentity>>) x.Value.PackageCommits, (IReadOnlyList<PackageNameCommit<IPackageName>>) x.Value.PackageNameCommits, blobService))).ToList<Task>());
      }
    }

    private async Task ApplyCommitsForOneNameAsync(
      PackageNameRequest<IPackageName> packageNameRequest,
      IReadOnlyList<PackageCommit<TPackageIdentity>> packageCommits,
      IReadOnlyList<PackageNameCommit<IPackageName>> packageNameCommits,
      IBlobService blobService)
    {
      FeedCore feed = packageNameRequest.Feed;
      IPackageName packageName = packageNameRequest.PackageName;
      Locator pathOfPackageMetadata = this.pathResolver.Convert((IPackageNameRequest) packageNameRequest);
      string str = (string) null;
      ISerializer<MetadataDocument<TMetadataEntry>> serializer = this.serializerFactory.Get(new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) packageNameRequest));
      Random r = new Random();
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        (string, MetadataDocument<TMetadataEntry>) documentCoreAsync = await this.GetPackageVersionStatesDocumentCoreAsync(pathOfPackageMetadata, blobService, serializer);
        string etag = documentCoreAsync.Item1;
        MetadataDocument<TMetadataEntry> metadataDocument1 = documentCoreAsync.Item2 ?? new MetadataDocument<TMetadataEntry>();
        List<TMetadataEntry> originalEntries = new List<TMetadataEntry>((IEnumerable<TMetadataEntry>) metadataDocument1.Entries);
        bool flag = false;
        foreach (PackageNameCommit<IPackageName> packageNameCommit in (IEnumerable<PackageNameCommit<IPackageName>>) packageNameCommits)
        {
          MetadataDocument<TMetadataEntry> metadataDocument2 = this.metadataDocumentOpApplierFactory.Get(packageNameCommit.Commit.CommitOperationData).Apply(packageNameCommit.Commit, metadataDocument1);
          if (metadataDocument2 != metadataDocument1)
          {
            flag = true;
            metadataDocument1 = metadataDocument2;
          }
        }
        foreach (PackageCommit<TPackageIdentity> packageCommit in (IEnumerable<PackageCommit<TPackageIdentity>>) packageCommits)
        {
          IMetadataDocumentOpApplier<MetadataDocument<TMetadataEntry>> documentOpApplier = this.metadataDocumentOpApplierFactory.Get(packageCommit.Commit.CommitOperationData, false);
          if (documentOpApplier != null)
          {
            MetadataDocument<TMetadataEntry> metadataDocument3 = documentOpApplier.Apply(packageCommit.Commit, metadataDocument1);
            if (metadataDocument3 != metadataDocument1)
            {
              flag = true;
              metadataDocument1 = metadataDocument3;
            }
          }
          IMetadataEntryOpApplier<TMetadataEntry, MetadataDocument<TMetadataEntry>> metadataEntryOpApplier = this.opApplierFactory.Get(packageCommit.Commit.CommitOperationData);
          int closestIndex;
          TMetadataEntry metadataEntry1 = PackagingUtils.BinarySearch<IPackageVersion, TMetadataEntry>(packageCommit.PackageRequest.PackageId.Version, (IReadOnlyList<TMetadataEntry>) metadataDocument1.Entries, (Func<TMetadataEntry, IPackageVersion>) (metadataEntry => metadataEntry.PackageIdentity.Version), this.versionComparer, out closestIndex);
          ICommitLogEntry commit = packageCommit.Commit;
          TMetadataEntry currentState = metadataEntry1;
          MetadataDocument<TMetadataEntry> metadataDocument4 = metadataDocument1;
          TMetadataEntry metadataEntry2 = metadataEntryOpApplier.Apply(commit, currentState, metadataDocument4);
          if ((object) metadataEntry1 != (object) metadataEntry2)
          {
            flag = true;
            if ((object) metadataEntry1 != null)
              metadataDocument1.Entries[closestIndex] = metadataEntry2;
            else
              metadataDocument1.Entries.Insert(closestIndex, metadataEntry2);
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
        this.EnsureEntriesStrictlyOrdered(feed, packageName, metadataDocument1.Entries);
        this.changeValidator.Validate((IFeedRequest) packageNameRequest, packageName, (IList<TMetadataEntry>) originalEntries, (IList<TMetadataEntry>) metadataDocument1.Entries, packageNameCommits.Select<PackageNameCommit<IPackageName>, ICommitLogEntry>((Func<PackageNameCommit<IPackageName>, ICommitLogEntry>) (c => c.Commit)).Union<ICommitLogEntry>(packageCommits.Select<PackageCommit<TPackageIdentity>, ICommitLogEntry>((Func<PackageCommit<TPackageIdentity>, ICommitLogEntry>) (c => c.Commit))));
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

    private void EnsureEntriesStrictlyOrdered(
      FeedCore feed,
      IPackageName packageName,
      List<TMetadataEntry> metadataEntries)
    {
      for (int index = 1; index < metadataEntries.Count; ++index)
      {
        IPackageVersion version1 = metadataEntries[index - 1].PackageIdentity.Version;
        IPackageVersion version2 = metadataEntries[index].PackageIdentity.Version;
        int num = this.versionComparer.Compare(version2, version1);
        if (num <= 0)
          throw new InvalidDataException(string.Format("Tried to write out of order, or multiple same versioned metadata entries for feed: '{0}', package: '{1}', curr: {2}, prev: {3}, result: {4}", (object) feed.Id, (object) packageName.NormalizedName, (object) version2?.DisplayVersion, (object) version1?.DisplayVersion, (object) num));
      }
    }

    public async Task<TMetadataEntry> GetPackageVersionStateAsync(
      IPackageRequest<TPackageIdentity> request)
    {
      MetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      TMetadataEntry versionStateAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStateAsync)))
      {
        MetadataByBlobService<TPackageIdentity, TMetadataEntry> metadataByBlobService = sendInTheThisObject;
        IPackageRequest<TPackageIdentity> packageRequest = request;
        TPackageIdentity packageId = request.PackageId;
        IPackageName name = packageId.Name;
        PackageNameQuery<TMetadataEntry> packageNameQueryRequest = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>((IFeedRequest) packageRequest, name));
        QueryOptions<TMetadataEntry> queryOptions = new QueryOptions<TMetadataEntry>();
        packageId = request.PackageId;
        queryOptions.VersionUpper = packageId.Version;
        packageId = request.PackageId;
        queryOptions.VersionLower = packageId.Version;
        packageNameQueryRequest.Options = queryOptions;
        // ISSUE: explicit non-virtual call
        MetadataDocument<TMetadataEntry> statesDocumentAsync = await __nonvirtual (metadataByBlobService.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest));
        versionStateAsync = statesDocumentAsync != null ? statesDocumentAsync.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
      }
      return versionStateAsync;
    }

    public async Task<MetadataDocument<TMetadataEntry>> GetPackageVersionStatesDocumentAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      MetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      MetadataDocument<TMetadataEntry> statesDocumentAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesDocumentAsync)))
      {
        MetadataByBlobService<TPackageIdentity, TMetadataEntry>.EnsureQuarantineUntilDateProjected(packageNameQueryRequest);
        IPackageNameRequest requestData = packageNameQueryRequest.RequestData;
        Locator pathOfPackageMetadata = sendInTheThisObject.pathResolver.Convert(requestData);
        IBlobService blobService = sendInTheThisObject.blobServiceFactory.Get(sendInTheThisObject.containerAddress);
        ISerializer<MetadataDocument<TMetadataEntry>> serializer = sendInTheThisObject.serializerFactory.Get(packageNameQueryRequest);
        MetadataDocument<TMetadataEntry> metadataDocument = (await sendInTheThisObject.GetPackageVersionStatesDocumentCoreAsync(pathOfPackageMetadata, blobService, serializer)).Item2;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        statesDocumentAsync = metadataDocument == null ? (MetadataDocument<TMetadataEntry>) null : new MetadataDocument<TMetadataEntry>(metadataDocument.Entries.Where<TMetadataEntry>(MetadataByBlobService<TPackageIdentity, TMetadataEntry>.\u003C\u003EO.\u003C0\u003E__IsTerrapinApproved ?? (MetadataByBlobService<TPackageIdentity, TMetadataEntry>.\u003C\u003EO.\u003C0\u003E__IsTerrapinApproved = new Func<TMetadataEntry, bool>(IsTerrapinApproved))).ToList<TMetadataEntry>(), metadataDocument.Properties);
      }
      return statesDocumentAsync;

      static bool IsTerrapinApproved(TMetadataEntry entry)
      {
        if ((object) entry == null || entry.IsLocal)
          return true;
        if (entry.PackageIdentity.Name.Protocol.LowercasedName == "nuget")
        {
          string normalizedName = entry.PackageIdentity.Name.NormalizedName;
          string normalizedVersion = entry.PackageIdentity.Version.NormalizedVersion;
          if (normalizedName == "microsoft.identity.client" && normalizedVersion.StartsWith("16.") || normalizedName == "newtonsoft.json" && normalizedVersion == "2021.2.19.2")
            return false;
        }
        if (!entry.QuarantineUntilDate.HasValue)
          return true;
        DateTime utcNow = DateTime.UtcNow;
        DateTime? quarantineUntilDate = entry.QuarantineUntilDate;
        return quarantineUntilDate.HasValue && utcNow > quarantineUntilDate.GetValueOrDefault();
      }
    }

    private static void EnsureQuarantineUntilDateProjected(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      List<string> projections = packageNameQueryRequest?.Options?.GetProjections();
      if (projections == null || !projections.Any<string>())
        return;
      packageNameQueryRequest.Options = packageNameQueryRequest.Options.OnlyProjecting((Expression<Func<TMetadataEntry, object>>) (x => (object) x.QuarantineUntilDate));
    }

    public async Task<List<TMetadataEntry>> GetPackageVersionStatesAsync(
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest)
    {
      MetadataByBlobService<TPackageIdentity, TMetadataEntry> sendInTheThisObject = this;
      List<TMetadataEntry> versionStatesAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesAsync)))
      {
        // ISSUE: explicit non-virtual call
        versionStatesAsync = (await __nonvirtual (sendInTheThisObject.GetPackageVersionStatesDocumentAsync(packageNameQueryRequest)))?.Entries ?? new List<TMetadataEntry>();
      }
      return versionStatesAsync;
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
