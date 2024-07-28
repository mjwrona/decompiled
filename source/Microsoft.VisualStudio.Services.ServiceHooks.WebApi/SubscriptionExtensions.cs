// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class SubscriptionExtensions
  {
    public static void Validate(this Subscription subscription, bool isBeingCreated)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(subscription.EventType, "subscription.EventType");
      if (!isBeingCreated)
        ArgumentUtility.CheckForEmptyGuid(subscription.Id, "subscription.Id");
      else
        ArgumentUtility.CheckForOutOfRange<byte>(subscription.ProbationRetries, "subscription.ProbationRetries", (byte) 0, (byte) 0);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(subscription.ConsumerId, "subscription.ConsumerId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(subscription.PublisherId, "subscription.PublisherId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(subscription.ConsumerActionId, "subscription.ConsumerActionId");
    }
  }
}
