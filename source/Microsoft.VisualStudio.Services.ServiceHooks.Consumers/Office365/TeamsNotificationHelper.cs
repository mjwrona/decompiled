// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365.TeamsNotificationHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Office365
{
  public static class TeamsNotificationHelper
  {
    public static void ProcessNotificationData(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      if (!(eventArgs.Notification.Details is NotificationDetailsInternal details))
        return;
      if (details.NotificationData == null)
        details.NotificationData = (IDictionary<string, string>) new Dictionary<string, string>();
      JObject jobject = JObject.FromObject((object) raisedEvent, JsonSerializer.Create(CommonConsumerSettings.JsonSerializerSettings))["resource"] as JObject;
      switch (raisedEvent.EventType)
      {
        case "build.complete":
          TeamsNotificationHelper.ProcessBuildNotificationData(details.NotificationData, jobject["result"]?.ToString());
          break;
        case "git.pullrequest.created":
        case "git.pullrequest.merged":
        case "git.pullrequest.updated":
          TeamsNotificationHelper.ProcessPullRequestNotificationData(details.NotificationData);
          break;
        case "ms.vss-release.deployment-completed-event":
          TeamsNotificationHelper.ProcessDeploymentCompletedNotificationData(details.NotificationData, jobject["environment"][(object) "status"]?.ToString());
          break;
        case "ms.vss-release.deployment-approval-completed-event":
          TeamsNotificationHelper.ProcessDeploymentApprovalCompletedNotificationData(details.NotificationData, jobject["approval"][(object) "status"]?.ToString());
          break;
      }
    }

    internal static void ProcessDeploymentApprovalCompletedNotificationData(
      IDictionary<string, string> inputNotificationData,
      string statusValue)
    {
      string str1 = "#f8a800";
      string str2 = "teams-review-waiting.png";
      switch (statusValue)
      {
        case "approved":
          str1 = "#107c10";
          str2 = "teams-review-approved.png";
          break;
        case "rejected":
          str1 = "#da0a00";
          str2 = "teams-review-rejected.png";
          break;
      }
      inputNotificationData["color"] = str1;
      inputNotificationData["iconFileName"] = str2;
    }

    internal static void ProcessDeploymentCompletedNotificationData(
      IDictionary<string, string> inputNotificationData,
      string statusValue)
    {
      string str1 = "teams-deployment.png";
      string str2 = "#dadada";
      switch (statusValue)
      {
        case "succeeded":
          str1 = "#107c10";
          str2 = "teams-deployment-succeeded.png";
          break;
        case "partiallySucceeded":
          str1 = "#f8a800";
          str2 = "teams-deployment-partially-succeeded.png";
          break;
        case "inProgress":
          str1 = "#0078D4";
          str2 = "teams-deployment-in-progress.png";
          break;
        case "rejected":
          str1 = "#da0a00";
          str2 = "teams-deployment-failed.png";
          break;
        case "canceled":
          str1 = "#dadada";
          str2 = "teams-deployment-cancelled.png";
          break;
      }
      inputNotificationData["color"] = str1;
      inputNotificationData["iconFileName"] = str2;
    }

    internal static void ProcessPullRequestNotificationData(
      IDictionary<string, string> inputNotificationData)
    {
      string str1 = (string) null;
      string str2 = "teams-pull-request.png";
      string str3 = (string) null;
      string str4;
      if (inputNotificationData.TryGetValue("status", out str4))
      {
        switch (str4)
        {
          case "Vote Reset":
            str1 = "#dadada";
            str2 = "teams-pull-requet-reset-vote.png";
            str3 = "Vote Reset";
            break;
          case "Approved":
            str1 = "#107c10";
            str2 = "teams-pull-request-approved.png";
            str3 = "Approved";
            break;
          case "Approved with suggestions":
            str1 = "#107c10";
            str2 = "teams-pull-request-approved.png";
            str3 = "Approved with suggestions";
            break;
          case "Not ready":
            str1 = "#f8a800";
            str2 = "teams-pull-request-waiting.png";
            str3 = "Waiting for Author";
            break;
          case "Rejected":
            str1 = "#da0a00";
            str2 = "teams-pull-request-rejected.png";
            str3 = "Rejected";
            break;
        }
      }
      inputNotificationData["color"] = str1;
      inputNotificationData["iconFileName"] = str2;
      inputNotificationData["text"] = str3;
    }

    internal static void ProcessBuildNotificationData(
      IDictionary<string, string> inputNotificationData,
      string statusValue)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      switch (statusValue)
      {
        case "succeeded":
          str1 = "#107c10";
          str2 = "teams-build-succeeded.png";
          str3 = "Succeeded";
          break;
        case "partiallySucceeded":
          str1 = "#f8a800";
          str2 = "teams-build-partially-succeeded.png";
          str3 = "Partially Succeeded";
          break;
        case "canceled":
          str1 = "#dadada";
          str2 = "teams-build-cancelled.png";
          str3 = "Cancelled";
          break;
        case "failed":
          str1 = "#da0a00";
          str2 = "teams-build-failed.png";
          str3 = "Failed";
          break;
      }
      inputNotificationData["color"] = str1;
      inputNotificationData["iconFileName"] = str2;
      inputNotificationData["text"] = str3;
    }
  }
}
