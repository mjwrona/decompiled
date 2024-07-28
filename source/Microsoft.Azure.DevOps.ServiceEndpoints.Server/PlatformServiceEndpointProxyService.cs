// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformServiceEndpointProxyService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class PlatformServiceEndpointProxyService : 
    IServiceEndpointProxyService2,
    IVssFrameworkService
  {
    private readonly Func<IVssRequestContext, IDictionary<string, string>, string, string, string, string, string, string, JToken, string, HttpRequestProxy> getProxy;
    private int _timeoutInSeconds = 20;
    private const int TokenValidityInMinutes = 10;
    private const string TimeoutInSecondsPath = "/Service/Endpoints/EndpointsProxy/Timeout";

    public PlatformServiceEndpointProxyService()
      : this(PlatformServiceEndpointProxyService.\u003C\u003EO.\u003C0\u003E__GetProxy ?? (PlatformServiceEndpointProxyService.\u003C\u003EO.\u003C0\u003E__GetProxy = new Func<IVssRequestContext, IDictionary<string, string>, string, string, string, string, string, string, JToken, string, HttpRequestProxy>(HttpRequestProxyFactory.GetProxy)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public PlatformServiceEndpointProxyService(
      Func<IVssRequestContext, IDictionary<string, string>, string, string, string, string, string, string, JToken, string, HttpRequestProxy> getProxy)
    {
      this.getProxy = getProxy;
      this.ServiceEndpointSecurity = new ServiceEndpointSecurity();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Endpoints/EndpointsProxy/Timeout");
      this._timeoutInSeconds = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Endpoints/EndpointsProxy/Timeout", this._timeoutInSeconds);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public IList<string> QueryServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding binding,
      HttpRequestProxy proxy = null)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding>(binding, nameof (binding));
      ArgumentUtility.CheckForNull<string>(binding.EndpointId, "EndpointId");
      PlatformServiceEndpointProxyService.ValidateParameters(binding);
      this.ApplySecurityChecks(requestContext, scopeIdentifier, binding.EndpointId, binding.EndpointUrl, (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest) null);
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = PlatformServiceEndpointProxyService.GetServiceEndpoint(requestContext, scopeIdentifier, binding.EndpointId);
      if (serviceEndpoint.IsDisabled && requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.EndpointDisabledError((object) serviceEndpoint.Id));
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType serviceEndpointType = requestContext.GetService<PlatformServiceEndpointTypesService>().GetServiceEndpointTypes(requestContext, serviceEndpoint.Type, serviceEndpoint.Authorization?.Scheme).FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>();
      JToken replacementContext = EndpointMustacheHelper.CreateReplacementContext(serviceEndpoint, binding.Parameters, scopeIdentifier, serviceEndpointType: serviceEndpointType);
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(replacementContext);
      if ((!string.IsNullOrEmpty(binding.DataSourceName) ? 0 : (string.IsNullOrEmpty(binding.EndpointUrl) ? 1 : 0)) != 0)
        return (IList<string>) new List<string>()
        {
          endpointStringResolver.ResolveVariablesInMustacheFormat(binding.ResultTemplate)
        };
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource = PlatformServiceEndpointProxyService.GetDataSource(new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails()
      {
        DataSourceName = binding.DataSourceName,
        DataSourceUrl = binding.EndpointUrl,
        ResultSelector = binding.ResultSelector
      }, serviceEndpointType);
      string endpointUrl = dataSource.EndpointUrl;
      dataSource.EndpointUrl = endpointStringResolver.ResolveVariablesInDollarFormat(serviceEndpoint, dataSource.EndpointUrl, binding.Parameters, serviceEndpointType);
      dataSource.EndpointUrl = endpointStringResolver.ResolveVariablesInMustacheFormat(dataSource.EndpointUrl);
      if (!string.IsNullOrEmpty(dataSource.ResourceUrl))
        dataSource.ResourceUrl = endpointStringResolver.ResolveVariablesInMustacheFormat(dataSource.ResourceUrl);
      bool flag = serviceEndpoint.ShouldAcceptUntrustedCertificates();
      if (proxy == null)
        proxy = this.getProxy(requestContext, (IDictionary<string, string>) null, scopeIdentifier.ToString(), binding.EndpointId, dataSource.EndpointUrl, dataSource.ResourceUrl, dataSource.ResultSelector, binding.ResultTemplate, replacementContext, (string) null);
      if (requestContext != null && requestContext.IsFeatureEnabled("ServiceEndpoints.EnableCustomHeadersInExtensionDataSource"))
        this.ResolveDatasourceHeaders(serviceEndpoint, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) dataSource.Headers);
      HttpStatusCode httpStatusCode = HttpStatusCode.Unused;
      HttpRequestProxy httpRequestProxy = proxy;
      List<TaskSourceDefinition> sourceDefinitions = new List<TaskSourceDefinition>();
      TaskSourceDefinition sourceDefinition = new TaskSourceDefinition();
      sourceDefinition.Endpoint = dataSource.EndpointUrl;
      sourceDefinitions.Add(sourceDefinition);
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> headers = dataSource.Headers;
      int num = flag ? 1 : 0;
      ref HttpStatusCode local1 = ref httpStatusCode;
      string str1;
      ref string local2 = ref str1;
      string str2;
      ref string local3 = ref str2;
      string str3;
      ref string local4 = ref str3;
      int timeoutInSeconds = this._timeoutInSeconds;
      IList<string> result = httpRequestProxy.ExecuteRequest((IList<TaskSourceDefinition>) sourceDefinitions, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) headers, num != 0, ref local1, out local2, out local3, out local4, true, timeoutInSeconds).Result;
      CustomerIntelligenceHelper.PublishServiceEndpointProxyTelemetry(requestContext, httpStatusCode, nameof (QueryServiceEndpoint), endpointUrl, serviceEndpoint, (TaskDefinitionEndpoint) null, (string) null, binding?.ResultTemplate, dataSource.ResultSelector, (Exception) null);
      return result;
    }

    public Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult ExecuteServiceEndpointRequest(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string endpointId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest endpointRequest,
      HttpRequestProxy proxy = null)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest>(endpointRequest, nameof (endpointRequest));
      PlatformServiceEndpointProxyService.ValidateExecuteServiceEndpointRequestParameters(endpointId, endpointRequest);
      this.ApplySecurityChecks(requestContext, scopeIdentifier, endpointId, endpointRequest.DataSourceDetails.DataSourceUrl, endpointRequest);
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSourceObject;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest = PlatformServiceEndpointProxyService.PrepareServiceEndpointRequest(requestContext, scopeIdentifier, endpointId, endpointRequest, out endpoint, out endpointType, out dataSourceObject);
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = serviceEndpointRequest.ServiceEndpointDetails.ToServiceEndpoint(endpointId);
      AuthConfiguration oAuthConfiguration;
      PlatformServiceEndpointProxyService.PatchAuthConfigurationForReplacementContext(requestContext, endpoint, serviceEndpoint, out oAuthConfiguration);
      Dictionary<string, string> wellKnownParameters = MustacheContextHelper.GetWellKnownParameters(requestContext);
      wellKnownParameters.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) serviceEndpointRequest.DataSourceDetails.Parameters);
      JToken replacementContext = EndpointMustacheHelper.CreateReplacementContext(serviceEndpoint, wellKnownParameters, scopeIdentifier, (OAuthConfiguration) oAuthConfiguration, serviceEndpointRequest.DataSourceDetails.InitialContextTemplate, endpointType);
      EndpointStringResolver endpointStringResolver = new EndpointStringResolver(replacementContext);
      string dataSourceUrl = serviceEndpointRequest.DataSourceDetails.DataSourceUrl;
      serviceEndpointRequest.DataSourceDetails.DataSourceUrl = endpointStringResolver.ResolveVariablesInDollarFormat(serviceEndpoint, serviceEndpointRequest.DataSourceDetails.DataSourceUrl, serviceEndpointRequest.DataSourceDetails.Parameters, endpointType);
      serviceEndpointRequest.DataSourceDetails.DataSourceUrl = endpointStringResolver.ResolveVariablesInMustacheFormat(serviceEndpointRequest.DataSourceDetails.DataSourceUrl);
      if (!string.IsNullOrEmpty(serviceEndpointRequest.DataSourceDetails.RequestContent))
        serviceEndpointRequest.DataSourceDetails.RequestContent = endpointStringResolver.ResolveVariablesInMustacheFormat(serviceEndpointRequest.DataSourceDetails.RequestContent);
      if (!string.IsNullOrEmpty(serviceEndpointRequest.DataSourceDetails.ResourceUrl))
        serviceEndpointRequest.DataSourceDetails.ResourceUrl = endpointStringResolver.ResolveVariablesInMustacheFormat(serviceEndpointRequest.DataSourceDetails.ResourceUrl);
      try
      {
        if (!string.IsNullOrEmpty(serviceEndpointRequest.DataSourceDetails.DataSourceUrl))
          serviceEndpointRequest.DataSourceDetails.DataSourceUrl = Regex.Replace(serviceEndpointRequest.DataSourceDetails.DataSourceUrl, "(?<!:)//", "/", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(4000.0));
        if (!string.IsNullOrEmpty(serviceEndpointRequest.DataSourceDetails.ResourceUrl))
          serviceEndpointRequest.DataSourceDetails.ResourceUrl = Regex.Replace(serviceEndpointRequest.DataSourceDetails.ResourceUrl, "(?<!:)//", "/", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(4000.0));
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidEndpointResponseException(ex.Message);
      }
      if (!InternalServiceHelper.IsInternalService(endpoint.Type) && !this.IsTrustedHost(serviceEndpointRequest.DataSourceDetails.DataSourceUrl, endpoint.Url, endpointType?.TrustedHosts))
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.CallToUntrustedHostNotAllowed((object) serviceEndpointRequest.DataSourceDetails.DataSourceUrl));
      HttpStatusCode httpStatusCode = HttpStatusCode.Unused;
      IEndpointAuthorizer endpointAuthorizer = EndpointAuthorizerFactory.GetEndpointAuthorizer(requestContext, serviceEndpoint, scopeIdentifier, dataSourceObject);
      if (requestContext != null && requestContext.IsFeatureEnabled("ServiceEndpoints.EnableCustomHeadersInExtensionDataSource"))
        this.ResolveDatasourceHeaders(serviceEndpoint, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) dataSourceObject?.Headers);
      bool flag = serviceEndpoint.ShouldAcceptUntrustedCertificates();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult endpointRequestResult;
      if (!string.IsNullOrWhiteSpace(serviceEndpointRequest.DataSourceDetails.ResultSelector))
      {
        ResponseSelector responseSelector = ResponseSelectorFactory.GetResponseSelector(serviceEndpointRequest.DataSourceDetails.ResultSelector, (string) null, serviceEndpointRequest.ResultTransformationDetails?.ResultTemplate, replacementContext, serviceEndpointRequest.ResultTransformationDetails?.CallbackContextTemplate, serviceEndpointRequest.ResultTransformationDetails?.CallbackRequiredTemplate);
        IDictionary<string, string> requestParameters = (IDictionary<string, string>) new Dictionary<string, string>();
        if (!serviceEndpointRequest.DataSourceDetails.RequestVerb.IsNullOrEmpty<char>())
        {
          requestParameters.Add("requestVerb", serviceEndpointRequest.DataSourceDetails.RequestVerb);
          requestParameters.Add("requestContent", serviceEndpointRequest.DataSourceDetails.RequestContent);
        }
        else
          requestParameters.Add("requestVerb", "GET");
        if (proxy == null)
          proxy = new HttpRequestProxy(requestContext, scopeIdentifier.ToString("D"), endpointId, serviceEndpointRequest.DataSourceDetails.DataSourceUrl, requestParameters, endpointAuthorizer, serviceEndpointRequest.DataSourceDetails.ResourceUrl, responseSelector);
        HttpRequestProxy httpRequestProxy = proxy;
        List<TaskSourceDefinition> sourceDefinitions = new List<TaskSourceDefinition>();
        TaskSourceDefinition sourceDefinition = new TaskSourceDefinition();
        sourceDefinition.Endpoint = serviceEndpointRequest.DataSourceDetails.DataSourceUrl;
        sourceDefinitions.Add(sourceDefinition);
        List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> headers = dataSourceObject?.Headers;
        int num = flag ? 1 : 0;
        ref HttpStatusCode local1 = ref httpStatusCode;
        string str1;
        ref string local2 = ref str1;
        int timeoutInSeconds = this._timeoutInSeconds;
        ResponseSelectorResult responseSelectorResult = httpRequestProxy.ExecuteRequest((IList<TaskSourceDefinition>) sourceDefinitions, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) headers, num != 0, ref local1, out local2, timeoutInSeconds);
        JArray jarray = new JArray();
        if (responseSelectorResult.Result != null)
        {
          foreach (string str2 in (IEnumerable<string>) responseSelectorResult.Result)
            jarray.Add((JToken) str2);
        }
        endpointRequestResult = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult()
        {
          Result = (JToken) jarray,
          StatusCode = httpStatusCode,
          ErrorMessage = str1,
          ActivityId = requestContext.ActivityId,
          CallbackContextParameters = responseSelectorResult.CallbackContext,
          CallbackRequired = responseSelectorResult.CallbackRequired
        };
      }
      else
      {
        ResponseSelector selector = (ResponseSelector) new HandleBarResponseSelector(serviceEndpointRequest.ResultTransformationDetails?.ResultTemplate, replacementContext);
        if (proxy == null)
          proxy = new HttpRequestProxy(requestContext, scopeIdentifier.ToString("D"), endpointId, serviceEndpointRequest.DataSourceDetails.DataSourceUrl, (IDictionary<string, string>) null, endpointAuthorizer, serviceEndpointRequest.DataSourceDetails.ResourceUrl, selector);
        HttpRequestProxy httpRequestProxy = proxy;
        List<TaskSourceDefinition> sourceDefinitions = new List<TaskSourceDefinition>();
        TaskSourceDefinition sourceDefinition = new TaskSourceDefinition();
        sourceDefinition.Endpoint = serviceEndpointRequest.DataSourceDetails.DataSourceUrl;
        sourceDefinitions.Add(sourceDefinition);
        List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> headers = dataSourceObject?.Headers;
        int num = flag ? 1 : 0;
        ref HttpStatusCode local3 = ref httpStatusCode;
        string str;
        ref string local4 = ref str;
        int timeoutInSeconds = this._timeoutInSeconds;
        ResponseSelectorResult responseSelectorResult = httpRequestProxy.ExecuteRequest((IList<TaskSourceDefinition>) sourceDefinitions, (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) headers, num != 0, ref local3, out local4, timeoutInSeconds);
        endpointRequestResult = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequestResult()
        {
          Result = responseSelectorResult?.Result == null || !responseSelectorResult.Result.Any<string>() ? (JToken) new JArray() : JToken.Parse(responseSelectorResult.Result[0]),
          StatusCode = httpStatusCode,
          ErrorMessage = str,
          ActivityId = requestContext.ActivityId
        };
      }
      CustomerIntelligenceHelper.PublishServiceEndpointProxyTelemetry(requestContext, endpointRequestResult.StatusCode, nameof (ExecuteServiceEndpointRequest), dataSourceUrl, serviceEndpoint, (TaskDefinitionEndpoint) null, (string) null, serviceEndpointRequest.ResultTransformationDetails?.ResultTemplate, serviceEndpointRequest.DataSourceDetails.ResultSelector, (Exception) null);
      return endpointRequestResult;
    }

    private void ResolveDatasourceHeaders(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader> customHeaders)
    {
      if (serviceEndpoint == null || serviceEndpoint.Authorization == null || serviceEndpoint.Authorization.Parameters == null || customHeaders == null)
        return;
      JObject replacementContext = JObject.FromObject((object) new
      {
        endpoint = serviceEndpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) serviceEndpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      });
      MustacheTemplateEngine mustacheTemplateEngine = new MustacheTemplateEngine();
      foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader customHeader in (IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthorizationHeader>) customHeaders)
      {
        if (customHeader != null && customHeader.Name != null && customHeader.Value != null)
        {
          customHeader.Name = customHeader.Name.Trim();
          customHeader.Value = mustacheTemplateEngine.EvaluateTemplate(customHeader.Value.Trim(), (JToken) replacementContext);
        }
      }
    }

    private static void PatchAuthConfigurationForReplacementContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpointFromDetails,
      out AuthConfiguration oAuthConfiguration)
    {
      Guid configurationId = Guid.Empty;
      oAuthConfiguration = (AuthConfiguration) null;
      if (!PlatformServiceEndpointProxyService.ContainsOAuthConfigurationId(serviceEndpoint) && !PlatformServiceEndpointProxyService.ContainsOAuthConfigurationId(serviceEndpointFromDetails))
        return;
      if (PlatformServiceEndpointProxyService.ContainsOAuthConfigurationId(serviceEndpoint))
        configurationId = new Guid(serviceEndpoint.Authorization.Parameters["ConfigurationId"]);
      if (PlatformServiceEndpointProxyService.ContainsOAuthConfigurationId(serviceEndpointFromDetails))
        configurationId = new Guid(serviceEndpointFromDetails.Authorization.Parameters["ConfigurationId"]);
      oAuthConfiguration = (AuthConfiguration) null;
      if (!(configurationId != Guid.Empty))
        return;
      IOAuthConfigurationService2 service = requestContext.GetService<IOAuthConfigurationService2>();
      oAuthConfiguration = service.GetAuthConfiguration(requestContext, configurationId);
      service.ReadAuthConfigurationSecrets(requestContext, oAuthConfiguration);
    }

    private static bool ContainsOAuthConfigurationId(Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint) => serviceEndpoint != null && serviceEndpoint.Authorization?.Scheme != null && serviceEndpoint.Authorization.Scheme.Equals("OAuth2", StringComparison.InvariantCultureIgnoreCase) && serviceEndpoint.Authorization.Parameters != null && serviceEndpoint.Authorization.Parameters.ContainsKey("ConfigurationId");

    private static void ValidateParameters(Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceBinding binding)
    {
      int num = !string.IsNullOrEmpty(binding.DataSourceName) ? 1 : 0;
      if ((num & (string.IsNullOrEmpty(binding.EndpointUrl) ? (false ? 1 : 0) : (!string.IsNullOrEmpty(binding.ResultSelector) ? 1 : 0))) != 0)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDataSourceBindingException(ServiceEndpointResources.InvalidDataSourceBindingBothMentioned());
      if (num != 0 && !Guid.TryParse(binding.EndpointId, out Guid _))
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDataSourceBindingException(ServiceEndpointResources.InvalidDataSourceBindingEndpointIdInvalid());
    }

    private static void ValidateExecuteServiceEndpointRequestParameters(
      string endpointId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest)
    {
      PlatformServiceEndpointProxyService.ValidateServiceEndpointInput(endpointId, serviceEndpointRequest.ServiceEndpointDetails);
      PlatformServiceEndpointProxyService.ValidateDataSourceDetails(serviceEndpointRequest.DataSourceDetails, serviceEndpointRequest.ResultTransformationDetails, serviceEndpointRequest.ServiceEndpointDetails);
    }

    private static void ValidateServiceEndpointInput(
      string serviceEndpointId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails serviceEndpointDetails)
    {
      Guid result;
      int num = !Guid.TryParse(serviceEndpointId, out result) || result.Equals(Guid.Empty) ? (InternalServiceHelper.IsInternalService(serviceEndpointId) ? 1 : 0) : 1;
      bool flag = serviceEndpointDetails != null;
      if (num == 0 && !flag)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.ServiceEndpointIdAndDetailsBothMissingError());
      string errorMessage;
      if (flag && !serviceEndpointDetails.Validate(out errorMessage))
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(errorMessage);
    }

    private static void ValidateDataSourceDetails(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails dataSourceDetails,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ResultTransformationDetails resultTransformationDetails,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails serviceEndpointDetails)
    {
      if (dataSourceDetails == null)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.DataSourceDetailsMissingError());
      bool flag1 = !string.IsNullOrWhiteSpace(dataSourceDetails.DataSourceName);
      bool flag2 = !string.IsNullOrWhiteSpace(dataSourceDetails.DataSourceUrl);
      bool flag3 = resultTransformationDetails != null && !string.IsNullOrWhiteSpace(resultTransformationDetails.CallbackContextTemplate);
      bool flag4 = resultTransformationDetails != null && !string.IsNullOrWhiteSpace(resultTransformationDetails.CallbackRequiredTemplate);
      bool flag5 = !string.IsNullOrWhiteSpace(dataSourceDetails.InitialContextTemplate);
      int num1 = !string.IsNullOrWhiteSpace(dataSourceDetails.ResourceUrl) ? 1 : 0;
      int num2 = flag3 & flag4 & flag5 ? 1 : (flag3 || flag4 ? 0 : (!flag5 ? 1 : 0));
      if (flag1 & flag2)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.DataSourceNameAndUrlBothMentionedError());
      if (!flag1 && !flag2)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.DataSourceNameAndUrlBothMissingError());
      if (num2 == 0)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.PaginationFieldsMissing());
      if (num1 != 0)
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.ResourceUrlMentionedError());
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest PrepareServiceEndpointRequest(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string endpointId,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest endpointRequest,
      out Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      out Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSourceObject)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest = endpointRequest.Clone();
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails serviceEndpointDetails = serviceEndpointRequest.ServiceEndpointDetails;
      PlatformServiceEndpointTypesService service = requestContext.GetService<PlatformServiceEndpointTypesService>();
      endpointType = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType) null;
      if (!string.IsNullOrEmpty(serviceEndpointDetails?.Type))
      {
        serviceEndpointDetails.ConvertToSupportedAuthenticationScheme();
        endpointType = service.GetServiceEndpointTypes(requestContext, serviceEndpointDetails.Type, serviceEndpointDetails.Authorization?.Scheme).FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>();
      }
      Guid result;
      if ((!Guid.TryParse(endpointId, out result) ? 0 : (!result.Equals(Guid.Empty) ? 1 : 0)) != 0)
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = PlatformServiceEndpointProxyService.GetServiceEndpoint(requestContext.Elevate(), scopeIdentifier, endpointId);
        if (serviceEndpoint.IsDisabled && requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
          throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidServiceEndpointRequestException(ServiceEndpointResources.EndpointDisabledError((object) serviceEndpoint.Id));
        if (!string.IsNullOrEmpty(serviceEndpoint.Type) && endpointType == null)
        {
          serviceEndpoint.ConvertToSupportedAuthenticationScheme();
          endpointType = service.GetServiceEndpointTypes(requestContext, serviceEndpoint.Type, serviceEndpoint.Authorization?.Scheme).FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType>();
        }
        if (serviceEndpointRequest.ServiceEndpointDetails != null)
        {
          endpointType.TryGetAuthInputDescriptors(serviceEndpointRequest.ServiceEndpointDetails.Authorization.Scheme, out List<InputDescriptor> _);
          ServiceEndpointValidator.ValidateUrlChange(serviceEndpoint, serviceEndpointRequest.ServiceEndpointDetails, endpointType);
          ServiceEndpointHelper.PatchEndpointSecrets(serviceEndpointRequest.ServiceEndpointDetails.Authorization?.Parameters, serviceEndpoint.Authorization?.Parameters);
        }
        else
          serviceEndpointRequest.ServiceEndpointDetails = serviceEndpoint.ToServiceEndpointDetails();
      }
      endpoint = serviceEndpointRequest.ServiceEndpointDetails.ToServiceEndpoint(endpointId);
      endpoint.GetAdditionalServiceEndpointDetails(requestContext, endpointType);
      if (serviceEndpointRequest.ServiceEndpointDetails != null)
        serviceEndpointRequest.ServiceEndpointDetails.Data = endpoint.Data;
      dataSourceObject = (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource) null;
      if (serviceEndpointRequest.DataSourceDetails != null && !string.IsNullOrWhiteSpace(serviceEndpointRequest.DataSourceDetails.DataSourceName))
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource = PlatformServiceEndpointProxyService.GetDataSource(serviceEndpointRequest.DataSourceDetails, endpointType);
        dataSourceObject = dataSource;
        PlatformServiceEndpointProxyService.OverrideDataSourceDetailsWithDataSourceProperties(serviceEndpointRequest, dataSource);
        if (serviceEndpointRequest.ServiceEndpointDetails != null && dataSource.AuthenticationScheme != null)
          PlatformServiceEndpointProxyService.OverrideAuthenticationScheme(requestContext, scopeIdentifier, dataSource, serviceEndpointRequest, endpointId);
      }
      return serviceEndpointRequest;
    }

    private static void OverrideDataSourceDetailsWithDataSourceProperties(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest computedEndpointRequest,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource)
    {
      computedEndpointRequest.DataSourceDetails.DataSourceUrl = dataSource.EndpointUrl;
      computedEndpointRequest.DataSourceDetails.ResourceUrl = dataSource.ResourceUrl;
      if (!dataSource.RequestVerb.IsNullOrEmpty<char>())
        computedEndpointRequest.DataSourceDetails.RequestVerb = dataSource.RequestVerb;
      if (!dataSource.RequestContent.IsNullOrEmpty<char>())
        computedEndpointRequest.DataSourceDetails.RequestContent = dataSource.RequestContent;
      if (!dataSource.ResultSelector.IsNullOrEmpty<char>())
        computedEndpointRequest.DataSourceDetails.ResultSelector = dataSource.ResultSelector;
      computedEndpointRequest.DataSourceDetails.InitialContextTemplate = dataSource.InitialContextTemplate;
      if (computedEndpointRequest.ResultTransformationDetails == null)
        return;
      if (!string.IsNullOrEmpty(dataSource.CallbackRequiredTemplate))
        computedEndpointRequest.ResultTransformationDetails.CallbackRequiredTemplate = dataSource.CallbackRequiredTemplate;
      if (string.IsNullOrEmpty(dataSource.CallbackContextTemplate))
        return;
      computedEndpointRequest.ResultTransformationDetails.CallbackContextTemplate = dataSource.CallbackContextTemplate;
    }

    private static void OverrideAuthenticationScheme(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest,
      string endpointId)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.AuthenticationSchemeReference authenticationScheme = dataSource.AuthenticationScheme;
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, authenticationScheme.Type);
      string str = contribution != null ? (string) contribution.Properties.GetValue("name") : throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDatasourceException(ServiceEndpointResources.AuthSchemeForOverrideNotFound());
      if (string.IsNullOrEmpty(str))
        return;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = serviceEndpointRequest.ServiceEndpointDetails.ToServiceEndpoint(endpointId);
      AuthConfiguration oAuthConfiguration;
      PlatformServiceEndpointProxyService.PatchAuthConfigurationForReplacementContext(requestContext, serviceEndpoint, (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null, out oAuthConfiguration);
      JToken replacementContext = EndpointMustacheHelper.CreateReplacementContext(serviceEndpoint, serviceEndpointRequest.DataSourceDetails.Parameters, scopeIdentifier, (OAuthConfiguration) oAuthConfiguration);
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) serviceEndpoint.Authorization.Parameters)
        replacementContext.Last.AddAfterSelf((object) new JProperty(parameter.Key, (object) parameter.Value));
      MustacheTemplateParser templateParser = new MustacheTemplateParser();
      PlatformServiceEndpointProxyService.RegisterHelpers(requestContext, serviceEndpoint, templateParser);
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization endpointAuthorization = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization();
      endpointAuthorization.Scheme = str;
      if (authenticationScheme.Inputs != null)
      {
        foreach (string key in authenticationScheme.Inputs.Keys)
          endpointAuthorization.Parameters.Add(key, templateParser.ReplaceValues(authenticationScheme.Inputs[key], (object) replacementContext));
      }
      serviceEndpointRequest.ServiceEndpointDetails.Authorization = endpointAuthorization;
      try
      {
        new ServiceEndpointValidator(requestContext).ValidateEndpointFields(serviceEndpointRequest.ServiceEndpointDetails.Authorization.Parameters, (IList<InputDescriptor>) ServiceEndpointContributionExtensions.GetInputDescriptors(contribution.Properties), "endpoint.Authorization.Parameters", false);
      }
      catch (ArgumentException ex)
      {
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDatasourceException(ServiceEndpointResources.DatasourceAuthSchemeOverridesDoNotMatchAuthSchemeInputs());
      }
    }

    private static void RegisterHelpers(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      MustacheTemplateParser templateParser)
    {
      if (!serviceEndpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase))
        return;
      templateParser.RegisterHelper("#GetAzureStorageAccessKey", new MustacheTemplateHelperMethod(new AzureRMAuthorizationHelpers(requestContext, serviceEndpoint).GetStorageAccessKey));
      templateParser.RegisterHelper("#GetKubeConfig", new MustacheTemplateHelperMethod(new AzureRMAuthorizationHelpers(requestContext, serviceEndpoint).GetKubeConfig));
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string serviceEndpointId)
    {
      Guid result;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint;
      if (Guid.TryParse(serviceEndpointId, out result))
      {
        serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, scopeIdentifier, result, refreshAuthenticationParameters: new RefreshAuthenticationParameters()
        {
          EndpointId = result,
          TokenValidityInMinutes = (short) 10
        });
        if (serviceEndpoint == null || string.IsNullOrWhiteSpace(serviceEndpoint.Type))
          throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointNotFoundException(ServiceEndpointResources.ServiceEndPointNotFound((object) serviceEndpointId));
      }
      else
      {
        List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> list = requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpointsAsync(requestContext, scopeIdentifier, string.Empty, (IEnumerable<string>) null, (IEnumerable<string>) new List<string>()
        {
          serviceEndpointId
        }, (string) null, false).Result.ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
        serviceEndpoint = list.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>() ? list.First<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>() : new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint();
      }
      return serviceEndpoint;
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource GetDataSource(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails dataSourceDetails,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      dataSourceDetails.Parameters.Add("paginationEnabled", "true");
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource;
      if (!string.IsNullOrWhiteSpace(dataSourceDetails.DataSourceName))
      {
        string dataSourceName = new EndpointStringResolver(EndpointMustacheHelper.CreateReplacementContext((Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null, dataSourceDetails.Parameters)).ResolveVariablesInMustacheFormat(dataSourceDetails.DataSourceName);
        dataSource = PlatformServiceEndpointProxyService.GetDataSourceByName(endpointType, dataSourceName);
      }
      else
        dataSource = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource()
        {
          EndpointUrl = dataSourceDetails.DataSourceUrl,
          ResourceUrl = dataSourceDetails.ResourceUrl,
          ResultSelector = dataSourceDetails.ResultSelector,
          Headers = dataSourceDetails.Headers,
          InitialContextTemplate = dataSourceDetails.InitialContextTemplate
        };
      return dataSource;
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource GetDataSourceByName(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      string dataSourceName)
    {
      if (endpointType?.DataSources != null)
      {
        foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSource dataSource in endpointType.DataSources)
        {
          if (string.Equals(dataSource.Name, dataSourceName, StringComparison.OrdinalIgnoreCase))
            return dataSource;
        }
      }
      throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceNotFoundException(ServiceEndpointResources.DataSourceNotFound((object) dataSourceName));
    }

    private void ApplySecurityChecks(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string serviceEndpointId,
      string dataSourceUrl,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest serviceEndpointRequest)
    {
      if (!string.IsNullOrWhiteSpace(dataSourceUrl) && !dataSourceUrl.StartsWith("{{endpoint.url}}") && !dataSourceUrl.StartsWith("{{{endpoint.url}}}") && !dataSourceUrl.StartsWith("{{configuration.Url}}") && !dataSourceUrl.StartsWith("{{{configuration.Url}}}"))
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidDataSourceBindingException(ServiceEndpointResources.ShouldStartWithEndpointOrConfigurationUrlError());
      Guid result;
      bool flag1 = Guid.TryParse(serviceEndpointId, out result) && !result.Equals(Guid.Empty);
      bool flag2 = serviceEndpointRequest != null && serviceEndpointRequest.ServiceEndpointDetails != null;
      if (scopeIdentifier.Equals(Guid.Empty))
        this.ServiceEndpointSecurity.CheckCallerIsServicePrincipal(requestContext, Guid.Empty.ToString("D"));
      else if (flag1 & flag2)
      {
        Func<IVssRequestContext, string> getErrorMessageFunc = (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForAdminOperation());
        this.ServiceEndpointSecurity.CheckPermission(requestContext, scopeIdentifier.ToString("D"), serviceEndpointId, 2, false, getErrorMessageFunc);
      }
      else if (flag1)
      {
        this.ServiceEndpointSecurity.CheckPermission(requestContext, scopeIdentifier.ToString("D"), serviceEndpointId, 1, true, (Func<IVssRequestContext, string>) (context => ServiceEndpointResources.EndpointAccessDeniedForUseOperation()));
      }
      else
      {
        if (InternalServiceHelper.IsInternalService(serviceEndpointId))
          return;
        this.ServiceEndpointSecurity.CheckFrameworkReadPermissions(requestContext);
      }
    }

    private bool IsTrustedHost(string url, Uri endpointUrl, List<string> trustedHosts)
    {
      if (!this.IsAbsolute(url) || trustedHosts == null || trustedHosts.Count == 0)
        return true;
      string host = new Uri(url).Host;
      if (endpointUrl != (Uri) null)
        trustedHosts.Add(endpointUrl.Host);
      return trustedHosts.Any<string>((Func<string, bool>) (trustedDomain => UrlHelper.IsSubDomain(host, trustedDomain)));
    }

    private bool IsAbsolute(string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri _);

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this._timeoutInSeconds = changedEntries.GetValueFromPath<int>("/Service/Endpoints/EndpointsProxy/Timeout", this._timeoutInSeconds);
    }

    public ServiceEndpointSecurity ServiceEndpointSecurity { get; set; }
  }
}
