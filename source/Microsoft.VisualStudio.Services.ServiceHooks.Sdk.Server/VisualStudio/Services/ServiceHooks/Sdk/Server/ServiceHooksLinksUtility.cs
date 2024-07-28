// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksLinksUtility
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class ServiceHooksLinksUtility
  {
    public static ReferenceLinks GetBaseReferenceLinks(
      IVssRequestContext requestContext,
      string selfLink)
    {
      ReferenceLinks baseReferenceLinks = new ReferenceLinks();
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.SelfLink, selfLink);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetConsumerReferenceLinks(
      IVssRequestContext requestContext,
      Consumer consumer,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = ServiceHooksLinksUtility.GetBaseReferenceLinks(requestContext, consumer.Url);
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.ActionsLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.ConsumerActionsLocationId, (object) new
      {
        ConsumerId = consumer.Id
      }));
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetConsumerActionReferenceLinks(
      IVssRequestContext requestContext,
      ConsumerAction consumerAction,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = ServiceHooksLinksUtility.GetBaseReferenceLinks(requestContext, consumerAction.Url);
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.ConsumerLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.ConsumersLocationId, (object) new
      {
        ConsumerId = consumerAction.ConsumerId
      }));
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetSubscriptionReferenceLinks(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = ServiceHooksLinksUtility.GetBaseReferenceLinks(requestContext, subscription.Url);
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.ConsumerLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.ConsumersLocationId, (object) new
      {
        ConsumerId = subscription.ConsumerId
      }));
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.ActionsLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.ConsumerActionsLocationId, (object) new
      {
        ConsumerId = subscription.ConsumerId
      }));
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.NotificationsLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.NotificationsLocationId, (object) new
      {
        subscriptionId = subscription.Id
      }));
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.PublisherLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.PublishersLocationId, (object) new
      {
        publisherId = subscription.PublisherId
      }));
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetPublisherReferenceLinks(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher publisher,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = ServiceHooksLinksUtility.GetBaseReferenceLinks(requestContext, publisher.Url);
      baseReferenceLinks.AddLink(ServiceHooksApiConstants.EventTypesLink, urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.PublisherEventTypesLocationId, (object) new
      {
        publisherId = publisher.Id
      }));
      return baseReferenceLinks;
    }
  }
}
