// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.HttpConfigurationExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class HttpConfigurationExtensions
  {
    private const string ETagHandlerKey = "Microsoft.AspNet.OData.ETagHandler";
    private const string TimeZoneInfoKey = "Microsoft.AspNet.OData.TimeZoneInfo";
    private const string UrlKeyDelimiterKey = "Microsoft.AspNet.OData.UrlKeyDelimiterKey";
    private const string ContinueOnErrorKey = "Microsoft.AspNet.OData.ContinueOnErrorKey";
    private const string NullDynamicPropertyKey = "Microsoft.AspNet.OData.NullDynamicPropertyKey";
    private const string ContainerBuilderFactoryKey = "Microsoft.AspNet.OData.ContainerBuilderFactoryKey";
    private const string RootContainerMappingsKey = "Microsoft.AspNet.OData.RootContainerMappingsKey";
    private const string DefaultQuerySettingsKey = "Microsoft.AspNet.OData.DefaultQuerySettings";
    private const string NonODataRootContainerKey = "Microsoft.AspNet.OData.NonODataRootContainerKey";
    private const string HttpRequestMessageKey = "MS_HttpRequestMessage";
    private const string DefaultCollection = "DefaultCollection";
    internal const string ODataPrefix = "_odata";
    internal static readonly List<IODataRoutingConvention> AnalyticsRoutingConventions = new List<IODataRoutingConvention>()
    {
      (IODataRoutingConvention) new AnalyticsODataRoutingConvention("AnalyticsController"),
      (IODataRoutingConvention) new FunctionRoutingConvention(),
      (IODataRoutingConvention) new UnmappedRequestRoutingConvention()
    };
    private const string c_publicProjectRequestRestrictionsAppliedKey = "PublicProjectRequestRestrictionsAppliedKey";
    private static readonly PublicProjectRequestRestrictionsAttribute s_publicProjectRequestRestrictionsAttribute = new PublicProjectRequestRestrictionsAttribute().WithMechanismsToAdvertise(AuthenticationMechanisms.Basic | AuthenticationMechanisms.Windows | AuthenticationMechanisms.Federated | AuthenticationMechanisms.OAuth | AuthenticationMechanisms.FederatedRedirectOnlyIfAcceptHtml);

    public static ODataRoute MapODataServiceRoute(
      this HttpConfiguration configuration,
      string routeName,
      string routePrefix)
    {
      Action<IContainerBuilder> configureAction = (Action<IContainerBuilder>) (builder => builder.AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, (Func<IServiceProvider, IEnumerable<IODataRoutingConvention>>) (sp => (IEnumerable<IODataRoutingConvention>) HttpConfigurationExtensions.AnalyticsRoutingConventions)).AddService<IEdmModel>(ServiceLifetime.Scoped, (Func<IServiceProvider, IEdmModel>) (sp => HttpConfigurationExtensions.GetCreateModel(sp))).AddService<ODataSerializerProvider, AnalyticsODataSerializerProvider>(ServiceLifetime.Singleton).AddService<ODataPrimitiveSerializer, FastODataPrimitiveSerializer>(ServiceLifetime.Singleton).AddService<ODataResourceSerializer, FastODataResourceSerializer>(ServiceLifetime.Singleton).AddService<ODataResourceSetSerializer, AnnotatedODataResourceSerializer>(ServiceLifetime.Singleton).AddService<IODataPathHandler, AnalyticsODataPathHandler>(ServiceLifetime.Singleton).AddService<DefaultQuerySettings>(ServiceLifetime.Singleton, (Func<IServiceProvider, DefaultQuerySettings>) (sp => AnalyticsODataDefaults.DefaultQuerySettings)).AddService<ODataValidationSettings>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataValidationSettings>) (sp => AnalyticsODataDefaults.DefaultValidationSettings)).AddServicePrototype<ODataMessageWriterSettings>(AnalyticsODataDefaults.DefaultODataMessageWriterSettings).AddService<ODataBatchHandler>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataBatchHandler>) (sp => (ODataBatchHandler) new AnalyticsODataBatchHandler(GlobalConfiguration.DefaultServer))).AddService<ODataUriResolver>(ServiceLifetime.Singleton, (Func<IServiceProvider, ODataUriResolver>) (sp => (ODataUriResolver) new AnalyticsODataUriResolver())).AddService<SkipTokenHandler, AnalyticsSkipTokenHandler>(ServiceLifetime.Singleton));
      ODataRoute odataRoute = configuration.MapODataServiceRoute(routeName, routePrefix, configureAction);
      odataRoute.Constraints["TFS_HostType"] = (object) new TfsApiHostTypeConstraint(TeamFoundationHostType.ProjectCollection);
      return odataRoute;
    }

    private static IEdmModel GetCreateModel(IServiceProvider sp)
    {
      IVssRequestContext vssRequestContext = HttpContext.Current.GetVssRequestContext();
      try
      {
        string virtualPath = vssRequestContext.VirtualPath();
        string path = HttpContext.Current.Request.Path;
        string collectionName = vssRequestContext.ExecutionEnvironment.IsHostedDeployment ? "DefaultCollection" : vssRequestContext.ServiceHost.Name;
        (string str, int num) = HttpConfigurationExtensions.ParseUrl(vssRequestContext, virtualPath, path, collectionName);
        vssRequestContext.SetODataModelVersion(num);
        if (vssRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
          HttpContext.Current.Response.AddHeader("X-Analytics-OData-Model-Version", num.ToString());
        if (!string.IsNullOrEmpty(str) && !vssRequestContext.Items.ContainsKey("PublicProjectRequestRestrictionsAppliedKey"))
        {
          HttpConfigurationExtensions.s_publicProjectRequestRestrictionsAttribute.ApplyRequestRestrictions(vssRequestContext, (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "project",
              (object) str
            }
          });
          vssRequestContext.Items.Add("PublicProjectRequestRestrictionsAppliedKey", (object) true);
        }
        vssRequestContext.ValidateIdentity();
        vssRequestContext.TraceODataModelVersion();
        return vssRequestContext.GetService<AnalyticsMetadataService>().GetModel(vssRequestContext, str).EdmModel;
      }
      catch (UnauthorizedRequestException ex)
      {
        HttpApplicationWrapper application = new HttpApplicationWrapper(HttpContext.Current.ApplicationInstance);
        TeamFoundationApplicationCore.CompleteRequest(vssRequestContext, (IHttpApplication) application, HttpStatusCode.Unauthorized, (Exception) ex);
        throw;
      }
      catch (ODataUnrecognizedPathException ex)
      {
        HttpRequestMessage httpRequestMessage = HttpContext.Current.GetHttpRequestMessage();
        int num = httpRequestMessage.ShouldIncludeErrorDetail() ? 1 : 0;
        Version apiVersion = httpRequestMessage.GetApiVersion();
        WrappedException wrappedException = new WrappedException((Exception) ex, num != 0, apiVersion);
        HttpStatusCode statusCode = HttpStatusCode.NotFound;
        throw new HttpResponseException(httpRequestMessage.CreateResponse<WrappedException>(statusCode, wrappedException));
      }
      catch (Exception ex)
      {
        HttpContext.Current.GetHttpRequestMessage().ThrowWrappedException(ex);
        throw;
      }
    }

    internal static (string projectName, int odataModelVersion) ParseUrl(
      IVssRequestContext requestContext,
      string virtualPath,
      string path,
      string collectionName)
    {
      string str = (string) null;
      int num = ModelVersionExtension.GetDefaultVersion();
      requestContext.SetRequestVersioned(false);
      virtualPath.Split('/');
      string[] array = path.Split('/');
      int index1 = Array.FindIndex<string>(array, (Predicate<string>) (x => x == "_odata"));
      if (index1 >= 0)
      {
        int index2 = index1 - 1;
        int index3 = index1 + 1;
        if (0 <= index2 && !string.IsNullOrWhiteSpace(array[index2]) && array[index2] != "_odata" && !virtualPath.Contains(array[index2]) && array[index2] != collectionName)
          str = array[index2];
        if (index3 < array.Length && !string.IsNullOrWhiteSpace(array[index3]) && !array[index3].StartsWith("$") && !AnalyticsModelBuilder.s_entityNames.Contains(array[index3]))
        {
          num = ModelVersionExtension.ExtractODataModelVersion(requestContext, array[index3]);
          if (num == ModelVersionExtension.GetDefaultVersion())
            throw new ODataUnrecognizedPathException("Resource not found for the segment '" + array[index3] + "'.");
        }
      }
      if (num == ModelVersionExtension.GetDefaultVersion() && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new UnsupportedODataModelVersionException(AnalyticsResources.ODATA_NOT_SUPPORTED_VERSION_PRIVATE_PREVIEW(), requestContext);
      return (str, num);
    }

    public static IVssRequestContext GetVssRequestContext(this HttpContext httpContext) => httpContext.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext;

    public static HttpRequestMessage GetHttpRequestMessage(this HttpContext httpContext) => httpContext.Items[(object) "MS_HttpRequestMessage"] as HttpRequestMessage;

    public static PublicProjectRequestRestrictionsAttribute WithMechanismsToAdvertise(
      this PublicProjectRequestRestrictionsAttribute attribute,
      AuthenticationMechanisms value)
    {
      return new PublicProjectRequestRestrictionsAttribute(false, true, (string) null, attribute.RequiredAuthentication, attribute.AllowNonSsl, attribute.AllowCORS, value, attribute.Description, attribute.AgentFilterType, attribute.AgentFilter, attribute.ProjectParameter);
    }
  }
}
