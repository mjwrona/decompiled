// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenBatchRawRequestValidatingConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenBatchRawRequestValidatingConverter : 
    IConverter<MavenBatchRawRequest, PackagesBatchRequest<MavenPackageIdentity>>,
    IHaveInputType<MavenBatchRawRequest>,
    IHaveOutputType<PackagesBatchRequest<MavenPackageIdentity>>
  {
    private IConverter<IRawPackageRequest, MavenPackageIdentity> identityConverter;
    private IRegistryService registryService;

    public MavenBatchRawRequestValidatingConverter(
      IConverter<IRawPackageRequest, MavenPackageIdentity> identityConverter,
      IRegistryService registryService)
    {
      this.identityConverter = identityConverter;
      this.registryService = registryService;
    }

    public PackagesBatchRequest<MavenPackageIdentity> Convert(MavenBatchRawRequest input)
    {
      MavenPackagesBatchRequest batchRequest = input.BatchRequest;
      this.ValidateBatchRequestSize(batchRequest);
      return new PackagesBatchRequest<MavenPackageIdentity>((IFeedRequest) input, batchRequest.Data, batchRequest.GetOperationType(), (IReadOnlyCollection<IPackageRequest<MavenPackageIdentity>>) batchRequest.Packages.Select<MavenMinimalPackageDetails, PackageRequest<MavenPackageIdentity>>(new Func<MavenMinimalPackageDetails, PackageRequest<MavenPackageIdentity>>(ConvertToPackageRequest)).ToList<PackageRequest<MavenPackageIdentity>>());

      PackageRequest<MavenPackageIdentity> ConvertToPackageRequest(
        MavenMinimalPackageDetails package)
      {
        return new PackageRequest<MavenPackageIdentity>((IFeedRequest) input, this.identityConverter.Convert((IRawPackageRequest) new MavenRawPackageRequest((IFeedRequest) input, package.Group, package.Artifact, package.Version)));
      }
    }

    private void ValidateBatchRequestSize(MavenPackagesBatchRequest batchRequest)
    {
      int maxValue = this.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/MaxPackagesBatchRequestSize", 100);
      BatchRequestUtils.EnsureBatchRequestIsInRange(batchRequest.Packages.Count<MavenMinimalPackageDetails>(), 1, maxValue, "Packages");
    }
  }
}
