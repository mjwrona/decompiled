// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps.AzChatOpsActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.Azure.DevOps.AzChatOps.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WorkplaceMessagingApps
{
  public class AzChatOpsActionTask : ActionTask
  {
    private readonly JObject m_webHookEventJObject;
    private readonly string m_postToUrl;
    private const string SlackApiArea = "slack";
    private const string TeamsApiArea = "teams";

    public AzChatOpsActionTask(JObject webHookEventJObject, string postToUrl)
    {
      this.m_webHookEventJObject = webHookEventJObject;
      this.m_postToUrl = postToUrl;
    }

    public override async Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      AzChatOpsActionTask chatOpsActionTask = this;
      Stopwatch sw = new Stopwatch();
      try
      {
        chatOpsActionTask.UpdateNotificationForRequest(requestContext, string.Format(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_RequestTemplate, (object) chatOpsActionTask.m_postToUrl.ToString(), (object) chatOpsActionTask.m_webHookEventJObject.ToString()));
        sw.Start();
        try
        {
          Uri azChatOpsBaseUri = chatOpsActionTask.GetAzChatOpsBaseUri();
          (string azChatopsResourceArea, string azChatopsApiMethod) = chatOpsActionTask.GetAzChatOpsApiAreaAndMethod();
          await chatOpsActionTask.CreateAzChatOpsEventAsync(requestContext, azChatOpsBaseUri, azChatopsResourceArea, azChatopsApiMethod);
        }
        finally
        {
          sw.Stop();
        }
        chatOpsActionTask.UpdateNotificationForResponse(requestContext, ServiceHooksWebApiResources.Response_OK(), new double?(sw.Elapsed.TotalSeconds));
        return new ActionTaskResult(ActionTaskResultLevel.Success);
      }
      catch (Exception ex)
      {
        chatOpsActionTask.UpdateNotificationForResponse(requestContext, ServiceHooksWebApiResources.Response_Error(), new double?(sw.Elapsed.TotalSeconds), ex.Message, ex.ToString());
        return new ActionTaskResult(ActionTaskResultLevel.EnduringFailure, ex, ex.Message);
      }
    }

    internal async Task CreateAzChatOpsEventAsync(
      IVssRequestContext requestContext,
      Uri chatopsBaseUri,
      string azChatopsResourceArea,
      string azChatopsApiMethod)
    {
      if (chatopsBaseUri == (Uri) null)
        throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidUrl);
      if (string.IsNullOrWhiteSpace(azChatopsResourceArea))
        throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidApiArea);
      if (string.IsNullOrWhiteSpace(azChatopsApiMethod))
        throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidApiMethod);
      ICreateClient clientProvider = (ICreateClient) requestContext.ClientProvider;
      switch (azChatopsResourceArea)
      {
        case "slack":
          AzChatOpsSlackHttpClient client1 = clientProvider.CreateClient<AzChatOpsSlackHttpClient>(requestContext, chatopsBaseUri, "ServiceHooksToAzChatOpsSlack", (ApiResourceLocationCollection) null);
          if (azChatopsApiMethod.Equals("NotificationData", StringComparison.OrdinalIgnoreCase))
          {
            await client1.CreateEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
            break;
          }
          if (azChatopsApiMethod.Equals("BoardsNotificationData", StringComparison.OrdinalIgnoreCase))
          {
            await client1.CreateBoardsEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
            break;
          }
          if (!azChatopsApiMethod.Equals("ReposNotificationData", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidApiMethod);
          await client1.CreateReposEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
          break;
        case "teams":
          AzChatOpsTeamsHttpClient client2 = clientProvider.CreateClient<AzChatOpsTeamsHttpClient>(requestContext, chatopsBaseUri, "ServiceHooksToAzChatOpsTeams", (ApiResourceLocationCollection) null);
          if (azChatopsApiMethod.Equals("AzPipelinesNotificationData", StringComparison.OrdinalIgnoreCase))
          {
            await client2.CreateEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
            break;
          }
          if (azChatopsApiMethod.Equals("AzBoardsNotificationData", StringComparison.OrdinalIgnoreCase))
          {
            await client2.CreateBoardsEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
            break;
          }
          if (!azChatopsApiMethod.Equals("AzReposNotificationData", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidApiMethod);
          await client2.CreateReposEventAsync(this.m_webHookEventJObject, (object) null, new CancellationToken());
          break;
        default:
          throw new ArgumentException(WorkplaceMessagingAppsConsumerResources.AzChatOpsActionTask_InvalidApiArea);
      }
    }

    internal Uri GetAzChatOpsBaseUri()
    {
      int length = this.m_postToUrl.IndexOf("_apis/");
      return length >= 0 ? new Uri(this.m_postToUrl.Substring(0, length)) : (Uri) null;
    }

    internal (string, string) GetAzChatOpsApiAreaAndMethod()
    {
      string str = "_apis/";
      int num = this.m_postToUrl.IndexOf(str);
      if (num >= 0)
      {
        string[] strArray = this.m_postToUrl.Substring(num + str.Length).Split('/', '?');
        if (strArray != null && strArray.Length > 1)
          return (strArray[0], strArray[1]);
      }
      return ((string) null, (string) null);
    }
  }
}
