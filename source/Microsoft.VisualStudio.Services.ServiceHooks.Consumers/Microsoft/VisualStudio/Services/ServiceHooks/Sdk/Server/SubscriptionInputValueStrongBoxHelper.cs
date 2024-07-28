// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.SubscriptionInputValueStrongBoxHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class SubscriptionInputValueStrongBoxHelper
  {
    private const string c_strongBoxDrawerNamePrefix = "Microsoft.VisualStudio.Services.ServiceHooks";

    public static List<InputDescriptor> GetConfidentialInputDescriptors(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId)
    {
      ConsumerImplementation consumer = SubscriptionInputValueStrongBoxHelper.GetConsumerService(requestContext).GetConsumer(requestContext, consumerId);
      ConsumerActionImplementation actionImplementation = consumer.Actions.FirstOrDefault<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (a => a.Id == consumerActionId));
      if (actionImplementation == null)
        return new List<InputDescriptor>(0);
      List<InputDescriptor> inputDescriptors = new List<InputDescriptor>();
      inputDescriptors.AddRange(consumer.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (i => i.IsConfidential)));
      inputDescriptors.AddRange(actionImplementation.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (i => i.IsConfidential)));
      return inputDescriptors;
    }

    public static void StripConfidentialInputs(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      out List<KeyValuePair<string, string>> confidentialPublisherInputs,
      out List<KeyValuePair<string, string>> confidentialConsumerInputs)
    {
      Dictionary<string, InputDescriptor> dictionary = SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, subscription.ConsumerId, subscription.ConsumerActionId).ToDictionary<InputDescriptor, string>((Func<InputDescriptor, string>) (i => i.Id));
      List<KeyValuePair<string, string>> nonConfidentialConsumerInputs;
      List<KeyValuePair<string, string>> nonConfidentialPublisherInputs;
      subscription.SplitInputsByScopeAndConfidentiality(dictionary, out confidentialConsumerInputs, out confidentialPublisherInputs, out nonConfidentialConsumerInputs, out nonConfidentialPublisherInputs);
      subscription.ConsumerInputs = (IDictionary<string, string>) nonConfidentialConsumerInputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (i => i.Key), (Func<KeyValuePair<string, string>, string>) (i => i.Value));
      subscription.PublisherInputs = (IDictionary<string, string>) nonConfidentialPublisherInputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (i => i.Key), (Func<KeyValuePair<string, string>, string>) (i => i.Value));
    }

    public static void ApplyConfidentialInputs(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      List<KeyValuePair<string, string>> confidentialPublisherInputs,
      List<KeyValuePair<string, string>> confidentialConsumerInputs,
      bool maskValues = true)
    {
      foreach (KeyValuePair<string, string> confidentialConsumerInput in confidentialConsumerInputs)
        subscription.ConsumerInputs.Add(new KeyValuePair<string, string>(confidentialConsumerInput.Key, maskValues ? SecurityHelper.GetMaskedValue(confidentialConsumerInput.Value) : confidentialConsumerInput.Value));
      foreach (KeyValuePair<string, string> confidentialPublisherInput in confidentialPublisherInputs)
        subscription.PublisherInputs.Add(new KeyValuePair<string, string>(confidentialPublisherInput.Key, maskValues ? SecurityHelper.GetMaskedValue(confidentialPublisherInput.Value) : confidentialPublisherInput.Value));
    }

    public static void StoreConfidentialInputs(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      List<KeyValuePair<string, string>> confidentialPublisherInputs,
      List<KeyValuePair<string, string>> confidentialConsumerInputs)
    {
      foreach (KeyValuePair<string, string> confidentialPublisherInput in confidentialPublisherInputs)
        SubscriptionInputValueStrongBoxHelper.WriteConfidentialInputValue(requestContext, subscription.Id, SubscriptionInputScope.Publisher, confidentialPublisherInput);
      foreach (KeyValuePair<string, string> confidentialConsumerInput in confidentialConsumerInputs)
        SubscriptionInputValueStrongBoxHelper.WriteConfidentialInputValue(requestContext, subscription.Id, SubscriptionInputScope.Consumer, confidentialConsumerInput);
    }

    public static void AddConfidentialInputValuesToSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool maskValues)
    {
      List<InputDescriptor> inputDescriptors = SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, subscription.ConsumerId, subscription.ConsumerActionId);
      if (inputDescriptors.Count == 0)
        return;
      foreach (SubscriptionInputValue subscriptionInputValue in SubscriptionInputValueStrongBoxHelper.GetItems(requestContext, subscription.Id, maskValues))
      {
        SubscriptionInputValue confidentialInputValue = subscriptionInputValue;
        IDictionary<string, string> dictionary;
        if (maskValues)
        {
          dictionary = !inputDescriptors.Any<InputDescriptor>((Func<InputDescriptor, bool>) (inputDesc => inputDesc.Id.Equals(confidentialInputValue.InputId))) ? subscription.PublisherInputs : subscription.ConsumerInputs;
        }
        else
        {
          switch (confidentialInputValue.Scope)
          {
            case SubscriptionInputScope.Publisher:
              dictionary = subscription.PublisherInputs;
              break;
            case SubscriptionInputScope.Consumer:
              dictionary = subscription.ConsumerInputs;
              break;
            default:
              throw new NotImplementedException(confidentialInputValue.Scope.ToString());
          }
        }
        if (dictionary.ContainsKey(confidentialInputValue.InputId))
          dictionary.Remove(confidentialInputValue.InputId);
        dictionary.Add(new KeyValuePair<string, string>(confidentialInputValue.InputId, confidentialInputValue.InputValue));
      }
    }

    public static void RestoreConfidentialInputValues(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId,
      Guid subscriptionId,
      IDictionary<string, string> inputDict)
    {
      List<SubscriptionInputValue> source = (List<SubscriptionInputValue>) null;
      List<InputDescriptor> inputDescriptors = SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, consumerId, consumerActionId);
      if (inputDescriptors == null || inputDescriptors.Count == 0)
        return;
      foreach (string str in new List<string>(inputDict.Keys.Where<string>((Func<string, bool>) (key => SecurityHelper.IsMasked(inputDict[key])))))
      {
        string inputId = str;
        if (inputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (id => id.Id == inputId)) != null)
        {
          if (source == null)
          {
            source = SubscriptionInputValueStrongBoxHelper.GetItems(requestContext, subscriptionId);
            if (source == null || source.Count == 0)
              break;
          }
          SubscriptionInputValue subscriptionInputValue = source.FirstOrDefault<SubscriptionInputValue>((Func<SubscriptionInputValue, bool>) (civ => civ.InputId == inputId));
          if (subscriptionInputValue != null)
            inputDict[inputId] = subscriptionInputValue.InputValue;
        }
      }
    }

    public static void RestoreConfidentialInputValuesToNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      if (SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, notification.Details.ConsumerId, notification.Details.ConsumerActionId).Count == 0)
        return;
      List<SubscriptionInputValue> items = SubscriptionInputValueStrongBoxHelper.GetItems(requestContext, notification.SubscriptionId);
      if (notification is TestNotification testNotification && testNotification.OverriddenConfidentialInputs != null)
      {
        foreach (Tuple<string, string, SubscriptionInputScope> confidentialInput in testNotification.OverriddenConfidentialInputs)
        {
          Tuple<string, string, SubscriptionInputScope> overriddenInput = confidentialInput;
          do
            ;
          while (items.Remove(items.FirstOrDefault<SubscriptionInputValue>((Func<SubscriptionInputValue, bool>) (i => i.InputId == overriddenInput.Item1))));
          items.Add(new SubscriptionInputValue()
          {
            InputId = overriddenInput.Item1,
            InputValue = overriddenInput.Item2,
            Scope = overriddenInput.Item3,
            SubscriptionId = testNotification.SubscriptionId
          });
        }
      }
      if (items.Count == 0)
        return;
      foreach (SubscriptionInputValue subscriptionInputValue in items)
      {
        IDictionary<string, string> dictionary;
        switch (subscriptionInputValue.Scope)
        {
          case SubscriptionInputScope.Publisher:
            dictionary = notification.Details.PublisherInputs;
            break;
          case SubscriptionInputScope.Consumer:
            dictionary = notification.Details.ConsumerInputs;
            break;
          default:
            throw new NotImplementedException(subscriptionInputValue.Scope.ToString());
        }
        if (dictionary.ContainsKey(subscriptionInputValue.InputId))
        {
          if (SecurityHelper.IsMasked(dictionary[subscriptionInputValue.InputId]))
          {
            dictionary.Remove(subscriptionInputValue.InputId);
            dictionary.Add(subscriptionInputValue.InputId, subscriptionInputValue.InputValue);
          }
        }
        else
          dictionary.Add(subscriptionInputValue.InputId, subscriptionInputValue.InputValue);
      }
    }

    public static void UpdateUnmaskedInputsInStrongBox(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      List<KeyValuePair<string, string>> confidentialPublisherInputs,
      List<KeyValuePair<string, string>> confidentialConsumerInputs,
      bool maskValues = true)
    {
      foreach (KeyValuePair<string, string> confidentialPublisherInput in confidentialPublisherInputs)
      {
        if (!SecurityHelper.IsMasked(confidentialPublisherInput.Value))
        {
          SubscriptionInputValueStrongBoxHelper.WriteConfidentialInputValue(requestContext, subscription.Id, SubscriptionInputScope.Publisher, confidentialPublisherInput);
          subscription.PublisherInputs.Add(confidentialPublisherInput.Key, SecurityHelper.GetMaskedValue(confidentialPublisherInput.Value));
        }
        else
          subscription.PublisherInputs.Add(confidentialPublisherInput.Key, confidentialPublisherInput.Value);
      }
      foreach (KeyValuePair<string, string> confidentialConsumerInput in confidentialConsumerInputs)
      {
        if (!SecurityHelper.IsMasked(confidentialConsumerInput.Value))
        {
          SubscriptionInputValueStrongBoxHelper.WriteConfidentialInputValue(requestContext, subscription.Id, SubscriptionInputScope.Consumer, confidentialConsumerInput);
          subscription.ConsumerInputs.Add(confidentialConsumerInput.Key, maskValues ? SecurityHelper.GetMaskedValue(confidentialConsumerInput.Value) : confidentialConsumerInput.Value);
        }
        else
          subscription.ConsumerInputs.Add(confidentialConsumerInput.Key, confidentialConsumerInput.Value);
      }
    }

    public static void RemoveOrphanedInputValuesFromStrongBox(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      List<KeyValuePair<string, string>> confidentialPublisherInputs,
      List<KeyValuePair<string, string>> confidentialConsumerInputs)
    {
      HashSet<string> itemKeysToDelete = SubscriptionInputValueStrongBoxHelper.GetItemKeysForDrawer(requestContext, subscription.Id);
      confidentialConsumerInputs.ForEach((Action<KeyValuePair<string, string>>) (input => itemKeysToDelete.Remove(input.Key)));
      confidentialPublisherInputs.ForEach((Action<KeyValuePair<string, string>>) (input => itemKeysToDelete.Remove(input.Key)));
      foreach (string lookupKey in itemKeysToDelete)
        SubscriptionInputValueStrongBoxHelper.DeleteItem(requestContext, subscription.Id, lookupKey);
    }

    public static void DeleteDrawer(IVssRequestContext requestContext, Guid identifier)
    {
      ITeamFoundationStrongBoxService strongBoxService = SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext);
      Guid drawerId = SubscriptionInputValueStrongBoxHelper.GetDrawerId(requestContext, identifier);
      if (!(drawerId != Guid.Empty))
        return;
      strongBoxService.DeleteDrawer(requestContext, drawerId);
    }

    public static List<SubscriptionInputValue> GetItems(
      IVssRequestContext requestContext,
      Guid identifier,
      bool maskValues = false)
    {
      List<SubscriptionInputValue> items = new List<SubscriptionInputValue>();
      ITeamFoundationStrongBoxService strongBoxService = SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext);
      Guid drawerId = SubscriptionInputValueStrongBoxHelper.GetDrawerId(requestContext, identifier);
      if (drawerId != Guid.Empty)
      {
        foreach (StrongBoxItemInfo drawerContent in strongBoxService.GetDrawerContents(requestContext, drawerId))
        {
          if (maskValues)
          {
            SubscriptionInputValue subscriptionInputValue = new SubscriptionInputValue()
            {
              InputId = drawerContent.LookupKey,
              InputValue = SecurityHelper.GetMaskedValue(drawerContent.LookupKey)
            };
            items.Add(subscriptionInputValue);
          }
          else
          {
            SubscriptionInputValue subscriptionInputValue = JsonConvert.DeserializeObject<SubscriptionInputValue>(strongBoxService.GetString(requestContext, drawerId, drawerContent.LookupKey));
            items.Add(subscriptionInputValue);
          }
        }
      }
      return items;
    }

    private static ServiceHooksConsumerService GetConsumerService(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ServiceHooksConsumerService>();

    private static void WriteConfidentialInputValue(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      SubscriptionInputScope scope,
      KeyValuePair<string, string> input)
    {
      SubscriptionInputValueStrongBoxHelper.AddItem(requestContext, subscriptionId, input.Key, new SubscriptionInputValue()
      {
        InputId = input.Key,
        InputValue = input.Value,
        Scope = scope,
        SubscriptionId = subscriptionId
      });
    }

    private static void AddItem(
      IVssRequestContext requestContext,
      Guid identifier,
      string lookupKey,
      SubscriptionInputValue item)
    {
      ITeamFoundationStrongBoxService strongBoxService = SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext);
      Guid drawerId = SubscriptionInputValueStrongBoxHelper.GetDrawerId(requestContext, identifier);
      if (drawerId == Guid.Empty)
        drawerId = strongBoxService.CreateDrawer(requestContext, SubscriptionInputValueStrongBoxHelper.CreateDrawerName(requestContext, identifier));
      strongBoxService.AddString(requestContext, drawerId, lookupKey, JsonConvert.SerializeObject((object) item, Formatting.None));
    }

    private static void DeleteItem(
      IVssRequestContext requestContext,
      Guid identifier,
      string lookupKey)
    {
      ITeamFoundationStrongBoxService strongBoxService = SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext);
      Guid drawerId = SubscriptionInputValueStrongBoxHelper.GetDrawerId(requestContext, identifier);
      if (!(drawerId != Guid.Empty))
        return;
      strongBoxService.DeleteItem(requestContext, drawerId, lookupKey);
    }

    private static HashSet<string> GetItemKeysForDrawer(
      IVssRequestContext requestContext,
      Guid identifier)
    {
      HashSet<string> keys = new HashSet<string>();
      ITeamFoundationStrongBoxService strongBoxService = SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext);
      Guid drawerId = SubscriptionInputValueStrongBoxHelper.GetDrawerId(requestContext, identifier);
      if (drawerId != Guid.Empty)
        strongBoxService.GetDrawerContents(requestContext, drawerId).ForEach((Action<StrongBoxItemInfo>) (info => keys.Add(info.LookupKey)));
      return keys;
    }

    private static string CreateDrawerName(IVssRequestContext requestContext, Guid identifier) => string.Format("{0}_{1}_{2}", (object) "Microsoft.VisualStudio.Services.ServiceHooks", (object) requestContext.ServiceHost.InstanceId, (object) identifier);

    private static Guid GetDrawerId(IVssRequestContext requestContext, Guid identifier) => SubscriptionInputValueStrongBoxHelper.GetStrongBoxService(ref requestContext).UnlockDrawer(requestContext, SubscriptionInputValueStrongBoxHelper.CreateDrawerName(requestContext, identifier), false);

    private static ITeamFoundationStrongBoxService GetStrongBoxService(
      ref IVssRequestContext requestContext)
    {
      requestContext = requestContext.Elevate();
      return requestContext.GetService<ITeamFoundationStrongBoxService>();
    }
  }
}
