// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument.UpdatePackageDocumentHandlerFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument
{
  public class UpdatePackageDocumentHandlerFactory : 
    IFactory<PackageMetadataRequest, IAsyncHandler<PackageMetadataRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ITracerService tracerService;

    public UpdatePackageDocumentHandlerFactory(
      IVssRequestContext requestContext,
      ITracerService tracerService)
    {
      this.requestContext = requestContext;
      this.tracerService = tracerService;
    }

    public IAsyncHandler<PackageMetadataRequest, NullResult> Get(
      PackageMetadataRequest metadataRequest)
    {
      using (this.tracerService.Enter((object) this, nameof (Get)))
      {
        if (metadataRequest.AdditionalData.IsUpstreamCached)
          throw new InvalidPackageException(Resources.Error_CannotCreateOrUpdateCachedPackage());
        return metadataRequest.AdditionalData.Attachments != null && metadataRequest.AdditionalData.Attachments.Any<KeyValuePair<string, Attachment>>() ? (IAsyncHandler<PackageMetadataRequest, NullResult>) new PackageIngestionHandler((INpmIngestionService) new NpmPackageIngestionServiceFacade(this.requestContext)) : NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageMetadataRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<PackageMetadataRequest, NullResult>>) new UpdateVersionsHandlerBootstrapper(this.requestContext));
      }
    }
  }
}
