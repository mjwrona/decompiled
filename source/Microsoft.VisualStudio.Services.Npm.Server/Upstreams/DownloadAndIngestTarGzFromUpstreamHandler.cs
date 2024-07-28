// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.DownloadAndIngestTarGzFromUpstreamHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class DownloadAndIngestTarGzFromUpstreamHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageFileRequest<NpmPackageIdentity, IStorageId>, ContentResult>,
    IHaveInputType<IPackageFileRequest<NpmPackageIdentity, IStorageId>>,
    IHaveOutputType<ContentResult>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamNpmClient>> upstreamClientFactory;
    private readonly INpmIngestionService npmIngestionService;
    private readonly ITracerService tracerService;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly ICache<string, object> requestContextItems;
    private readonly IFeedViewVisibilityValidator upstreamValidator;
    private readonly IOrgLevelPackagingSetting<bool> propagateDeprecateSetting;

    public DownloadAndIngestTarGzFromUpstreamHandler(
      IFactory<UpstreamSource, Task<IUpstreamNpmClient>> upstreamClientFactory,
      INpmIngestionService npmIngestionService,
      ITracerService tracerService,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      ICache<string, object> requestContextItems,
      IFeedViewVisibilityValidator upstreamValidator,
      IOrgLevelPackagingSetting<bool> propagateDeprecateSetting)
    {
      this.upstreamClientFactory = upstreamClientFactory;
      this.npmIngestionService = npmIngestionService;
      this.tracerService = tracerService;
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.requestContextItems = requestContextItems;
      this.upstreamValidator = upstreamValidator;
      this.propagateDeprecateSetting = propagateDeprecateSetting;
    }

    public async Task<ContentResult> Handle(
      IPackageFileRequest<NpmPackageIdentity, IStorageId> request)
    {
      DownloadAndIngestTarGzFromUpstreamHandler sendInTheThisObject = this;
      UpstreamStorageId additionalData1 = request.AdditionalData as UpstreamStorageId;
      TryAllUpstreamsStorageId additionalData2 = request.AdditionalData as TryAllUpstreamsStorageId;
      if (additionalData1 == null && additionalData2 == null)
        return (ContentResult) null;
      UpstreamSourceInfo upstreamContentSource = additionalData1?.UpstreamContentSource;
      IEnumerable<UpstreamSource> upstreamSources = sendInTheThisObject.GetValidUpstreamSources((IFeedRequest) request, upstreamContentSource);
      if (upstreamSources == null || !upstreamSources.Any<UpstreamSource>())
        throw new ArgumentException("expected a valid upstream. found none.", "AdditionalData");
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        using (CancellationTokenSource tokenSource = new CancellationTokenSource())
        {
          CancellationToken cancellationToken = tokenSource.Token;
          foreach (UpstreamSource upstreamSource in upstreamSources)
          {
            await sendInTheThisObject.upstreamValidator.Validate((IFeedRequest) request, upstreamSource);
            IUpstreamNpmClient client = await sendInTheThisObject.upstreamClientFactory.Get(upstreamSource);
            try
            {
              Stream stream = (Stream) await client.GetPackageContentStreamAsync(request.Feed, request.PackageId.Name, request.PackageId.Version, cancellationToken);
              try
              {
                if (stream != null)
                {
                  PackageVersionInternalMetadata internalMetadata = await client.GetPackageInternalMetadata(request.PackageId.Name, request.PackageId.Version);
                  sendInTheThisObject.requestContextItems.Set("Packaging.Properties.PackageSource", (object) "upstream");
                  sendInTheThisObject.requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) upstreamSource.Id);
                  string deprecateMessage = sendInTheThisObject.propagateDeprecateSetting.Get() ? internalMetadata.DeprecateMessage : (string) null;
                  await sendInTheThisObject.npmIngestionService.SaveStreamToFeedAsync(request.Feed, new UpstreamPackageContent(internalMetadata.SourceChain, stream), request.PackageId, deprecateMessage);
                  stream.Seek(0L, SeekOrigin.Begin);
                  ContentResult contentResult = new ContentResult(stream, request.FilePath);
                  stream = (Stream) null;
                  return contentResult;
                }
              }
              finally
              {
                stream?.Dispose();
              }
              stream = (Stream) null;
            }
            catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
            {
            }
            client = (IUpstreamNpmClient) null;
          }
          cancellationToken = new CancellationToken();
        }
      }
      throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFoundInUpstreams((object) request.PackageId.Name.FullName, (object) string.Join(",", upstreamSources.Select<UpstreamSource, string>((Func<UpstreamSource, string>) (s => s.Location))), (object) request.Feed.FullyQualifiedName));
    }

    private IEnumerable<UpstreamSource> GetValidUpstreamSources(
      IFeedRequest feedRequest,
      UpstreamSourceInfo source)
    {
      IEnumerable<UpstreamSource> source1 = this.upstreamsFromFeedHandler.Handle(feedRequest);
      if (source == null)
        return source1;
      UpstreamSource upstreamSource = source1.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (s => s.Id == source.Id));
      if (upstreamSource == null)
        return source1;
      return (IEnumerable<UpstreamSource>) new UpstreamSource[1]
      {
        upstreamSource
      };
    }
  }
}
