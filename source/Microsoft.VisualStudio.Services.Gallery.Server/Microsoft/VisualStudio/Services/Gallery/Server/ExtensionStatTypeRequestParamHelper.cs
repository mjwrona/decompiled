// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionStatTypeRequestParamHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class ExtensionStatTypeRequestParamHelper
  {
    public static DailyStatType GetStatTypeOrThrow(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string statTypeValue,
      string layerName)
    {
      DailyStatType statTypeOrThrow = DailyStatType.WebPageView;
      bool flag = false;
      if (!string.IsNullOrEmpty(statTypeValue))
      {
        if ("pageview".Equals(statTypeValue, StringComparison.OrdinalIgnoreCase))
        {
          statTypeOrThrow = DailyStatType.WebPageView;
          flag = true;
        }
        else if ("installcount".Equals(statTypeValue, StringComparison.OrdinalIgnoreCase))
        {
          if (GalleryUtil.IsVSTSOrTFSIntegrationTargets((IEnumerable<InstallationTarget>) DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName).InstallationTargets))
          {
            statTypeOrThrow = DailyStatType.Install;
            flag = true;
          }
        }
        else if ("download".Equals(statTypeValue, StringComparison.OrdinalIgnoreCase))
        {
          if (DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName).IsVsCodeExtension())
          {
            statTypeOrThrow = DailyStatType.WebDownload;
            flag = true;
          }
        }
        else if ("uninstall".Equals(statTypeValue, StringComparison.OrdinalIgnoreCase))
        {
          if (ExtensionStatTypeRequestParamHelper.IsIncrementUninstallStatisticValid(DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName), requestContext))
          {
            statTypeOrThrow = DailyStatType.Uninstall;
            flag = true;
          }
          else
          {
            string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Controller params, publisherName:{0}, extensionName:{1}, version:{2}, statType:{3}", (object) publisherName, (object) extensionName, (object) version, (object) statTypeOrThrow);
            requestContext.TraceAlways(12061109, TraceLevel.Warning, "gallery", layerName, format);
          }
        }
      }
      if (!flag)
        throw new InvalidOperationException(GalleryResources.ExtensionDailyStatsInvalidOperation((object) statTypeValue));
      return statTypeOrThrow;
    }

    private static bool IsIncrementUninstallStatisticValid(
      PublishedExtension extension,
      IVssRequestContext requestContext)
    {
      if (extension.IsVsCodeExtension() && GalleryServerUtil.IsVSCodeUserAgent(requestContext))
        return true;
      return extension.IsVsExtension() && GalleryServerUtil.IsVSIdeUserAgent(requestContext);
    }
  }
}
