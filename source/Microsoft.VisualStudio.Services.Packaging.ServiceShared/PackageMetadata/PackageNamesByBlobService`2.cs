// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageNamesByBlobService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class PackageNamesByBlobService<TPackageName, TPackageId> : 
    IPackageNamesService<TPackageName, TPackageId>
    where TPackageName : class, IPackageName
    where TPackageId : IPackageIdentity
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly ContainerAddress containerAddress;
    private readonly ISerializer<IReadOnlyList<PackageNameEntry<TPackageName>>> serializer;
    private readonly ITracerService tracerService;

    public PackageNamesByBlobService(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      ContainerAddress containerAddress,
      ISerializer<IReadOnlyList<PackageNameEntry<TPackageName>>> serializer,
      ITracerService tracerService)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.containerAddress = containerAddress;
      this.serializer = serializer;
      this.tracerService = tracerService;
    }

    public async Task<IReadOnlyList<IPackageNameEntry<TPackageName>>> GetPackageNamesAsync(
      IFeedRequest feedRequest)
    {
      PackageNamesByBlobService<TPackageName, TPackageId> sendInTheThisObject = this;
      IReadOnlyList<IPackageNameEntry<TPackageName>> packageNamesAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageNamesAsync)))
        packageNamesAsync = (IReadOnlyList<IPackageNameEntry<TPackageName>>) (await sendInTheThisObject.GetPackageNamesCoreAsync(feedRequest)).Item2;
      return packageNamesAsync;
    }

    public async Task<(string eTag, List<PackageNameEntry<TPackageName>> packageNames)> GetPackageNamesCoreAsync(
      IFeedRequest feedRequest)
    {
      (string, List<PackageNameEntry<TPackageName>>) packageNamesCoreAsync;
      using (Stream blobStream = (Stream) new MemoryStream())
      {
        string blobAsync = await this.blobServiceFactory.Get(this.containerAddress).GetBlobAsync(new Locator(new string[2]
        {
          feedRequest.Feed.Id.ToString(),
          "names.txt"
        }), blobStream);
        packageNamesCoreAsync = blobAsync != null ? (blobAsync, this.serializer.Deserialize(blobStream).ToList<PackageNameEntry<TPackageName>>()) : ((string) null, new List<PackageNameEntry<TPackageName>>());
      }
      return packageNamesCoreAsync;
    }

    public async Task ApplyCommitsAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<PackageCommit<TPackageId>> packageCommits)
    {
      PackageNamesByBlobService<TPackageName, TPackageId> sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ApplyCommitsAsync)))
      {
        IBlobService blobService = sendInTheThisObject.blobServiceFactory.Get(sendInTheThisObject.containerAddress);
        Locator path = new Locator(new string[2]
        {
          feedRequest.Feed.Id.ToString(),
          "names.txt"
        });
        Dictionary<string, List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>> normNameToEntryMapForCommits = new Dictionary<string, List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>>();
        foreach (PackageCommit<TPackageId> packageCommit in (IEnumerable<PackageCommit<TPackageId>>) packageCommits)
        {
          string normalizedName = packageCommit.PackageRequest.PackageId.Name.NormalizedName;
          if (!normNameToEntryMapForCommits.ContainsKey(normalizedName))
            normNameToEntryMapForCommits[normalizedName] = new List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>();
          normNameToEntryMapForCommits[normalizedName].Add(new Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>(packageCommit.Commit, new PackageNameEntry<TPackageName>()
          {
            LastUpdatedDateTime = packageCommit.Commit.CreatedDate,
            Name = (TPackageName) packageCommit.PackageRequest.PackageId.Name
          }));
        }
        string str = (string) null;
        Random r = new Random();
        for (int iterations = 0; iterations < 10 && str == null; ++iterations)
        {
          if (iterations != 0)
            Thread.Sleep(500 + r.Next(1000));
          (string, List<PackageNameEntry<TPackageName>>) packageNamesCoreAsync = await sendInTheThisObject.GetPackageNamesCoreAsync(feedRequest);
          string etag = packageNamesCoreAsync.Item1;
          Dictionary<string, PackageNameEntry<TPackageName>> dictionary = packageNamesCoreAsync.Item2.ToDictionary<PackageNameEntry<TPackageName>, string, PackageNameEntry<TPackageName>>((Func<PackageNameEntry<TPackageName>, string>) (n => n.Name.NormalizedName), (Func<PackageNameEntry<TPackageName>, PackageNameEntry<TPackageName>>) (n => n));
          foreach (Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>> tuple in normNameToEntryMapForCommits.Values.SelectMany<List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>, Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>((Func<List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>, IEnumerable<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>>) (s => (IEnumerable<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>) s.ToList<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>())))
          {
            if (tuple.Item1.CommitOperationData is IAddOperationData || !dictionary.ContainsKey(tuple.Item2.Name.NormalizedName))
              dictionary[tuple.Item2.Name.NormalizedName] = tuple.Item2;
            else
              dictionary[tuple.Item2.Name.NormalizedName].LastUpdatedDateTime = tuple.Item1.CreatedDate;
          }
          str = await blobService.PutBlobAsync(path, sendInTheThisObject.serializer.Serialize((IReadOnlyList<PackageNameEntry<TPackageName>>) dictionary.Values.ToList<PackageNameEntry<TPackageName>>()).AsArraySegment(), etag);
        }
        blobService = (IBlobService) null;
        path = (Locator) null;
        normNameToEntryMapForCommits = (Dictionary<string, List<Tuple<ICommitLogEntry, PackageNameEntry<TPackageName>>>>) null;
        r = (Random) null;
      }
    }
  }
}
