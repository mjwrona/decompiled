// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusNotificationHubSendActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public sealed class ServiceBusNotificationHubSendActionTask : ServiceBusActionTaskBase
  {
    private NotificationHubClient m_notificationHubClient;

    public ServiceBusNotificationHubSendActionTask(
      string serviceBusConnectionString,
      string notificationHubName,
      string tagsExpression,
      IDictionary<string, string> templateProperties,
      string messageContent)
      : base(serviceBusConnectionString, messageContent)
    {
      this.NotificationHubName = notificationHubName;
      this.TagsExpression = tagsExpression;
      this.TemplateProperties = templateProperties;
    }

    public string NotificationHubName { get; private set; }

    public string TagsExpression { get; private set; }

    public IDictionary<string, string> TemplateProperties { get; private set; }

    protected override void InitializeAction(TimeSpan timeout) => this.m_notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(this.ServiceBusConnectionString, this.NotificationHubName);

    protected override async Task<string> PerformActionAsync(TimeSpan timeout)
    {
      string str;
      try
      {
        NotificationOutcome notificationOutcome = await this.m_notificationHubClient.SendTemplateNotificationAsync(this.TemplateProperties, this.TagsExpression);
        str = string.Format(AzureConsumerResources.ServiceBusNotificationHubSendAction_Response_Ok_Template, (object) notificationOutcome.TrackingId, (object) notificationOutcome.State);
      }
      catch (TimeoutException ex)
      {
        throw new TimeoutException(AzureConsumerResources.ServiceBus_Response_Error_Timeout);
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions != null)
        {
          Exception exception = ex.InnerExceptions.FirstOrDefault<Exception>();
          if (exception != null)
            throw exception;
          throw;
        }
        else
          throw;
      }
      return str;
    }
  }
}
