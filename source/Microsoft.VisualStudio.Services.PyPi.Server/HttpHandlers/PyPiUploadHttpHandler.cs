// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.HttpHandlers.PyPiUploadHttpHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.HttpHandlers
{
  public class PyPiUploadHttpHandler : PackageIngestionHttpHandler
  {
    private readonly 
    #nullable disable
    IFeedCacheService feedCacheService;
    private readonly IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> ingester;
    private static readonly IReadOnlyList<HttpMethod> StaticAllowedHttpMethods = (IReadOnlyList<HttpMethod>) ImmutableList.Create<HttpMethod>(HttpMethod.Post);

    protected override string FeedIdRouteKey => "feedId";

    public PyPiUploadHttpHandler(
      IFeedCacheService feedCacheService,
      IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> ingester)
    {
      this.AddExceptionMappings(PyPiExceptionMappings.Mappings);
      this.feedCacheService = feedCacheService;
      this.ingester = ingester;
    }

    public override string UserRequestTimeoutSecondsPath => "/Configuration/" + Protocol.PyPi.CorrectlyCasedName + "/Push/UserRequestTimeoutSeconds";

    protected override string Service => Protocol.PyPi.CommitLogItemType;

    protected override string Method => this.Service + ".Upload";

    public override void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData)
    {
      if (!request.Content.IsMimeMultipartContent())
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.UnsupportedMediaTypeException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnexpectedMediaType((object) "multipart/form-data"));
    }

    protected override IReadOnlyList<HttpMethod> AllowedHttpMethods => PyPiUploadHttpHandler.StaticAllowedHttpMethods;

    protected override async Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage request,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      PyPiUploadHttpHandler uploadHttpHandler = this;
      MultipartFormDataStreamProvider multipartFormDataStreamProvider = (MultipartFormDataStreamProvider) null;
      try
      {
        if (!PyPiFeatureFlags.PyPiEnabled.Bootstrap(vssRequestContext).Get())
          throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
        string fullName = uploadHttpHandler.CreateTemporaryDirectoryForIngestion().FullName;
        using (vssRequestContext.CreateTimeToFirstPageExclusionBlock())
          multipartFormDataStreamProvider = await HttpHandlerHelper.ReadMultipartFormAndFileDataFromRequest(vssRequestContext, request, fullName);
        NameValueCollection formData = multipartFormDataStreamProvider.FormData;
        if (formData == null || ((IEnumerable<string>) formData.AllKeys).IsNullOrEmpty<string>())
          throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingMultipartFormData());
        Collection<MultipartFileData> fileData = multipartFormDataStreamProvider.FileData;
        if (fileData.IsNullOrEmpty<MultipartFileData>())
          throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingMultipartFileData());
        MultipartFileData multipartFileData1 = fileData.FirstOrDefault<MultipartFileData>((Func<MultipartFileData, bool>) (f => PyPiUploadHttpHandler.UnquoteToken(f.Headers?.ContentDisposition?.Name) == "content"));
        if (multipartFileData1 == null)
          throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingContentPart());
        string FilePath1 = PyPiUploadHttpHandler.UnquoteToken(multipartFileData1.Headers.ContentDisposition.FileName);
        if (string.IsNullOrWhiteSpace(FilePath1))
          throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingContentFilename());
        if (multipartFileData1.Headers.ContentType != null && !VssStringComparer.ContentType.Equals(multipartFileData1.Headers.ContentType.MediaType, "application/octet-stream"))
          throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_ContentMediaTypeNotApplicationOctetStream());
        MultipartFileData multipartFileData2 = fileData.FirstOrDefault<MultipartFileData>((Func<MultipartFileData, bool>) (f => PyPiUploadHttpHandler.UnquoteToken(f.Headers?.ContentDisposition?.Name) == "gpg_signature"));
        bool flag = multipartFileData2 != null;
        string FilePath2 = (string) null;
        if (flag)
        {
          FilePath2 = PyPiUploadHttpHandler.UnquoteToken(multipartFileData2.Headers.ContentDisposition.FileName);
          if (string.IsNullOrWhiteSpace(FilePath2))
            throw new InvalidPublishRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_MissingGpgSignatureFilename());
        }
        Dictionary<string, string[]> dictionary = ((IEnumerable<string>) formData.AllKeys).Where<string>((Func<string, bool>) (k => !k.Equals("action"))).ToDictionary<string, string, string[]>((Func<string, string>) (key => key), new Func<string, string[]>(formData.GetValues));
        using (FileStream packageStream = new FileStream(multipartFileData1.LocalFileName, FileMode.Open))
        {
          using (FileStream gpgSignatureStream = flag ? new FileStream(multipartFileData2.LocalFileName, FileMode.Open) : (FileStream) null)
          {
            NullResult nullResult = await uploadHttpHandler.ingester.Handle(new PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>(feedRequest, new PackageIngestionFormData((IReadOnlyDictionary<string, string[]>) dictionary, new PackageFileStream(FilePath1, packageStream.Length, (Stream) packageStream, (BlobIdentifier) null), flag ? new PackageFileStream(FilePath2, gpgSignatureStream.Length, (Stream) gpgSignatureStream, (BlobIdentifier) null) : (PackageFileStream) null), "1"));
          }
        }
      }
      catch (IOException ex) when (ex.InnerException is HttpException)
      {
        if (ex.InnerException.Message.Contains("The client disconnected"))
          uploadHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (IOException ex) when (ex.InnerException is TaskCanceledException)
      {
        if (vssRequestContext.IsCanceled)
          uploadHttpHandler.HandleClientCancelledException(ex.InnerException);
        throw;
      }
      catch (TaskCanceledException ex)
      {
        if (vssRequestContext.IsCanceled)
          uploadHttpHandler.HandleClientCancelledException((Exception) ex);
        throw;
      }
      finally
      {
        uploadHttpHandler.DeleteTemporaryFiles(vssRequestContext, multipartFormDataStreamProvider?.FileData);
      }
      multipartFormDataStreamProvider = (MultipartFormDataStreamProvider) null;
    }

    private static string UnquoteToken(string token) => string.IsNullOrWhiteSpace(token) || !token.StartsWith("\"", StringComparison.Ordinal) || !token.EndsWith("\"", StringComparison.Ordinal) || token.Length <= 1 ? token : token.Substring(1, token.Length - 2);

    private void DeleteTemporaryFiles(
      IVssRequestContext requestContext,
      Collection<MultipartFileData> fileData)
    {
      if (fileData == null)
        return;
      foreach (MultipartFileData multipartFileData in fileData)
        PyPiUploadHttpHandler.TryDeleteFile(requestContext, multipartFileData.LocalFileName);
    }

    private static void TryDeleteFile(IVssRequestContext requestContext, string fileName)
    {
      try
      {
        File.Delete(fileName);
      }
      catch (Exception ex)
      {
        IDictionary<string, object> items = requestContext.Items;
        Guid? feedId = (items != null ? items.GetValueOrDefault<string, object>("Packaging.Feed") : (object) null) is FeedCore valueOrDefault ? new Guid?(valueOrDefault.Id) : new Guid?();
        requestContext.Trace(TracingUtils.GetTracePointFor(feedId, (IProtocol) Protocol.PyPi, (IHasher<uint>) FNVHasher32.Instance), TraceLevel.Warning, Protocol.PyPi.CorrectlyCasedName, nameof (TryDeleteFile), string.Format("Unable to delete the temporary file {0}. Exception: {1}. Message: {2}", (object) fileName, (object) ex.GetType(), (object) ex.Message));
      }
    }

    protected override void ValidatePackageSize(IVssRequestContext requestContext, long packageSize) => PackageIngestionUtils.ValidateMaxPackageSize(requestContext, Protocol.PyPi.CorrectlyCasedName, packageSize);

    protected override void HandleClientCancelledException(Exception e) => throw new ServiceCanceledDueToClientException(e);

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.PyPi;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    protected override void WriteResponse(
      IVssRequestContext vssRequestContext,
      HttpContextBase httpContext,
      HttpRequestMessage request)
    {
      httpContext.Response.StatusCode = 200;
    }
  }
}
