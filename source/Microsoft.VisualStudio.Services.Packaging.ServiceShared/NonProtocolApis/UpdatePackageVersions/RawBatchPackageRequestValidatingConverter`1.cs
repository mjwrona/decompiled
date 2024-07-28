// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.RawBatchPackageRequestValidatingConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public class RawBatchPackageRequestValidatingConverter<TIdentity> : 
    IConverter<IBatchRawRequest, PackagesBatchRequest<TIdentity>>,
    IHaveInputType<IBatchRawRequest>,
    IHaveOutputType<PackagesBatchRequest<TIdentity>>
    where TIdentity : IPackageIdentity
  {
    private readonly IConverter<IRawPackageRequest, TIdentity> identityConverter;
    private readonly IRegistryService registryService;

    public RawBatchPackageRequestValidatingConverter(
      IConverter<IRawPackageRequest, TIdentity> identityConverter,
      IRegistryService registryService)
    {
      this.identityConverter = identityConverter;
      this.registryService = registryService;
    }

    public PackagesBatchRequest<TIdentity> Convert(IBatchRawRequest input)
    {
      IPackagesBatchRequest packagesBatchRequest = input.BatchRequest ?? throw new ArgumentNullException("BatchRequest");
      int maxValue = this.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/MaxPackagesBatchRequestSize", 100);
      BatchRequestUtils.EnsureBatchRequestIsInRange((packagesBatchRequest.Packages ?? throw new ArgumentNullException("Packages")).Count<MinimalPackageDetails>(), 1, maxValue, "Packages");
      return new PackagesBatchRequest<TIdentity>((IFeedRequest) input, packagesBatchRequest.Data, packagesBatchRequest.GetOperationType(), (IReadOnlyCollection<IPackageRequest<TIdentity>>) packagesBatchRequest.Packages.Select<MinimalPackageDetails, PackageRequest<TIdentity>>((Func<MinimalPackageDetails, PackageRequest<TIdentity>>) (p => this.ConvertPackageToRequest(input, p))).ToList<PackageRequest<TIdentity>>());
    }

    private PackageRequest<TIdentity> ConvertPackageToRequest(
      IBatchRawRequest batchRawRequest,
      MinimalPackageDetails minimalPackageDetails)
    {
      if (minimalPackageDetails == null)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ValueCannotBeNull((object) nameof (minimalPackageDetails)));
      TIdentity packageId = this.identityConverter.Convert((IRawPackageRequest) new RawPackageRequest((IFeedRequest) batchRawRequest, minimalPackageDetails.Id, minimalPackageDetails.Version));
      return new PackageRequest<TIdentity>((IFeedRequest) batchRawRequest, packageId);
    }
  }
}
