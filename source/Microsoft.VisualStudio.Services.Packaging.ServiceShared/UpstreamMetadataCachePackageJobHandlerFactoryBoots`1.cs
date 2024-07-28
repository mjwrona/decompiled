// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCachePackageJobHandlerFactoryBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCachePackageJobHandlerFactoryBootstrapper<TPackageName> : 
    IBootstrapper<IFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>>>
    where TPackageName : class, IPackageName
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory;
    private readonly IConverter<string, TPackageName> stringToPackageNameConverter;

    public UpstreamMetadataCachePackageJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory,
      IConverter<string, TPackageName> stringToPackageNameConverter)
    {
      this.requestContext = requestContext;
      this.upstreamMetadataManagerFactory = upstreamMetadataManagerFactory;
      this.stringToPackageNameConverter = stringToPackageNameConverter;
    }

    public IFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>> Bootstrap()
    {
      UpstreamMetadataCachePackageJobHandler<TPackageName> actualHandler = new UpstreamMetadataCachePackageJobHandler<TPackageName>(this.upstreamMetadataManagerFactory, new UpstreamMetadataCacheInfoStoreFactoryBootstrapper(this.requestContext, (IConverter<string, IPackageName>) this.stringToPackageNameConverter).Bootstrap(), this.requestContext.GetTracerFacade());
      return (IFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>>) new ByFuncInputFactory<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>>((Func<IPackageNameRequest<TPackageName>, IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>>) (packageNameRequest => UntilNonNullHandler.Create<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>((IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>) new FeatureFlagCheckingJobHandler<IPackageNameRequest<TPackageName>>(this.requestContext.GetFeatureFlagFacade(), packageNameRequest.Protocol.ReadOnlyFeatureFlagName, true), (IAsyncHandler<IPackageNameRequest<TPackageName, PushDrivenUpstreamsNotificationTelemetry>, JobResult>) actualHandler)));
    }
  }
}
