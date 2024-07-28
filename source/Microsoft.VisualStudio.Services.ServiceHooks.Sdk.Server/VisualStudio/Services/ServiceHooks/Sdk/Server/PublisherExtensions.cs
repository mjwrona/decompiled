// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.PublisherExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class PublisherExtensions
  {
    public static IList<Publisher> ToPublisherModels(
      this IEnumerable<ServiceHooksPublisher> publishers,
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs = null)
    {
      return (IList<Publisher>) publishers.Select<ServiceHooksPublisher, Publisher>((Func<ServiceHooksPublisher, Publisher>) (publisher => publisher.ToPublisherModel(requestContext, publisherInputs))).ToList<Publisher>();
    }

    public static void SetPublisherUrl(
      this IEnumerable<Publisher> publishers,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (Publisher publisher in publishers)
        publisher.SetPublisherUrl(urlHelper, requestContext);
    }

    public static void SetPublisherUrl(
      this Publisher publisher,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      if (urlHelper == null || !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && publisher.ServiceInstanceType != null && !publisher.ServiceInstanceType.Equals(requestContext.ServiceInstanceType().ToString(), StringComparison.OrdinalIgnoreCase))
        return;
      publisher.Url = urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.PublishersLocationId, (object) new
      {
        publisherId = publisher.Id
      });
      publisher.Links = ServiceHooksLinksUtility.GetPublisherReferenceLinks(requestContext, publisher, urlHelper);
      if (publisher.SupportedEvents == null)
        return;
      publisher.SupportedEvents.SetEventTypeUrl(urlHelper, requestContext);
    }

    public static void SetEventTypeUrl(
      this IEnumerable<EventTypeDescriptor> eventTypes,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      foreach (EventTypeDescriptor eventType in eventTypes)
        eventType.SetEventTypeUrl(urlHelper, requestContext);
    }

    public static void SetEventTypeUrl(
      this EventTypeDescriptor eventType,
      UrlHelper urlHelper,
      IVssRequestContext requestContext)
    {
      eventType.Url = urlHelper.RestLink(requestContext, ServiceHooksPublisherApiConstants.PublisherEventTypesLocationId, (object) new
      {
        publisherId = eventType.PublisherId,
        eventTypeId = eventType.Id
      });
    }
  }
}
