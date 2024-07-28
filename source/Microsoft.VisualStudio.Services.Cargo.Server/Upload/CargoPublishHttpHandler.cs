// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upload.CargoPublishHttpHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Exceptions;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upload
{
  public class CargoPublishHttpHandler : PackageIngestionHttpHandler
  {
    private readonly IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, NullResult> ingester;
    private readonly IFrotocolLevelPackagingSetting<long> maxPackageSizeSetting;
    private readonly IFrotocolLevelPackagingSetting<int> maxManifestSizeSetting;
    private static readonly IReadOnlyList<HttpMethod> StaticAllowedHttpMethods = (IReadOnlyList<HttpMethod>) ImmutableList.Create<HttpMethod>(HttpMethod.Put);

    public CargoPublishHttpHandler(
      IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, NullResult> ingester,
      IFrotocolLevelPackagingSetting<long> maxPackageSizeSetting,
      IFrotocolLevelPackagingSetting<int> maxManifestSizeSetting)
    {
      this.AddExceptionMappings(CargoExceptionMappings.Mappings);
      this.ingester = ingester;
      this.maxPackageSizeSetting = maxPackageSizeSetting;
      this.maxManifestSizeSetting = maxManifestSizeSetting;
    }

    public override string UserRequestTimeoutSecondsPath => "/Configuration/Cargo/Publish/UserRequestTimeoutSeconds";

    protected override string Service => Protocol.Cargo.CorrectlyCasedName;

    protected override string Method => this.Service + ".Publish";

    protected override string FeedIdRouteKey => "feedId";

    protected override IReadOnlyList<HttpMethod> AllowedHttpMethods => CargoPublishHttpHandler.StaticAllowedHttpMethods;

    public override void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData)
    {
    }

    protected override string? GetClientSessionIdFrom(HttpRequestMessage request) => (string) null;

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.Cargo;

    protected override async Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage httpRequest,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      CargoPublishHttpHandler publishHttpHandler = this;
      FileInfo tempFile = (FileInfo) null;
      try
      {
        BinaryReader reader;
        byte[] rawMetadata;
        FileStream packageStream;
        using (Stream rawStream = await httpRequest.Content.ReadAsStreamAsync())
        {
          reader = new BinaryReader(rawStream);
          try
          {
            int count = reader.ReadInt32();
            int num1 = publishHttpHandler.maxManifestSizeSetting.Get(feedRequest);
            rawMetadata = count <= num1 ? reader.ReadBytes(count) : throw new PackageLimitExceededException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_ManifestLengthExceeded((object) num1));
            int num2 = reader.ReadInt32();
            long num3 = publishHttpHandler.maxPackageSizeSetting.Get(feedRequest);
            if ((long) num2 > num3)
              throw new PackageLimitExceededException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FileLimitExceeded((object) num3));
            tempFile = publishHttpHandler.CreateTemporaryFileForIngestion();
            packageStream = await HttpHandlerHelper.ReadToTempFileAsync(vssRequestContext, rawStream, tempFile.FullName, new long?((long) num2));
            try
            {
              NullResult nullResult = await publishHttpHandler.ingester.Handle(new PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>(feedRequest, new CargoIngestionInput((LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>) null, CargoPublishManifest.LazyDeserialize(DeflateCompressibleBytes.FromUncompressedBytes(rawMetadata)), (IStorageId) null, (Stream) packageStream), "1"));
            }
            finally
            {
              packageStream?.Dispose();
            }
          }
          finally
          {
            reader?.Dispose();
          }
        }
        reader = (BinaryReader) null;
        rawMetadata = (byte[]) null;
        packageStream = (FileStream) null;
      }
      catch (IOException ex) when (ex.InnerException is HttpException)
      {
        if (ex.InnerException.Message.Contains("The client disconnected"))
          publishHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (IOException ex) when (ex.InnerException is TaskCanceledException)
      {
        if (vssRequestContext.IsCanceled)
          publishHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (TaskCanceledException ex)
      {
        if (vssRequestContext.IsCanceled)
          publishHttpHandler.HandleClientCancelledException((Exception) ex);
        throw;
      }
      finally
      {
        if (tempFile != null)
          publishHttpHandler.TryDeleteFile(vssRequestContext, tempFile.FullName);
      }
      tempFile = (FileInfo) null;
    }

    protected override void WriteResponse(
      IVssRequestContext vssRequestContext,
      HttpContextBase httpContext,
      HttpRequestMessage request)
    {
      httpContext.Response.StatusCode = 200;
    }

    protected override void HandleClientCancelledException(Exception e) => throw new ServiceCanceledDueToClientException(e);

    protected override void ValidatePackageSize(IVssRequestContext requestContext, long packageSize)
    {
    }

    private void TryDeleteFile(IVssRequestContext requestContext, string fileName)
    {
      using (ITracerBlock tracerBlock = requestContext.GetTracerFacade().Enter((object) this, nameof (TryDeleteFile)))
      {
        try
        {
          File.Delete(fileName);
        }
        catch (Exception ex)
        {
          tracerBlock.TraceInfo(string.Format("Unable to delete the temporary file {0}. Exception: {1}. Message: {2}", (object) fileName, (object) ex.GetType(), (object) ex.Message));
        }
      }
    }
  }
}
