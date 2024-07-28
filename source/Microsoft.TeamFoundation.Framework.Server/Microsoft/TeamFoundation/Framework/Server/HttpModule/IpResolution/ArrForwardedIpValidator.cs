// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.ArrForwardedIpValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal sealed class ArrForwardedIpValidator : IArrForwardedIpValidator
  {
    private static readonly string s_Area = "HttpModule";
    private static readonly string s_Layer = nameof (ArrForwardedIpValidator);

    public ArrStatus Validate(
      IVssRequestContext requestContext,
      ArrForwardingValidationHeaders headers)
    {
      requestContext.TraceEnter(60110, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, nameof (Validate));
      ArrStatus result = this.ValidateIpAndToken(requestContext, headers);
      this.TraceValidationResult(requestContext, result);
      requestContext.TraceLeave(60111, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, nameof (Validate));
      return result;
    }

    private ArrStatus ValidateIpAndToken(
      IVssRequestContext requestContext,
      ArrForwardingValidationHeaders headers)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableArrForwardingIpValidation") || headers.ForwardedFor == null && headers.Authorization == null)
        return ArrStatus.Empty;
      if (string.IsNullOrWhiteSpace(headers.ForwardedFor) || string.IsNullOrWhiteSpace(headers.Authorization) || !IPAddress.TryParse(headers.ForwardedFor, out IPAddress _))
        return ArrStatus.Invalid;
      IOAuth2AuthenticationService service = requestContext.GetService<IOAuth2AuthenticationService>();
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.EnableS2SArrValidator") && this.ValidateToken(service, requestContext, headers, OAuth2TokenValidators.S2S_ARR) == ArrStatus.Valid ? ArrStatus.Valid : this.ValidateToken(service, requestContext, headers, OAuth2TokenValidators.S2S);
    }

    private ArrStatus ValidateToken(
      IOAuth2AuthenticationService tokenValidator,
      IVssRequestContext requestContext,
      ArrForwardingValidationHeaders headers,
      OAuth2TokenValidators validator)
    {
      try
      {
        return tokenValidator.ValidateToken(requestContext, headers.Authorization, validator, out JwtSecurityToken _, out bool _, out bool _) != null ? ArrStatus.Valid : ArrStatus.Invalid;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60116, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, ex);
        return ArrStatus.Invalid;
      }
    }

    private void TraceValidationResult(IVssRequestContext requestContext, ArrStatus result)
    {
      switch (result)
      {
        case ArrStatus.Valid:
          requestContext.Trace(60113, TraceLevel.Info, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, "Originator IP of forwarded request passed validation and can be trusted");
          break;
        case ArrStatus.Invalid:
          requestContext.TraceAlways(60112, TraceLevel.Warning, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, "Forwarding for spoofed IP detected!");
          break;
        case ArrStatus.Empty:
          requestContext.Trace(60114, TraceLevel.Verbose, ArrForwardedIpValidator.s_Area, ArrForwardedIpValidator.s_Layer, "Usual traffic, both headers are missing");
          break;
      }
    }
  }
}
