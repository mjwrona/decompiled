// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution.SmartRouterForwardedIpValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution
{
  internal class SmartRouterForwardedIpValidator : ISmartRouterForwardedIpValidator
  {
    private const int HostBitsV4 = 16;
    private const int HostBitsV6 = 64;

    public SmartRouterIpStatus Validate(
      IVssRequestContext requestContext,
      string? forwardedFor,
      string? userHostAddress,
      LocalHostAddressCallback? getLocalHostAddress = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      SmartRouterIpStatus validationStatus = this.ValidateIpAndSource(requestContext, forwardedFor, userHostAddress, getLocalHostAddress ?? SmartRouterForwardedIpValidator.\u003C\u003EO.\u003C0\u003E__GetLocalHostIpAddress ?? (SmartRouterForwardedIpValidator.\u003C\u003EO.\u003C0\u003E__GetLocalHostIpAddress = new LocalHostAddressCallback(IpAddressUtility.GetLocalHostIpAddress)));
      this.TraceValidationResult(requestContext, validationStatus);
      return validationStatus;
    }

    private SmartRouterIpStatus ValidateIpAndSource(
      IVssRequestContext requestContext,
      string? forwardedForHeader,
      string? requestUserHostAddress,
      LocalHostAddressCallback localHostAddressCallback)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.ForwardingIpValidation.Enabled"))
        return SmartRouterIpStatus.ValidationDisabled;
      if (forwardedForHeader == null)
        return SmartRouterIpStatus.Empty;
      IPAddress ipAddress = localHostAddressCallback(requestContext).Item2;
      if (ipAddress == null)
        return SmartRouterIpStatus.InvalidLocalHost;
      if (!IPAddress.TryParse(forwardedForHeader, out IPAddress _))
        return SmartRouterIpStatus.Invalid;
      IPAddress address;
      if (!IPAddress.TryParse(requestUserHostAddress, out address) || address.AddressFamily != ipAddress.AddressFamily)
        return SmartRouterIpStatus.InvalidUserHost;
      int rangeBits;
      if (address.AddressFamily == AddressFamily.InterNetwork)
      {
        rangeBits = 16;
      }
      else
      {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
          return SmartRouterIpStatus.InvalidUserHost;
        rangeBits = 64;
      }
      return !IPAddressRange.IsAddressInRange(ipAddress.GetAddressBytes(), rangeBits, address.GetAddressBytes()) ? SmartRouterIpStatus.Invalid : SmartRouterIpStatus.Valid;
    }

    private void TraceValidationResult(
      IVssRequestContext requestContext,
      SmartRouterIpStatus validationStatus)
    {
      switch (validationStatus)
      {
        case SmartRouterIpStatus.Empty:
          requestContext.Trace(34005503, TraceLevel.Verbose, "SmartRouter", "Server", "Usual traffic, both headers are missing");
          break;
        case SmartRouterIpStatus.Valid:
          requestContext.Trace(34005502, TraceLevel.Info, "SmartRouter", "Server", "Originator IP of forwarded request passed validation and can be trusted");
          break;
        case SmartRouterIpStatus.Invalid:
          requestContext.TraceAlways(34005501, TraceLevel.Warning, "SmartRouter", "Server", "Forwarding for spoofed IP detected!");
          break;
        case SmartRouterIpStatus.InvalidUserHost:
          requestContext.Trace(34005505, TraceLevel.Verbose, "SmartRouter", "Server", "The user host address could not be determined");
          break;
        case SmartRouterIpStatus.InvalidLocalHost:
          requestContext.Trace(34005504, TraceLevel.Verbose, "SmartRouter", "Server", "The local host address could not be determined");
          break;
      }
    }
  }
}
