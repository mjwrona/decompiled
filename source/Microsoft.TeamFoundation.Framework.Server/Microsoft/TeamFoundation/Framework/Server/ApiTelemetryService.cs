// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApiTelemetryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ApiTelemetryService : VssBaseService, IVssFrameworkService
  {
    private string[] m_ignoredUserAgents = Array.Empty<string>();
    private const string c_registryRootPath = "/Configuration/ApiTelemetry";
    private const string c_ignoredUserAgentsRegistryPath = "/Configuration/ApiTelemetry/IgnoredUserAgents";
    private static readonly char[] s_ignoredUserAgentsListSeparator = new char[1]
    {
      '|'
    };
    private const string CIAreaName = "Area";
    private const string CIResourceName = "Resource";
    private const string CIMethodName = "Method";
    private const string CIRequestedApiVersionName = "RequestedApiVersion";
    private const string CIRequestedResourceVersionName = "RequestedResourceVersion";
    private const string CIRawApiVersionName = "RawApiVersion";
    private const string CIRawResVersionName = "RawResourceVersion";
    private const string CIActualApiVersionName = "ActualApiVersion";
    private const string CIActualResourceVersionName = "ActualResourceVersion";
    private const string CIAuthName = "Auth";
    private const string CIAppIDName = "AppID";
    private const string CIResponseCodeName = "ResponseCode";
    private static readonly string AreaName = "RestApi Tracking";
    private static readonly string FeatureName = "RestApi";
    private static readonly string s_area = "RestApi Tracking";
    private static readonly string s_layer = "CI events";
    public const string ApiTelemetryFeatureName = "VisualStudio.FrameworkService.RestApiTelemtry";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.TraceEnter(10021001, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, "ServiceStart");
      try
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/Configuration/ApiTelemetry/*");
        this.LoadSettings(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10021002, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10021003, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.TraceEnter(10021011, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, "ServiceEnd");
      try
      {
        systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10021012, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10021013, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, "ServiceEnd");
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10021021, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, nameof (LoadSettings));
      try
      {
        string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/ApiTelemetry/IgnoredUserAgents", string.Empty);
        if (!string.IsNullOrWhiteSpace(str))
          this.m_ignoredUserAgents = str.Split(ApiTelemetryService.s_ignoredUserAgentsListSeparator, StringSplitOptions.RemoveEmptyEntries);
        else
          this.m_ignoredUserAgents = Array.Empty<string>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10021022, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10021023, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, nameof (LoadSettings));
      }
    }

    internal CustomerIntelligenceData GetRestTelemetry(
      IVssRequestContext requestContext,
      TfsApiController tfsController)
    {
      CustomerIntelligenceData restTelemetry = new CustomerIntelligenceData();
      restTelemetry.Add("Area", VersionedApiResourceConstraint.GetAreaFromRouteData(tfsController.RequestContext.RouteData));
      restTelemetry.Add("Resource", VersionedApiResourceConstraint.GetResourceFromRouteData(tfsController.RequestContext.RouteData));
      restTelemetry.Add("Method", tfsController.Request.Method.ToString());
      string apiVersion = (string) null;
      string resourceVersion = (string) null;
      tfsController.Request.ParseApiResourceVersionAndResourceVersion(out apiVersion, out resourceVersion);
      if (!string.IsNullOrEmpty(apiVersion))
        restTelemetry.Add("RawApiVersion", apiVersion);
      if (!string.IsNullOrEmpty(resourceVersion))
        restTelemetry.Add("RawResourceVersion", resourceVersion);
      ApiResourceVersion apiResourceVersion = tfsController.Request.GetApiResourceVersion();
      if (apiResourceVersion != null)
      {
        restTelemetry.Add("RequestedApiVersion", apiResourceVersion.ApiVersionString);
        restTelemetry.Add("RequestedResourceVersion", (double) apiResourceVersion.ResourceVersion);
      }
      ApiResourceVersion versionFromRouteData = VersionedApiResourceConstraint.GetApiVersionFromRouteData(tfsController.RequestContext.RouteData);
      if (versionFromRouteData != null)
      {
        restTelemetry.Add("ActualApiVersion", versionFromRouteData.ApiVersionString);
        restTelemetry.Add("ActualResourceVersion", (double) versionFromRouteData.ResourceVersion);
      }
      string str;
      if (tfsController.TfsRequestContext.RootContext.Items.TryGetValue<string>(RequestContextItemsKeys.ClientId, out str))
        restTelemetry.Add("AppID", str);
      return restTelemetry;
    }

    internal bool ShouldLog(IVssRequestContext requestContext, Type controllerType)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.RestApiTelemtry"))
        return false;
      ApiTelemetryAttribute[] customAttributes = controllerType.GetCustomAttributes(typeof (ApiTelemetryAttribute), true) as ApiTelemetryAttribute[];
      if (customAttributes.Length == 0 || !customAttributes[0].EnableLogging)
        return false;
      IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
      return (!(authenticatedDescriptor != (IdentityDescriptor) null) || !ServicePrincipals.IsServicePrincipal(requestContext, authenticatedDescriptor)) && (customAttributes[0].SkipUserAgentChecking || !this.IsRequestUserAgentToBeIgnored(requestContext));
    }

    private bool IsRequestUserAgentToBeIgnored(IVssRequestContext tfsRequestContext)
    {
      string userAgent = tfsRequestContext.UserAgent;
      return string.IsNullOrWhiteSpace(userAgent) || ((IEnumerable<string>) this.m_ignoredUserAgents).Any<string>((Func<string, bool>) (ignoredUserAgent => userAgent.IndexOf(ignoredUserAgent, StringComparison.OrdinalIgnoreCase) != -1));
    }

    internal void AddResponseCode(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData,
      int responseCode)
    {
      ciData.Add("ResponseCode", (double) responseCode);
    }

    internal void PublishTelemetry(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, ApiTelemetryService.AreaName, ApiTelemetryService.FeatureName, ciData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10021031, ApiTelemetryService.s_area, ApiTelemetryService.s_layer, ex);
      }
    }
  }
}
