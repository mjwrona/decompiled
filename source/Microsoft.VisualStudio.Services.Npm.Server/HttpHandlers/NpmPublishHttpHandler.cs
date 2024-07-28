// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.HttpHandlers.NpmPublishHttpHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument;
using Microsoft.VisualStudio.Services.Npm.Server.Exceptions;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Npm.Server.HttpHandlers
{
  public class NpmPublishHttpHandler : PackageIngestionHttpHandler
  {
    internal const string PackageNameRouteKey = "packageName";
    internal const string ScopedPackageNameRouteKey = "unscopedPackageName";
    internal const string ScopedPackageScopeRouteKey = "packageScope";
    private readonly IFeedCacheService feedCacheService;
    private static readonly IReadOnlyList<HttpMethod> StaticAllowedHttpMethods = (IReadOnlyList<HttpMethod>) ImmutableList.Create<HttpMethod>(HttpMethod.Put);

    protected override string FeedIdRouteKey => "feedId";

    public NpmPublishHttpHandler(IFeedCacheService feedCacheService)
    {
      this.AddExceptionMappings(NpmExceptionMappings.HttpExceptionMapping);
      this.feedCacheService = feedCacheService;
    }

    public override string UserRequestTimeoutSecondsPath => "/Configuration/Npm/Publish/UserRequestTimeoutSeconds";

    protected override string Service => "Npm";

    protected override string Method => "Npm.Publish";

    public override void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData)
    {
      if (routeData.Values["packageName"] == null && (routeData.Values["unscopedPackageName"] == null || routeData.Values["packageScope"] == null))
        throw new InvalidOperationException();
    }

    protected override IReadOnlyList<HttpMethod> AllowedHttpMethods => NpmPublishHttpHandler.StaticAllowedHttpMethods;

    [ControllerMethodTraceFilter(12000000)]
    protected override async Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage request,
      IFeedRequest feedRequest,
      RouteData routeData)
    {
      NpmPublishHttpHandler sendInTheThisObject = this;
      ITracerService tracerService = vssRequestContext.GetTracerFacade();
      ITracerBlock tracer = tracerService.Enter((object) sendInTheThisObject, nameof (AddPackageFromRequestAsync));
      try
      {
        PackageMetadata packageMetadata;
        try
        {
          using (Stream stream = await sendInTheThisObject.ReadStream(vssRequestContext, request))
          {
            if (stream == null || stream.Length == 0L)
              throw new InvalidPublishException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageAddOrUpdateMustHaveBody());
            using (StreamReader streamReader = new StreamReader(stream))
              packageMetadata = PackageJsonUtils.DeserializeNpmJsonDocumentNoThrow<PackageMetadata>((TextReader) streamReader, tracerService, (string) null, (string) null, (string) null, nameof (AddPackageFromRequestAsync), out IReadOnlyList<Exception> _) ?? throw new InvalidPublishException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageAddOrUpdateMustHaveBody());
          }
        }
        catch (IOException ex) when (ex.InnerException is HttpException)
        {
          if (ex.InnerException.Message.Contains("The client disconnected"))
            sendInTheThisObject.HandleClientCancelledException(ex.InnerException);
          throw;
        }
        catch (IOException ex) when (ex.InnerException is TaskCanceledException)
        {
          if (vssRequestContext.IsCanceled)
            sendInTheThisObject.HandleClientCancelledException(ex.InnerException);
          throw;
        }
        catch (TaskCanceledException ex)
        {
          if (vssRequestContext.IsCanceled)
            sendInTheThisObject.HandleClientCancelledException((Exception) ex);
          throw;
        }
        PackageMetadataRequest packageMetadataRequest = new PackageMetadataRequest(feedRequest, packageMetadata);
        NullResult nullResult = await new UpdatePackageDocumentHandlerFactory(vssRequestContext, tracerService).Get(packageMetadataRequest).Handle(packageMetadataRequest);
      }
      finally
      {
        tracer?.Dispose();
      }
      tracerService = (ITracerService) null;
      tracer = (ITracerBlock) null;
    }

    protected override void WriteExceptionResponse(
      Exception ex,
      HttpContextBase httpContext,
      HttpRequestMessage requestMessage)
    {
      NpmResponseMessage npmResponseMessage = NpmResponseMessage.FromWrappedException(new WrappedException(ex, false, requestMessage.GetApiVersion()), (HttpStatusCode) httpContext.Response.StatusCode);
      httpContext.Response.ContentType = "application/json";
      httpContext.Response.Write(JsonConvert.SerializeObject((object) npmResponseMessage, new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      }));
    }

    protected override void ValidatePackageSize(IVssRequestContext requestContext, long packageSize) => IngestionValidationUtils.ValidatePackageSize(requestContext, packageSize);

    protected override void HandleClientCancelledException(Exception e) => throw new ServiceCanceledDueToClientException(e);

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.npm;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage)
    {
      IEnumerable<string> values;
      return requestMessage.Headers.TryGetValues("npm-session", out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    private async Task<Stream> ReadStream(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      Stream stream;
      using (requestContext.CreateTimeToFirstPageExclusionBlock())
        stream = await request.Content.ReadAsStreamAsync();
      return stream;
    }
  }
}
