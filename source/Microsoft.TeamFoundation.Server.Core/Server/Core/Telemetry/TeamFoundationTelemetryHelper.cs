// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Telemetry.TeamFoundationTelemetryHelper
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Telemetry;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Server.Core.Telemetry
{
  public static class TeamFoundationTelemetryHelper
  {
    public const string KeyUserAgent = "TFS.UserAgent";
    public const string KeyUserIdentityId = "TFS.UserIdentityId";

    public static void PublishAppInsightsTelemetry(
      this IVssRequestContext requestContext,
      string telemetryEventName,
      CustomerIntelligenceData ciData,
      IDictionary<string, string> telemetryPropertiesMap)
    {
      requestContext.PublishAppInsightsTelemetry(telemetryEventName, Guid.Empty, ciData, telemetryPropertiesMap);
    }

    public static void PublishAppInsightsTelemetry(
      this IVssRequestContext requestContext,
      string telemetryEventName,
      Guid userId,
      CustomerIntelligenceData ciData,
      IDictionary<string, string> telemetryPropertiesMap)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      TelemetryEvent telemetryEvent = new TelemetryEvent(telemetryEventName);
      telemetryEvent.Properties.Add("TFS.UserAgent", (object) (requestContext.UserAgent ?? string.Empty));
      telemetryEvent.Properties.Add("TFS.UserIdentityId", (object) TeamFoundationTelemetryHelper.GetUserIdSaltedHash(requestContext, userId));
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) ciData.GetData())
      {
        if (telemetryPropertiesMap.ContainsKey(keyValuePair.Key))
          telemetryEvent.Properties.Add(telemetryPropertiesMap[keyValuePair.Key], keyValuePair.Value ?? (object) string.Empty);
      }
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTelemetryService>().PostEvent(telemetryEvent);
    }

    private static string GetUserIdSaltedHash(IVssRequestContext requestContext, Guid userId)
    {
      using (HMAC hmac = (HMAC) new HMACSHA256(requestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToByteArray()))
        return Convert.ToBase64String(hmac.ComputeHash((userId != Guid.Empty ? userId : requestContext.GetUserId()).ToByteArray()));
    }
  }
}
