// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.EventWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web.Services;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03", Description = "Azure DevOps Server Events web service")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "Eventing", CollectionServiceIdentifier = "CCB8C273-DACB-4f09-B9AA-8EC0E42BADA2", ConfigurationServiceIdentifier = "C424AE04-8C6F-4516-8B2D-238FFFCA3081")]
  public class EventWebService : FrameworkWebService
  {
    private INotificationSubscriptionService m_notificationService;

    public EventWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.RequestContext.CheckOnPremisesDeployment(true);
      this.m_notificationService = this.RequestContext.GetService<INotificationSubscriptionService>();
    }

    [WebMethod]
    public int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string projectName = null)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SubscribeEvent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (userId), (object) userId);
        methodInformation.AddParameter(nameof (eventType), (object) eventType);
        methodInformation.AddParameter(nameof (filterExpression), (object) filterExpression);
        methodInformation.AddParameter(nameof (preferences), (object) preferences);
        this.EnterMethod(methodInformation);
        Guid identifierFromUserId = this.GetUserIdentifierFromUserId(this.RequestContext, userId);
        SubscriptionProjectParser subscriptionProjectParser = new SubscriptionProjectParser();
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription()
        {
          SubscriberId = identifierFromUserId,
          SubscriptionFilter = (ISubscriptionFilter) new ExpressionFilter(eventType),
          ConditionString = filterExpression ?? string.Empty,
          DeliveryAddress = preferences.Address,
          Channel = DeliveryTypeChannelMapper.GetChannelName(preferences.Type),
          Matcher = "PathMatcher"
        };
        if (string.IsNullOrEmpty(projectName))
        {
          string newSubscriptionCondition = subscription.ConditionString;
          subscription.ProjectId = subscriptionProjectParser.GetProjectId(this.RequestContext, eventType, filterExpression, Guid.Empty, false, out newSubscriptionCondition);
          subscription.ConditionString = newSubscriptionCondition;
        }
        else
          subscription.ProjectId = subscriptionProjectParser.GetProjectGuidFromName(this.RequestContext, projectName);
        return this.m_notificationService.CreateSubscription(this.RequestContext, subscription);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public int SubscribeEventWithClassification(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification,
      string projectName = null)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SubscribeEventWithClassification), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (userId), (object) userId);
        methodInformation.AddParameter(nameof (eventType), (object) eventType);
        methodInformation.AddParameter(nameof (filterExpression), (object) filterExpression);
        methodInformation.AddParameter(nameof (preferences), (object) preferences);
        methodInformation.AddParameter(nameof (classification), (object) classification);
        this.EnterMethod(methodInformation);
        Guid identifierFromUserId = this.GetUserIdentifierFromUserId(this.RequestContext, userId);
        SubscriptionProjectParser subscriptionProjectParser = new SubscriptionProjectParser();
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription()
        {
          SubscriberId = identifierFromUserId,
          SubscriptionFilter = (ISubscriptionFilter) new ExpressionFilter(eventType),
          ConditionString = filterExpression ?? string.Empty,
          DeliveryAddress = preferences.Address,
          Channel = DeliveryTypeChannelMapper.GetChannelName(preferences.Type),
          Tag = classification,
          Matcher = "PathMatcher"
        };
        if (string.IsNullOrEmpty(projectName))
        {
          string newSubscriptionCondition = subscription.ConditionString;
          subscription.ProjectId = subscriptionProjectParser.GetProjectId(this.RequestContext, eventType, filterExpression, Guid.Empty, false, out newSubscriptionCondition);
          subscription.ConditionString = newSubscriptionCondition;
        }
        else
          subscription.ProjectId = subscriptionProjectParser.GetProjectGuidFromName(this.RequestContext, projectName);
        return this.m_notificationService.CreateSubscription(this.RequestContext, subscription);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UnsubscribeEvent(int subscriptionId, string projectName = null)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UnsubscribeEvent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (subscriptionId), (object) subscriptionId);
        this.EnterMethod(methodInformation);
        SubscriptionProjectParser subscriptionProjectParser = new SubscriptionProjectParser();
        this.m_notificationService.DeleteSubscription(this.RequestContext, subscriptionId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<EventWebService.SerializableSubscription> EventSubscriptions(
      string userId,
      string projectName = null)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (EventSubscriptions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (userId), (object) userId);
        this.EnterMethod(methodInformation);
        Guid identifierFromUserId = this.GetUserIdentifierFromUserId(this.RequestContext, userId);
        SubscriptionProjectParser subscriptionProjectParser = new SubscriptionProjectParser();
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(subscriberId: new Guid?(identifierFromUserId));
        if (!string.IsNullOrEmpty(projectName))
          anyFieldLookup.DataspaceId = new Guid?(subscriptionProjectParser.GetProjectGuidFromName(this.RequestContext, projectName));
        return this.m_notificationService.QuerySubscriptions(this.RequestContext, anyFieldLookup).ConvertAll<EventWebService.SerializableSubscription>((Converter<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, EventWebService.SerializableSubscription>) (subscription => this.ToSerializableSubscription(subscription)));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<EventWebService.SerializableSubscription> EventSubscriptionsByClassification(
      string userId,
      string classification,
      string projectName = null)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (EventSubscriptionsByClassification), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (userId), (object) userId);
        methodInformation.AddParameter(nameof (classification), (object) classification);
        this.EnterMethod(methodInformation);
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(subscriberId: new Guid?(this.GetUserIdentifierFromUserId(this.RequestContext, userId)), classification: classification);
        SubscriptionProjectParser subscriptionProjectParser = new SubscriptionProjectParser();
        if (!string.IsNullOrEmpty(projectName))
          anyFieldLookup.DataspaceId = new Guid?(subscriptionProjectParser.GetProjectGuidFromName(this.RequestContext, projectName));
        return this.m_notificationService.QuerySubscriptions(this.RequestContext, anyFieldLookup).ConvertAll<EventWebService.SerializableSubscription>((Converter<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, EventWebService.SerializableSubscription>) (subscription => this.ToSerializableSubscription(subscription)));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private EventWebService.SerializableSubscription ToSerializableSubscription(
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription)
    {
      EventWebService.SerializableSubscription serializableSubscription = new EventWebService.SerializableSubscription();
      serializableSubscription.ConditionString = subscription.ConditionString;
      serializableSubscription.DeliveryPreference = new DeliveryPreference()
      {
        Address = subscription.DeliveryAddress,
        Type = DeliveryTypeChannelMapper.GetDeliveryType(subscription.Channel),
        Schedule = DeliverySchedule.Immediate
      };
      serializableSubscription.EventType = subscription.SubscriptionFilter.EventType;
      serializableSubscription.ID = subscription.ID;
      serializableSubscription.Subscriber = subscription.SubscriberIdentity?.Descriptor?.Identifier;
      serializableSubscription.Tag = subscription.Tag;
      serializableSubscription.ProjectId = subscription.ProjectId;
      serializableSubscription.Warning = subscription.Warning;
      return serializableSubscription;
    }

    [WebMethod]
    public void FireAsyncEvent(bool guaranteed, string theEvent)
    {
      try
      {
        INotificationEventService service = this.RequestContext.GetService<INotificationEventService>();
        MethodInformation methodInformation = new MethodInformation(nameof (FireAsyncEvent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (guaranteed), (object) guaranteed);
        methodInformation.AddParameter(nameof (theEvent), (object) theEvent);
        this.EnterMethod(methodInformation);
        IVssRequestContext requestContext = this.RequestContext;
        VssNotificationEvent notificationEvent = EventWebService.ToVssNotificationEvent(this.RequestContext, theEvent);
        service.PublishEvent(requestContext, notificationEvent);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void FireBulkAsyncEvents(bool guaranteed, [ClientType(typeof (IEnumerable<string>))] string[] theEvents)
    {
      try
      {
        INotificationEventService service = this.RequestContext.GetService<INotificationEventService>();
        MethodInformation methodInformation = new MethodInformation(nameof (FireBulkAsyncEvents), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (guaranteed), (object) guaranteed);
        methodInformation.AddArrayParameter<string>(nameof (theEvents), (IList<string>) theEvents);
        this.EnterMethod(methodInformation);
        IVssRequestContext requestContext = this.RequestContext;
        IEnumerable<VssNotificationEvent> notificationEvents = EventWebService.ToVssNotificationEvents(this.RequestContext, (IEnumerable<string>) theEvents);
        service.PublishEvents(requestContext, notificationEvents);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    private Guid GetUserIdentifierFromUserId(IVssRequestContext requestContext, string userId)
    {
      if (string.IsNullOrEmpty(userId) || userId.Equals("*", StringComparison.Ordinal))
        return Guid.Empty;
      TeamFoundationIdentityService service = this.RequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity foundationIdentity = (TeamFoundationIdentity) null;
      if (userId.StartsWith("S-", true, CultureInfo.InvariantCulture))
      {
        try
        {
          IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(userId);
          foundationIdentity = service.ReadIdentity(requestContext, descriptorFromSid, MembershipQuery.None, ReadIdentityOptions.None);
        }
        catch (Exception ex)
        {
        }
      }
      if (foundationIdentity == null)
      {
        try
        {
          IdentityDescriptor descriptor = new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", userId);
          foundationIdentity = service.ReadIdentity(requestContext, descriptor, MembershipQuery.None, ReadIdentityOptions.None);
        }
        catch (Exception ex)
        {
        }
      }
      if (foundationIdentity == null)
      {
        foundationIdentity = service.ReadIdentity(requestContext, userId);
        if (foundationIdentity == null)
          throw new IdentityNotFoundException(FrameworkResources.IdentityNotFoundSimpleMessage());
      }
      return foundationIdentity.TeamFoundationId;
    }

    private static VssNotificationEvent ToVssNotificationEvent(
      IVssRequestContext requestContext,
      string theEvent)
    {
      try
      {
        string localName = XDocument.Parse(theEvent).Root.Name.LocalName;
        return new VssNotificationEvent(theEvent, localName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1002110, TraceLevel.Warning, nameof (EventWebService), "Microsoft.TeamFoundation.Server.Core.WebServices", "Failed to retrieve event type: {0}", (object) ex.Message);
      }
      return (VssNotificationEvent) null;
    }

    private static IEnumerable<VssNotificationEvent> ToVssNotificationEvents(
      IVssRequestContext requestContext,
      IEnumerable<string> theEvents)
    {
      List<VssNotificationEvent> notificationEvents = new List<VssNotificationEvent>();
      foreach (string theEvent in theEvents)
      {
        VssNotificationEvent notificationEvent = EventWebService.ToVssNotificationEvent(requestContext, theEvent);
        if (notificationEvent != null)
          notificationEvents.Add(notificationEvent);
      }
      return (IEnumerable<VssNotificationEvent>) notificationEvents;
    }

    [XmlRoot("Subscription")]
    public class SerializableSubscription : Microsoft.VisualStudio.Services.Notifications.Server.Subscription
    {
      public string Subscriber { get; set; }

      public string EventType { get; set; }

      public DeliveryPreference DeliveryPreference { get; set; }
    }
  }
}
