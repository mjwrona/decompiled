// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WiqlTextHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class WiqlTextHelper
  {
    public static void ValidateWiqlTextRequirements(IVssRequestContext requestContext, string wiql)
    {
      IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
      int maxWiqlTextLength = WiqlTextHelper.IsMsProjectUserAgent(requestContext) ? configurationInfo.MaxWiqlTextLengthForMSProject : configurationInfo.MaxWiqlTextLength;
      if (wiql.Length > maxWiqlTextLength)
      {
        WorkItemTrackingQueryMaxWiqlTextLengthLimitExceededException exceededException = new WorkItemTrackingQueryMaxWiqlTextLengthLimitExceededException(maxWiqlTextLength);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WorkItemService", QueryTelemetry.Feature, new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "ExceptionType",
            (object) exceededException.GetType().Name
          },
          {
            "ExceptionMessage",
            (object) exceededException.Message
          },
          {
            "WiqlLength",
            (object) wiql.Length
          }
        }));
        throw exceededException;
      }
    }

    private static bool IsMsProjectUserAgent(IVssRequestContext requestContext)
    {
      string userAgent = requestContext.RootContext.UserAgent;
      return !string.IsNullOrWhiteSpace(userAgent) && userAgent.IndexOf("WINPROJ.EXE", StringComparison.OrdinalIgnoreCase) > 0;
    }
  }
}
