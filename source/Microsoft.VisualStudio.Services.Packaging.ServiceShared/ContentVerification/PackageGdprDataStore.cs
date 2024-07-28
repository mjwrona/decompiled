// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.PackageGdprDataStore
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class PackageGdprDataStore : IPackageGdprDataStore
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly IResolvedCloudBlobContainerLister cloudBlobContainerFactory;
    private readonly ISerializer<PackageGdprData> serializer;
    private readonly ContainerAddress baseDeploymentContainerAddress;
    private readonly ITimeProvider timeProvider;
    public static readonly TimeSpan ExpireGdprContentOlderThan = TimeSpan.FromDays(14.0);

    public PackageGdprDataStore(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      IResolvedCloudBlobContainerLister cloudBlobContainerFactory,
      ISerializer<PackageGdprData> serializer,
      ContainerAddress baseDeploymentContainerAddress,
      ITimeProvider timeProvider)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.cloudBlobContainerFactory = cloudBlobContainerFactory;
      this.serializer = serializer;
      this.baseDeploymentContainerAddress = baseDeploymentContainerAddress;
      this.timeProvider = timeProvider;
    }

    public IEnumerable<string> DeleteExpiredData()
    {
      List<string> stringList = new List<string>();
      foreach (IResolvedCloudBlobContainer cloudBlobContainer in this.cloudBlobContainerFactory.GetFromRoot(this.baseDeploymentContainerAddress))
      {
        if (this.ShouldExpireContainer(cloudBlobContainer.Address.Path))
        {
          cloudBlobContainer.DeleteIfExistsAsync(VssRequestPump.Processor.CreateWithoutRequestContext(CancellationToken.None, Guid.Empty, Guid.Empty, VssClientHttpRequestSettings.Default.SessionId));
          stringList.Add(cloudBlobContainer.Name);
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private bool ShouldExpireContainer(Locator path)
    {
      DateTime result;
      return path.MatchesEnumerationQuery(this.baseDeploymentContainerAddress.Path, PathOptions.ImmediateChildren) && DateTime.TryParseExact(path.PathSegments[path.PathSegmentCount - 1], "yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) && !(this.timeProvider.Now - result < PackageGdprDataStore.ExpireGdprContentOlderThan);
    }

    public async Task<PackageGdprData> RetrieveUploaderData(
      CollectionId collectionId,
      FeedId feedId,
      IProtocol protocol,
      CommitLogBookmark commitLogBookmark,
      DateTime commitUploadDateTime)
    {
      IBlobService blobService = this.blobServiceFactory.Get(new ContainerAddress(this.baseDeploymentContainerAddress.CollectionId, new Locator(this.baseDeploymentContainerAddress.Path, new Locator(new string[1]
      {
        commitUploadDateTime.ToString("yyyyMMdd")
      }))));
      string[] strArray = new string[4];
      Guid guid = collectionId.Guid;
      strArray[0] = guid.ToString("N");
      strArray[1] = protocol.CorrectlyCasedName;
      guid = feedId.Guid;
      strArray[2] = guid.ToString("N");
      strArray[3] = commitLogBookmark.CommitId.ToString() + ".txt";
      return (await this.RetrieveUploaderDataCore(new Locator(strArray), blobService)).Item2;
    }

    private async Task<(string eTag, PackageGdprData gdprData)> RetrieveUploaderDataCore(
      Locator path,
      IBlobService blobService)
    {
      (string, PackageGdprData) valueTuple;
      using (Stream blobStream = (Stream) new MemoryStream())
      {
        string blobAsync = await blobService.GetBlobAsync(path, blobStream);
        valueTuple = blobAsync != null ? (blobAsync, this.serializer.Deserialize(blobStream)) : ((string) null, (PackageGdprData) null);
      }
      return valueTuple;
    }

    public async Task StoreUploaderData(
      CollectionId collectionId,
      FeedId feedId,
      IProtocol protocol,
      CommitLogBookmark commitLogBookmark,
      DateTime commitUploadDateTime,
      PackageGdprData gdprData)
    {
      IBlobService blobService = this.blobServiceFactory.Get(new ContainerAddress(this.baseDeploymentContainerAddress.CollectionId, new Locator(this.baseDeploymentContainerAddress.Path, new Locator(new string[1]
      {
        commitUploadDateTime.ToString("yyyyMMdd")
      }))));
      Locator path = new Locator(new string[4]
      {
        collectionId.Guid.ToString("N"),
        protocol.CorrectlyCasedName,
        feedId.Guid.ToString("N"),
        commitLogBookmark.CommitId.ToString() + ".txt"
      });
      string str = (string) null;
      Random r = new Random();
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        string etag = (await this.RetrieveUploaderDataCore(path, blobService)).Item1;
        str = await blobService.PutBlobAsync(path, this.serializer.Serialize(gdprData).AsArraySegment(), etag);
      }
      if (str == null)
        throw new ChangeConflictException(string.Format("Could not apply edit for: {0}", (object) path));
      blobService = (IBlobService) null;
      path = (Locator) null;
      r = (Random) null;
    }
  }
}
