// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageIngestionHttpHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers
{
  public abstract class PackageIngestionHttpHandler : IHttpAsyncHandler, IHttpHandler
  {
    private const string Layer = "HttpHandler";
    private readonly ApiExceptionMapping apiExceptionMapping;

    protected PackageIngestionHttpHandler() => this.apiExceptionMapping = new ApiExceptionMapping();

    public abstract string UserRequestTimeoutSecondsPath { get; }

    public bool IsReusable => false;

    protected abstract string Service { get; }

    protected virtual string Area => "BufferlessAsyncHttpHandler";

    protected abstract string Method { get; }

    protected abstract string FeedIdRouteKey { get; }

    public void ProcessRequest(HttpContext context) => throw new InvalidOperationException();

    public IAsyncResult BeginProcessRequest(
      HttpContext context,
      AsyncCallback cb,
      object extraData)
    {
      HttpContextWrapper httpContext = new HttpContextWrapper(context);
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) httpContext.ApplicationInstance).VssRequestContext;
      vssRequestContext.RequestTimer.SetPreControllerTime();
      vssRequestContext.ValidateIdentity();
      return this.ProcessRequestAsync((HttpContextBase) httpContext).ToApm(cb, extraData);
    }

    public void EndProcessRequest(IAsyncResult result) => result.WaitForApmConvertedTask();

    public abstract void ValidateRequestParameters(HttpRequestMessage request, RouteData routeData);

    protected abstract IReadOnlyList<HttpMethod> AllowedHttpMethods { get; }

    protected void AddExceptionMappings(Dictionary<Type, HttpStatusCode> mappings)
    {
      foreach (KeyValuePair<Type, HttpStatusCode> mapping in mappings)
        this.apiExceptionMapping.AddStatusCode(mapping.Key, mapping.Value);
    }

    protected async Task ProcessRequestMessageAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage httpRequest,
      HttpContextBase httpContext)
    {
      PackageIngestionHttpHandler sendInTheThisObject = this;
      using (vssRequestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (ProcessRequestMessageAsync)))
      {
        IFeatureFlagService featureFlagFacade = vssRequestContext.GetFeatureFlagFacade();
        RouteData routeData = httpContext.Request.RequestContext.RouteData;
        vssRequestContext.RequestTimeout = sendInTheThisObject.GetRequestTimeout(vssRequestContext);
        IProtocol protocol = sendInTheThisObject.GetProtocol();
        vssRequestContext.Items["Packaging.Protocol"] = (object) protocol;
        vssRequestContext.Items["Packaging.ClientSessionId"] = (object) sendInTheThisObject.GetClientSessionIdFrom(httpRequest);
        ProjectInfo projectFromRoute = ProjectUtility.GetProjectFromRoute(vssRequestContext, (IDictionary<string, object>) routeData.Values);
        string feedIdFromRequest = sendInTheThisObject.GetFeedIdFromRequest(routeData);
        IFeedRequest feedRequest = RequestInitHelper.GetFeedRequest(vssRequestContext, RequestInitHelper.GetUserSuppliedProjectNameOrId(httpContext), projectFromRoute != null ? projectFromRoute.Id : Guid.Empty, protocol, feedIdFromRequest, FeedValidator.GetFeedIsNotReadOnlyValidator());
        vssRequestContext.SetPackagingTracesInfoFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
        if (!protocol.EnabledSettingDefinition.Bootstrap(vssRequestContext).Get())
          throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
        string onlyFeatureFlagName = protocol.ReadOnlyFeatureFlagName;
        if (featureFlagFacade.IsEnabled(onlyFeatureFlagName))
          throw new FeatureReadOnlyException(Resources.Error_ServiceReadOnly());
        if (!sendInTheThisObject.AllowedHttpMethods.Contains<HttpMethod>(httpRequest.Method))
          throw Microsoft.VisualStudio.Services.Packaging.ServiceShared.ExceptionHelper.HttpMethodNotAllowed(httpRequest.Method);
        sendInTheThisObject.ValidateRequestParameters(httpRequest, routeData);
        long? contentLength = httpRequest.Content.Headers.ContentLength;
        if (contentLength.HasValue)
        {
          PackageIngestionHttpHandler ingestionHttpHandler = sendInTheThisObject;
          IVssRequestContext requestContext = vssRequestContext;
          contentLength = httpRequest.Content.Headers.ContentLength;
          long packageSize = contentLength.Value;
          ingestionHttpHandler.ValidatePackageSize(requestContext, packageSize);
        }
        try
        {
          await sendInTheThisObject.AddPackageFromRequestAsync(vssRequestContext, httpRequest, feedRequest, routeData);
        }
        catch (HttpException ex)
        {
          if (!httpContext.Response.IsClientConnected)
            sendInTheThisObject.HandleClientCancelledException((Exception) ex);
          throw;
        }
        httpContext.Response.StatusCode = 202;
        string str = new ValueFromCacheFactory<string>("Packaging.OperationId", (ICache<string, object>) new RequestContextItemsAsCacheFacade(vssRequestContext)).Get();
        if (str != null)
          httpContext.Response.AddHeader("X-Packaging-OperationId", str);
        sendInTheThisObject.WriteResponse(vssRequestContext, httpContext, httpRequest);
      }
    }

    protected abstract string? GetClientSessionIdFrom(HttpRequestMessage request);

    protected abstract IProtocol GetProtocol();

    protected virtual void WriteResponse(
      IVssRequestContext vssRequestContext,
      HttpContextBase httpContext,
      HttpRequestMessage request)
    {
    }

    protected abstract Task AddPackageFromRequestAsync(
      IVssRequestContext vssRequestContext,
      HttpRequestMessage request,
      IFeedRequest feedRequest,
      RouteData routeData);

    protected virtual void HandleException(
      Exception ex,
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      HttpRequestMessage requestMessage)
    {
      this.SetExceptionResponse(ex, requestContext, httpContext);
      this.WriteExceptionResponse(ex, httpContext, requestMessage);
    }

    protected virtual void SetExceptionResponse(
      Exception ex,
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      requestContext.Status = ex;
      requestContext.TraceException(103200, this.Area, "HttpHandler", ex);
      httpContext.Response.TrySkipIisCustomErrors = true;
      HttpStatusCode? nullable = this.apiExceptionMapping.GetStatusCode(ex.GetType());
      if (!nullable.HasValue)
      {
        TeamFoundationServiceException serviceException = ex as TeamFoundationServiceException;
        nullable = new HttpStatusCode?(serviceException != null ? serviceException.HttpStatusCode : HttpStatusCode.InternalServerError);
      }
      httpContext.Response.StatusCode = (int) nullable.Value;
      if (httpContext.Response.HeadersWritten || !ErrorInReasonPhraseExceptionFilter.ShouldEmitWarning(ex))
        return;
      PackagingWarningMessage warningMessage = ErrorInReasonPhraseExceptionFilter.ComputeWarningMessage(ex, nullable.Value, requestContext.ActivityId, requestContext.ExecutionEnvironment.IsOnPremisesDeployment, httpContext.Response.StatusDescription);
      this.ApplyWarningMessageToResponse(httpContext, warningMessage);
    }

    protected virtual void WriteExceptionResponse(
      Exception ex,
      HttpContextBase httpContext,
      HttpRequestMessage requestMessage)
    {
      WrappedException wrappedException = new WrappedException(ex, false, requestMessage.GetApiVersion());
      httpContext.Response.ContentType = "application/json";
      httpContext.Response.Write(JsonConvert.SerializeObject((object) wrappedException, new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      }));
    }

    protected abstract void HandleClientCancelledException(Exception e);

    protected abstract void ValidatePackageSize(IVssRequestContext requestContext, long packageSize);

    private static HttpRequestMessage RequestMessageFromRequest(HttpContextBase context)
    {
      HttpRequestBase request1 = context.Request;
      HttpRequestMessage request2 = new HttpRequestMessage(new HttpMethod(request1.HttpMethod), request1.Url)
      {
        Content = (HttpContent) new StreamContent(request1.GetBufferlessInputStream(true))
      };
      foreach (string header in (NameObjectCollectionBase) request1.Headers)
      {
        string[] values = request1.Headers.GetValues(header);
        if (!request2.Headers.TryAddWithoutValidation(header, (IEnumerable<string>) values))
          request2.Content.Headers.TryAddWithoutValidation(header, (IEnumerable<string>) values);
      }
      request2.Properties[TfsApiPropertyKeys.HttpContext] = (object) context;
      request2.SetRequestContext(new HttpRequestContext());
      return request2;
    }

    private string GetFeedIdFromRequest(RouteData routeData)
    {
      if (routeData.Values[this.FeedIdRouteKey] == null)
        throw new InvalidOperationException();
      return routeData.Values[this.FeedIdRouteKey].ToString();
    }

    private async Task ProcessRequestAsync(HttpContextBase httpContext)
    {
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) httpContext.ApplicationInstance).VssRequestContext;
      HttpRequestMessage requestMessage = PackageIngestionHttpHandler.RequestMessageFromRequest(httpContext);
      vssRequestContext.ServiceName = this.Service;
      try
      {
        string str1 = vssRequestContext.ActivityId.ToString("D");
        if (string.IsNullOrEmpty(httpContext.Response.Headers["ActivityId"]) || !httpContext.Response.Headers["ActivityId"].Contains(str1))
          httpContext.Response.Headers.Add("ActivityId", str1);
        string str2 = vssRequestContext.UniqueIdentifier.ToString("D");
        if (string.IsNullOrEmpty(httpContext.Response.Headers["X-TFS-Session"]) || !httpContext.Response.Headers["X-TFS-Session"].Contains(str2))
          httpContext.Response.Headers.Add("X-TFS-Session", str2);
        httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        vssRequestContext.EnterMethod(new MethodInformation(this.Method, MethodType.Normal, EstimatedMethodCost.Low));
        await this.ProcessRequestMessageAsync(vssRequestContext, requestMessage, httpContext);
        vssRequestContext.LeaveMethod();
        vssRequestContext = (IVssRequestContext) null;
        requestMessage = (HttpRequestMessage) null;
      }
      catch (Exception ex)
      {
        this.HandleException(ex, vssRequestContext, httpContext, requestMessage);
        vssRequestContext = (IVssRequestContext) null;
        requestMessage = (HttpRequestMessage) null;
      }
      finally
      {
        requestMessage.DisposeRequestResources();
        requestMessage.Dispose();
        vssRequestContext.RequestTimer.SetControllerTime();
      }
    }

    private TimeSpan GetRequestTimeout(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) this.UserRequestTimeoutSecondsPath, true, 1200));

    protected virtual void ApplyWarningMessageToResponse(
      HttpContextBase httpContext,
      PackagingWarningMessage message)
    {
      httpContext.Response.StatusDescription = message.StatusDescriptionMessage;
    }

    protected FileInfo CreateTemporaryFileForIngestion() => IngestionTemporaryFileHelper.CreateTemporaryFileForIngestion(this.GetProtocol());

    protected DirectoryInfo CreateTemporaryDirectoryForIngestion() => IngestionTemporaryFileHelper.CreateTemporaryDirectoryForIngestion(this.GetProtocol());
  }
}
