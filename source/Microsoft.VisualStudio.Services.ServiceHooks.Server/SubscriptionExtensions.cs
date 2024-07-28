// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.SubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  public static class SubscriptionExtensions
  {
    public static string GetConsumerInput(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      string inputId,
      bool required = false)
    {
      string consumerInput = (string) null;
      if (((!subscription.ConsumerInputs.TryGetValue(inputId, out consumerInput) ? 1 : (string.IsNullOrEmpty(consumerInput) ? 1 : 0)) & (required ? 1 : 0)) != 0)
        throw new ArgumentException(inputId);
      return consumerInput;
    }

    public static void SetSubscriptionUrl(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions)
        subscription.SetSubscriptionUrl(urlHelper, requestContext);
    }

    public static void SetSubscriptionUrl(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      Guid subscriptionsLocationId = ServiceHooksApiConstants.SubscriptionsLocationId;
      subscription.Url = urlHelper.RestLink(requestContext, subscriptionsLocationId, (object) new
      {
        subscriptionId = subscription.Id
      });
    }

    public static void SplitInputsByScopeAndConfidentiality(
      this Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      Dictionary<string, InputDescriptor> confidentialInputDescsById,
      out List<KeyValuePair<string, string>> confidentialConsumerInputs,
      out List<KeyValuePair<string, string>> confidentialPublisherInputs,
      out List<KeyValuePair<string, string>> nonConfidentialConsumerInputs,
      out List<KeyValuePair<string, string>> nonConfidentialPublisherInputs)
    {
      confidentialConsumerInputs = new List<KeyValuePair<string, string>>();
      confidentialPublisherInputs = new List<KeyValuePair<string, string>>();
      nonConfidentialConsumerInputs = new List<KeyValuePair<string, string>>();
      nonConfidentialPublisherInputs = new List<KeyValuePair<string, string>>();
      foreach (KeyValuePair<string, string> consumerInput in (IEnumerable<KeyValuePair<string, string>>) subscription.ConsumerInputs)
      {
        if (confidentialInputDescsById.ContainsKey(consumerInput.Key))
          confidentialConsumerInputs.Add(consumerInput);
        else
          nonConfidentialConsumerInputs.Add(consumerInput);
      }
      foreach (KeyValuePair<string, string> publisherInput in (IEnumerable<KeyValuePair<string, string>>) subscription.PublisherInputs)
      {
        if (confidentialInputDescsById.ContainsKey(publisherInput.Key))
          confidentialPublisherInputs.Add(publisherInput);
        else
          nonConfidentialPublisherInputs.Add(publisherInput);
      }
    }
  }
}
