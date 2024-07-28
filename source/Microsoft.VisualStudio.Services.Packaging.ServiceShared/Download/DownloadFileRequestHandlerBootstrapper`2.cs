// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadFileRequestHandlerBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadFileRequestHandlerBootstrapper<TPackageId, TMetadataEntry> : 
    IBootstrapper<IAsyncHandler<IPackageFileRequest<TPackageId>, HttpResponseMessage>>
    where TPackageId : class, IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageId>, IPackageFiles
  {
    private readonly IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?>? ingestingHandler;
    private readonly IMetadataCacheService metadataCacheService;
    private readonly IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService;
    private readonly IVssRequestContext requestContext;
    private readonly IReadOnlyList<ISpecificStorageContentProvider> extraContentProviders;
    private readonly IReadOnlyList<ISpecificExtractingStorageContentProvider> extraExtractingContentProviders;

    public DownloadFileRequestHandlerBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService,
      IMetadataCacheService metadataCacheService,
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?>? ingestingHandler,
      IReadOnlyList<ISpecificStorageContentProvider>? extraContentProviders = null,
      IReadOnlyList<ISpecificExtractingStorageContentProvider>? extraExtractingContentProviders = null)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.metadataCacheService = metadataCacheService;
      this.ingestingHandler = ingestingHandler;
      this.extraContentProviders = (IReadOnlyList<ISpecificStorageContentProvider>) ((object) extraContentProviders ?? (object) Array.Empty<ISpecificStorageContentProvider>());
      this.extraExtractingContentProviders = (IReadOnlyList<ISpecificExtractingStorageContentProvider>) ((object) extraExtractingContentProviders ?? (object) Array.Empty<ISpecificExtractingStorageContentProvider>());
    }

    public IAsyncHandler<IPackageFileRequest<TPackageId>, HttpResponseMessage> Bootstrap()
    {
      RetryDecoratingHandler<IPackageFileRequest<TPackageId>, IPackageFileRequest<TPackageId, ContentResult>> currentHandler = new RetryDecoratingHandler<IPackageFileRequest<TPackageId>, IPackageFileRequest<TPackageId, ContentResult>>(new PackageFileRequestToContentResultHandlerBootstrapper<TPackageId, TMetadataEntry>(this.requestContext, this.metadataService, this.metadataCacheService, this.ingestingHandler, this.extraContentProviders, this.extraExtractingContentProviders).Bootstrap().KeepInput<IPackageFileRequest<TPackageId>, ContentResult, IPackageFileRequest<TPackageId, ContentResult>>((Func<IPackageFileRequest<TPackageId>, ContentResult, IPackageFileRequest<TPackageId, ContentResult>>) ((request, response) => request.WithData<TPackageId, ContentResult>(response))), new RetryHelper(this.requestContext.GetTracerFacade(), (IReadOnlyList<TimeSpan>) new List<TimeSpan>()
      {
        TimeSpan.FromMilliseconds(100.0)
      }, (Func<Exception, bool>) (exception => exception.GetType() == typeof (PackageExistsIngestingFromUpstreamException))));
      DownloadPackageFileFromContentResultAsResponseMessageHandler<TPackageId> responseMessageHandler = new DownloadPackageFileFromContentResultAsResponseMessageHandler<TPackageId>();
      PackageMetricsCollector<TPackageId> metricsCollector = new PackageMetricsCollector<TPackageId>(this.requestContext);
      ByAsyncFuncAsyncHandler<IPackageFileRequest<TPackageId>, NullResult> forwardingToThisHandler = new ByAsyncFuncAsyncHandler<IPackageFileRequest<TPackageId>, NullResult>((Func<IPackageFileRequest<TPackageId>, Task<NullResult>>) (async request =>
      {
        await metricsCollector.UpdatePackageMetricsAsync(request.Feed.Id, request.Feed.Project?.Id, request.PackageId, 1.0);
        NullResult nullResult;
        return nullResult;
      }));
      DownloadPackageFileFromContentResultAsResponseMessageHandler<TPackageId> handler = responseMessageHandler;
      return currentHandler.ThenDelegateTo<IPackageFileRequest<TPackageId>, IPackageFileRequest<TPackageId, ContentResult>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<TPackageId, ContentResult>, HttpResponseMessage>) handler).ThenForwardOriginalRequestTo<IPackageFileRequest<TPackageId>, HttpResponseMessage>((IAsyncHandler<IPackageFileRequest<TPackageId>, NullResult>) forwardingToThisHandler);
    }
  }
}
