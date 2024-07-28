// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraEventsHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class JiraEventsHandler : IPipelineEventsHandler
  {
    private const string c_layer = "JiraEventsHandler";

    public bool IsValidEvent(
      IVssRequestContext requestContext,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Entering IsValidEvent for {0}.", (object) "jiraconnectapp");
      try
      {
        JiraLifecycleEventData eventData = JsonUtilities.Deserialize<JiraLifecycleEventData>(jsonPayload);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(eventData.EventType, "EventType");
        if (!JiraEventsHandler.isValidEventType(eventData))
          return false;
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Validating the event {0}.", (object) eventData.EventType);
        if (string.Equals(eventData.EventType, "installed", StringComparison.OrdinalIgnoreCase))
          return true;
        if ((string.Equals(eventData.EventType, "deleteproject", StringComparison.OrdinalIgnoreCase) || string.Equals(eventData.EventType, "deleteaccount", StringComparison.OrdinalIgnoreCase)) && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return false;
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Validate the token for event {0}.", (object) eventData.EventType);
        return JiraEventsHandler.ValidateRequest(requestContext, headers, eventData.BaseUrl, eventData.EventType);
      }
      catch
      {
      }
      return false;
    }

    public bool TryExtractDeliveryId(HttpRequest request, out Guid deliveryId)
    {
      deliveryId = Guid.Empty;
      return false;
    }

    public IEnumerable<IPipelineEventHandler> EventHandlers { get; } = (IEnumerable<IPipelineEventHandler>) new IPipelineEventHandler[1]
    {
      (IPipelineEventHandler) new JiraLifecycleEventHandler()
    };

    private static bool isValidEventType(JiraLifecycleEventData eventData) => string.Equals(eventData.EventType, "installed", StringComparison.OrdinalIgnoreCase) || string.Equals(eventData.EventType, "uninstalled", StringComparison.OrdinalIgnoreCase) || string.Equals(eventData.EventType, "deleteproject", StringComparison.OrdinalIgnoreCase) || string.Equals(eventData.EventType, "deleteaccount", StringComparison.OrdinalIgnoreCase);

    private static bool ValidateRequest(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string jiraBaseUrl,
      string eventType)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraBaseUrl);
      string str;
      if (!headers.TryGetValue("authorization", out str) || !str.StartsWith("JWT ", StringComparison.OrdinalIgnoreCase))
        return false;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Validating the token for jira account {0}.", (object) jiraAccountName);
      string token = str.Substring(4);
      if (string.Equals(eventType, "deleteproject", StringComparison.OrdinalIgnoreCase) || string.Equals(eventType, "deleteaccount", StringComparison.OrdinalIgnoreCase))
      {
        JiraConfigurationData configurationData = JiraHelper.GetJiraConfigurationData(requestContext, jiraAccountName);
        if (configurationData == null)
          return false;
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Validating the token using config data.");
        JiraInstallationData collectionConfiguration = JiraHelper.GetSecretsFromCollectionConfiguration(requestContext, configurationData.Id);
        if (collectionConfiguration != null)
        {
          JiraJsonWebTokenHelper.ValidateJsonWebToken(requestContext, token, collectionConfiguration.ClientKey, collectionConfiguration.SharedSecret);
          return true;
        }
        if (string.Equals(eventType, "deleteproject", StringComparison.OrdinalIgnoreCase))
          return false;
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Config data not available. Validate with key vault data.");
      }
      JiraInstallationData secretsFromKeyVault = JiraHelper.GetSecretsFromKeyVault(requestContext, jiraAccountName);
      if (secretsFromKeyVault == null)
        return false;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.ValidateIncomingEvent, nameof (JiraEventsHandler), "Validating the token using key vault data.");
      JiraJsonWebTokenHelper.ValidateJsonWebToken(requestContext, token, secretsFromKeyVault.ClientKey, secretsFromKeyVault.SharedSecret);
      return true;
    }

    internal static class Constants
    {
      public const string AppName = "jiraconnectapp";
    }
  }
}
