// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.AfdIpValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal sealed class AfdIpValidator : IAfdIpValidator
  {
    private static readonly string s_Area = "HttpModule";
    private static readonly string s_Layer = nameof (AfdIpValidator);
    private readonly IConfigQueryable<List<IPv4Subnet>> _config;

    public AfdIpValidator(IConfigQueryable<List<IPv4Subnet>> config) => this._config = config;

    public AfdStatus Validate(IVssRequestContext requestContext, string ip)
    {
      requestContext.TraceEnter(60035, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableAfdIpValidation"))
      {
        requestContext.TraceLeave(60054, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
        return AfdStatus.ValidationDisabled;
      }
      if (string.IsNullOrWhiteSpace(ip))
      {
        requestContext.Trace(60036, TraceLevel.Warning, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, "Provided IP address is empty or whitespace");
        requestContext.TraceLeave(60037, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
        return AfdStatus.UnknownAfdIp;
      }
      try
      {
        int ipAddress = IPv4Subnet.ParseIpAddress(ip);
        foreach (IPv4Subnet ipv4Subnet in this._config.QueryByCtx<List<IPv4Subnet>>(requestContext))
        {
          if (ipv4Subnet.Contains(ipAddress))
          {
            requestContext.TraceLeave(60038, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
            return AfdStatus.KnownAfdIp;
          }
        }
        bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AfdIpValidationVerifyOnlyMode");
        requestContext.TraceAlways(60053, TraceLevel.Warning, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, string.Format("AFD impersonation detected! (verifyOnlyMode={0})", (object) flag));
        requestContext.TraceLeave(60039, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
        return flag ? AfdStatus.ValidationDisabled : AfdStatus.UnknownAfdIp;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60048, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, ex);
        requestContext.TraceLeave(60049, AfdIpValidator.s_Area, AfdIpValidator.s_Layer, nameof (Validate));
        return AfdStatus.UnknownAfdIp;
      }
    }
  }
}
