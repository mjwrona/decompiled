// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.SubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class SubscriptionExtensions
  {
    public static void SplitInputsByScopeAndConfidentiality(
      this Subscription subscription,
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
