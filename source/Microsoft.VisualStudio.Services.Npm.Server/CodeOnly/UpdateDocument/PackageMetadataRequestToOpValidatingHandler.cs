// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument.PackageMetadataRequestToOpValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument
{
  public class PackageMetadataRequestToOpValidatingHandler : 
    IAsyncHandler<
    #nullable disable
    PackageMetadataRequest, ICommitOperationData>,
    IHaveInputType<PackageMetadataRequest>,
    IHaveOutputType<ICommitOperationData>
  {
    private readonly INpmMetadataService metadataService;
    private readonly IAsyncHandler<IRawPackageRequest, NullResult> ingestHandler;
    private readonly ITracerService tracerService;
    private readonly IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>> nameConverter;

    public PackageMetadataRequestToOpValidatingHandler(
      INpmMetadataService metadataService,
      IAsyncHandler<IRawPackageRequest, NullResult> ingestHandler,
      IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>> nameConverter,
      ITracerService tracerService)
    {
      this.metadataService = metadataService;
      this.ingestHandler = ingestHandler;
      this.tracerService = tracerService;
      this.nameConverter = nameConverter;
    }

    public async Task<ICommitOperationData> Handle(PackageMetadataRequest request)
    {
      PackageMetadataRequestToOpValidatingHandler sendInTheThisObject = this;
      ICommitOperationData commitOperationData;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        PackageNameQuery<INpmMetadataEntry> packageNameRequest = new PackageNameQuery<INpmMetadataEntry>((IPackageNameRequest) sendInTheThisObject.nameConverter.Convert((IRawPackageNameRequest) new RawPackageNameRequest((IFeedRequest) request, request.AdditionalData.Name)));
        packageNameRequest.Options = new QueryOptions<INpmMetadataEntry>().WithFilter((Func<INpmMetadataEntry, bool>) (x => !x.IsDeleted()));
        MetadataDocument<INpmMetadataEntry> statesDocumentAsync = await sendInTheThisObject.metadataService.GetPackageVersionStatesDocumentAsync(packageNameRequest);
        if (statesDocumentAsync == null || statesDocumentAsync.Entries == null || statesDocumentAsync.Entries.Count == 0)
          throw new PackageNotFoundException(Resources.Error_PackageNotFound((object) request.AdditionalData.Name, (object) request.Feed.FullyQualifiedName));
        if (statesDocumentAsync.GetRevision() != request.AdditionalData.Revision)
          throw new RevisionMismatchException(Resources.Error_RevisionMismatch());
        IEnumerable<(string, INpmMetadataEntry)> entriesToChange = PackageMetadataRequestToOpValidatingHandler.GetEntriesToChange(statesDocumentAsync.Entries, request.AdditionalData.Versions);
        if (!entriesToChange.Any<(string, INpmMetadataEntry)>())
          throw new RevisionMismatchException(Resources.Error_RevisionMismatch());
        List<NpmDeprecateOperationData> operationList = new List<NpmDeprecateOperationData>();
        foreach ((_, _) in entriesToChange)
        {
          (string, INpmMetadataEntry) pairedEntry;
          NullResult nullResult = await sendInTheThisObject.ingestHandler.Handle((IRawPackageRequest) new RawPackageRequest((IFeedRequest) request, pairedEntry.Item2.PackageIdentity.Name.FullName, pairedEntry.Item2.PackageIdentity.Version.DisplayVersion));
          operationList.Add(new NpmDeprecateOperationData((IPackageIdentity) pairedEntry.Item2.PackageIdentity, pairedEntry.Item1));
          pairedEntry = ();
        }
        commitOperationData = operationList.Count != 1 ? (ICommitOperationData) new BatchCommitOperationData((IReadOnlyCollection<ICommitOperationData>) operationList) : (ICommitOperationData) operationList.Single<NpmDeprecateOperationData>();
      }
      return commitOperationData;
    }

    private static IEnumerable<(string RequestedDeprecate, INpmMetadataEntry ExistingEntry)> GetEntriesToChange(
      List<INpmMetadataEntry> entries,
      IDictionary<string, VersionMetadata> newVersions)
    {
      List<(string, INpmMetadataEntry)> entriesToChange = new List<(string, INpmMetadataEntry)>();
      foreach (INpmMetadataEntry entry in entries)
      {
        if (!newVersions.ContainsKey(entry.PackageIdentity.Version.ToString()))
          throw new RevisionMismatchException(Resources.Error_RevisionMismatch());
        string deprecated = newVersions[entry.PackageIdentity.Version.ToString()].Deprecated;
        if (deprecated != entry.Deprecated)
          entriesToChange.Add((deprecated, entry));
      }
      return (IEnumerable<(string, INpmMetadataEntry)>) entriesToChange;
    }
  }
}
