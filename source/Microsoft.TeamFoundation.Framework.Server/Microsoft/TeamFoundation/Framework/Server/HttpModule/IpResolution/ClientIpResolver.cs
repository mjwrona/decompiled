// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.ClientIpResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal sealed class ClientIpResolver : IClientIpResolver
  {
    private readonly IAfdIpValidator _afdIpValidator;
    private readonly IArrForwardedIpValidator _arrIpValidator;
    private readonly ISmartRouterForwardedIpValidator _smartRouterIpValidator;
    private readonly IIpResolutionMetricCollector _metricCollector;
    private const string c_unknownIp = "unknown";
    private static readonly RegistryQuery s_afdEnabledQuery = (RegistryQuery) "/Configuration/AzureFrontDoor/Enabled";

    public ClientIpResolver(
      IAfdIpValidator afdIpValidator,
      IArrForwardedIpValidator arrIpValidator,
      ISmartRouterForwardedIpValidator smartRouterIpValidator,
      IIpResolutionMetricCollector metricCollector)
    {
      IAfdIpValidator afdIpValidator1 = afdIpValidator;
      IArrForwardedIpValidator forwardedIpValidator1 = arrIpValidator;
      ISmartRouterForwardedIpValidator forwardedIpValidator2 = smartRouterIpValidator;
      IIpResolutionMetricCollector resolutionMetricCollector = metricCollector;
      this._afdIpValidator = afdIpValidator1;
      this._arrIpValidator = forwardedIpValidator1;
      this._smartRouterIpValidator = forwardedIpValidator2;
      this._metricCollector = resolutionMetricCollector;
    }

    public string Resolve(IVssRequestContext requestContext, HttpRequestBase httpRequest)
    {
      string serverVariable = httpRequest.ServerVariables[HttpContextConstants.ResolvedClientIp];
      if (!string.IsNullOrEmpty(serverVariable))
        return serverVariable;
      string ip = requestContext.ExecutionEnvironment.IsHostedDeployment ? this.ResolveIpForHosted(requestContext.To(TeamFoundationHostType.Deployment), httpRequest) : this.ResolveForOnPrem(httpRequest);
      this.CacheResolvedIp(requestContext, httpRequest, ip);
      return ip;
    }

    private bool IsNullOrEmptyOrUnknown(string @string) => string.IsNullOrEmpty(@string) || @string.Equals("unknown", StringComparison.OrdinalIgnoreCase);

    private string ResolveIpForHosted(
      IVssRequestContext deploymentContext,
      HttpRequestBase httpRequest)
    {
      string str = (string) null;
      string forwardedFor;
      SmartRouterIpStatus smartRouterStatus = this.ValidateSmartRouterIfEnabledAndRouted(deploymentContext, httpRequest, out forwardedFor);
      AfdStatus afdValidationStatus;
      ArrStatus arrDimension;
      if (smartRouterStatus == SmartRouterIpStatus.Valid)
      {
        str = forwardedFor;
        afdValidationStatus = AfdStatus.UsedSmartRouter;
        arrDimension = ArrStatus.UsedSmartRouter;
      }
      else
      {
        ArrForwardingValidationHeaders headers = new ArrForwardingValidationHeaders()
        {
          Authorization = httpRequest.Headers["X-Arr-Authorization"],
          ForwardedFor = httpRequest.Headers["X-Arr-Forwarded-For"]
        };
        arrDimension = this._arrIpValidator.Validate(deploymentContext, headers);
        if (arrDimension == ArrStatus.Valid)
        {
          str = headers.ForwardedFor;
          afdValidationStatus = AfdStatus.UsedArr;
        }
        else
        {
          string header = httpRequest.Headers["X-FD-SocketIP"];
          afdValidationStatus = this.ValidateAfdIfEnabledAndRouted(deploymentContext, httpRequest, header);
          switch (afdValidationStatus)
          {
            case AfdStatus.KnownAfdIp:
            case AfdStatus.ValidationDisabled:
              str = header;
              break;
          }
        }
      }
      if (string.IsNullOrEmpty(str))
        str = this.GetSocketIp(httpRequest);
      this._metricCollector.CollectMetric(deploymentContext, arrDimension, afdValidationStatus, smartRouterStatus);
      return str;
    }

    private string GetSocketIp(HttpRequestBase httpRequest)
    {
      try
      {
        return httpRequest.UserHostAddress;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private AfdStatus ValidateAfdIfEnabledAndRouted(
      IVssRequestContext deploymentContext,
      HttpRequestBase httpRequest,
      string xFdSocketIp)
    {
      return deploymentContext.GetService<IVssRegistryService>().GetValue<bool>(deploymentContext, in ClientIpResolver.s_afdEnabledQuery, true) && xFdSocketIp != null ? this._afdIpValidator.Validate(deploymentContext, httpRequest.UserHostAddress) : AfdStatus.Empty;
    }

    private SmartRouterIpStatus ValidateSmartRouterIfEnabledAndRouted(
      IVssRequestContext deploymentContext,
      HttpRequestBase httpRequest,
      out string forwardedFor)
    {
      forwardedFor = (string) null;
      if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.ForwardingIpValidation.Enabled"))
        return SmartRouterIpStatus.ValidationDisabled;
      forwardedFor = httpRequest.Headers["X-SmartRouter-Forwarded-For"];
      return this._smartRouterIpValidator.Validate(deploymentContext, forwardedFor, this.GetSocketIp(httpRequest));
    }

    private string ResolveForOnPrem(HttpRequestBase httpRequest)
    {
      string @string = (string) null;
      try
      {
        @string = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
      }
      catch (Exception ex)
      {
      }
      if (this.IsNullOrEmptyOrUnknown(@string))
        @string = this.GetSocketIp(httpRequest);
      return @string;
    }

    private void CacheResolvedIp(
      IVssRequestContext requestContext,
      HttpRequestBase httpRequest,
      string ip)
    {
      if (!requestContext.IsHostProcessType(HostProcessType.ApplicationTier))
        return;
      httpRequest.ServerVariables[HttpContextConstants.ResolvedClientIp] = ip;
    }
  }
}
