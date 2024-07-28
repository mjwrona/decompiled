// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusClientHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class ServiceBusClientHelper
  {
    private static readonly string c_ruleName = "$Default";
    private static readonly string c_sbConnectionStringTemplate = "Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName={1};SharedAccessKey={2}";

    internal static async Task<bool> UpdateFilterAsync(
      IVssRequestContext requestContext,
      string messageBusName,
      string namespaceName,
      string topicName,
      string subscriptionName,
      string newFilter,
      ITFLogger logger)
    {
      ServiceBusClient client = ServiceBusClientHelper.CreateServiceBusClient(requestContext, messageBusName, namespaceName);
      object obj1 = (object) null;
      int num1 = 0;
      ServiceBusRuleManager ruleManager;
      SqlRuleFilter filter;
      bool flag1;
      try
      {
        ruleManager = client.CreateRuleManager(topicName, subscriptionName);
        filter = new SqlRuleFilter(newFilter);
        IAsyncEnumerator<RuleProperties> asyncEnumerator = ruleManager.GetRulesAsync(new CancellationToken()).GetAsyncEnumerator();
        object obj2 = (object) null;
        int num2 = 0;
        bool flag2;
        try
        {
          do
          {
            if (!await asyncEnumerator.MoveNextAsync())
              goto label_8;
          }
          while (!((RuleFilter) filter).Equals(asyncEnumerator.Current.Filter));
          logger.Info("UpdateFilterAsync: Filter already matches existing filter for subscription [" + topicName + "|" + subscriptionName + "], doing nothing");
          flag2 = true;
          num2 = 1;
        }
        catch (object ex)
        {
          obj2 = ex;
        }
label_8:
        if (asyncEnumerator != null)
          await asyncEnumerator.DisposeAsync();
        object obj = obj2;
        if (obj != null)
        {
          if (!(obj is Exception source))
            throw obj;
          ExceptionDispatchInfo.Capture(source).Throw();
        }
        if (num2 == 1)
        {
          flag1 = flag2;
        }
        else
        {
          obj2 = (object) null;
          asyncEnumerator = (IAsyncEnumerator<RuleProperties>) null;
          try
          {
            await ruleManager.DeleteRuleAsync(ServiceBusClientHelper.c_ruleName, new CancellationToken());
          }
          catch (ServiceBusException ex) when (((Exception) ex).Message.Contains("could not be found"))
          {
            logger.Warning("UpdateFilterAsync: Failed to Delete rule " + ServiceBusClientHelper.c_ruleName + " for subscription: " + subscriptionName + ", TopicName: " + topicName + ", because it was not found.");
          }
          catch (Exception ex)
          {
            logger.Error("UpdateFilterAsync: Failed to Delete rule " + ServiceBusClientHelper.c_ruleName + " for subscription: " + subscriptionName + ", TopicName: " + topicName);
            logger.Error(ex);
            flag1 = false;
            goto label_27;
          }
          try
          {
            await ruleManager.CreateRuleAsync(ServiceBusClientHelper.c_ruleName, (RuleFilter) filter, new CancellationToken());
          }
          catch (Exception ex)
          {
            logger.Error("UpdateFilterAsync: Failed to Create rule " + ServiceBusClientHelper.c_ruleName + " with value " + newFilter + " for subscription: " + subscriptionName + ", TopicName: " + topicName);
            logger.Error(ex);
            flag1 = false;
            goto label_27;
          }
          flag1 = true;
        }
label_27:
        num1 = 1;
      }
      catch (object ex)
      {
        obj1 = ex;
      }
      if (client != null)
        await ((IAsyncDisposable) client).DisposeAsync();
      object obj3 = obj1;
      if (obj3 != null)
      {
        if (!(obj3 is Exception source))
          throw obj3;
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num1 == 1)
        return flag1;
      obj1 = (object) null;
      client = (ServiceBusClient) null;
      ruleManager = (ServiceBusRuleManager) null;
      filter = (SqlRuleFilter) null;
      bool flag;
      return flag;
    }

    private static ServiceBusClient CreateServiceBusClient(
      IVssRequestContext requestContext,
      string serviceBusName,
      string nameSpace)
    {
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/MessageBus/ServiceBus/...");
      if (string.IsNullOrEmpty(nameSpace))
        nameSpace = ServiceBusSettingsHelper.GetMessageBusNamespace(registryEntries, serviceBusName);
      if (string.IsNullOrEmpty(nameSpace))
        throw new MessageBusConfigurationException(HostingResources.ServiceBusNamespaceNotRegistered((object) nameSpace));
      return new ServiceBusClient(ServiceBusClientHelper.CreateServiceBusConnectionString(requestContext, nameSpace, registryEntries));
    }

    private static string CreateServiceBusConnectionString(
      IVssRequestContext requestContext,
      string nameSpace,
      RegistryEntryCollection registryEntries)
    {
      string managementSetting = ServiceBusSettingsHelper.GetNamespaceScopedManagementSetting<string>(registryEntries, nameSpace, "/SharedAccessKeySettingName", string.Empty);
      MessageBusCredentials messageBusCredentials = ServiceBusSettingsHelper.GetMessageBusCredentials(requestContext, managementSetting);
      return string.Format(ServiceBusClientHelper.c_sbConnectionStringTemplate, (object) nameSpace, (object) messageBusCredentials.SharedAccessKeyName, (object) messageBusCredentials.SharedAccessKeyValue);
    }
  }
}
