// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.MavenSnapshotCleanupOperationDataGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations
{
  public class MavenSnapshotCleanupOperationDataGeneratingHandler : 
    IAsyncHandler<PackageRequest<MavenPackageIdentity>, MavenSnapshotCleanupOperationData>,
    IHaveInputType<PackageRequest<MavenPackageIdentity>>,
    IHaveOutputType<MavenSnapshotCleanupOperationData>
  {
    private readonly IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataFetchingHandler;
    private readonly IAsyncHandler<FeedCore, SnapshotRotationLimits> snapshotRotationLimitFetchingHandler;
    private readonly IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>> fileToBlobReferenceConverter;

    public MavenSnapshotCleanupOperationDataGeneratingHandler(
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataFetchingHandler,
      IAsyncHandler<FeedCore, SnapshotRotationLimits> snapshotRotationLimitFetchingHandler,
      IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>> fileToBlobReferenceConverter)
    {
      this.metadataFetchingHandler = metadataFetchingHandler;
      this.snapshotRotationLimitFetchingHandler = snapshotRotationLimitFetchingHandler;
      this.fileToBlobReferenceConverter = fileToBlobReferenceConverter;
    }

    public async Task<MavenSnapshotCleanupOperationData> Handle(
      PackageRequest<MavenPackageIdentity> snapshotPackageRequest)
    {
      IMavenMetadataEntry metadataEntry = await this.metadataFetchingHandler.Handle(snapshotPackageRequest);
      if (metadataEntry == null)
        throw ExceptionHelper.PackageNotFound(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_ArtifactNotFound((object) snapshotPackageRequest.PackageId.DisplayStringForMessages, (object) snapshotPackageRequest.Feed.Id));
      SnapshotRotationLimits snapshotRotationLimits = await this.snapshotRotationLimitFetchingHandler.Handle(snapshotPackageRequest.Feed);
      MavenSnapshotMetadataFiles<MavenPackageFileNew> snapshotMetadataFiles = new MavenSnapshotMetadataFiles<MavenPackageFileNew>(metadataEntry.PackageIdentity.Name, (IEnumerable<MavenPackageFileNew>) metadataEntry.PackageFiles, (Func<MavenPackageFileNew, string>) (x => x.Path));
      IDictionary<MavenSnapshotInstanceId, IList<MavenPackageFileNew>> filesByInstances = snapshotMetadataFiles.GetFilesByInstances();
      if (filesByInstances.Keys.Count < snapshotRotationLimits.RotationTargetCount)
        return (MavenSnapshotCleanupOperationData) null;
      IDictionary<MavenSnapshotInstanceId, IList<MavenPackageFileNew>> filesByInstancesToKeepMap = snapshotMetadataFiles.GetNewestSnapshotInstances(snapshotRotationLimits.MinimumSnapshotInstanceCount).GetFilesByInstances();
      List<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>> list1 = filesByInstances.Where<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>>((Func<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>, bool>) (fbi => !filesByInstancesToKeepMap.ContainsKey(fbi.Key))).ToList<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>>();
      List<BlobReferenceIdentifier> list2 = this.fileToBlobReferenceConverter.Convert((IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>) ((IPackageRequest<IPackageIdentity<MavenPackageName, MavenPackageVersion>>) snapshotPackageRequest).ToPackageNameRequest<MavenPackageName, MavenPackageVersion>().WithData<MavenPackageName, IEnumerable<MavenPackageFileNew>>(list1.SelectMany<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>, MavenPackageFileNew>((Func<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>, IEnumerable<MavenPackageFileNew>>) (fbi => (IEnumerable<MavenPackageFileNew>) fbi.Value)))).ToList<BlobReferenceIdentifier>();
      return list2.Any<BlobReferenceIdentifier>() ? new MavenSnapshotCleanupOperationData(snapshotPackageRequest.PackageId, (IList<MavenSnapshotInstanceId>) list1.Select<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>, MavenSnapshotInstanceId>((Func<KeyValuePair<MavenSnapshotInstanceId, IList<MavenPackageFileNew>>, MavenSnapshotInstanceId>) (fbi => fbi.Key)).ToList<MavenSnapshotInstanceId>(), (IEnumerable<BlobReferenceIdentifier>) list2) : (MavenSnapshotCleanupOperationData) null;
    }
  }
}
