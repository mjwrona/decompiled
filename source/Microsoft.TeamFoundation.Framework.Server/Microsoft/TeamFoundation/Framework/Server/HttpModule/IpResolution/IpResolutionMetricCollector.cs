// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.IpResolutionMetricCollector
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal class IpResolutionMetricCollector : IIpResolutionMetricCollector
  {
    private static readonly string[] _dimensionNames = new string[4]
    {
      "IpSource",
      "ArrStatus",
      "AfdStatus",
      "SmartRouterStatus"
    };

    public void CollectMetric(
      IVssRequestContext requestContext,
      ArrStatus arrDimension,
      AfdStatus afdDimension,
      SmartRouterIpStatus smartRouterDimension)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableArrForwardingIpValidation"))
        return;
      try
      {
        string str = this.InferIpSourceDimension(arrDimension, afdDimension, smartRouterDimension);
        MdmService.PublishMetricRaw("IpSourceStatistics", 1L, IpResolutionMetricCollector._dimensionNames, new string[4]
        {
          str,
          EnumUtility.ToString<ArrStatus>(arrDimension),
          EnumUtility.ToString<AfdStatus>(afdDimension),
          EnumUtility.ToString<SmartRouterIpStatus>(smartRouterDimension)
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60117, "HttpModule", nameof (IpResolutionMetricCollector), ex);
      }
    }

    private string InferIpSourceDimension(
      ArrStatus arrDimension,
      AfdStatus afdDimension,
      SmartRouterIpStatus smartRouterDimension)
    {
      if (arrDimension == ArrStatus.Valid)
        return "X-Arr-Forwarded-For";
      if (afdDimension == AfdStatus.KnownAfdIp)
        return "X-FD-SocketIP";
      return smartRouterDimension == SmartRouterIpStatus.Valid ? "X-SmartRouter-Forwarded-For" : "IpStack";
    }
  }
}
