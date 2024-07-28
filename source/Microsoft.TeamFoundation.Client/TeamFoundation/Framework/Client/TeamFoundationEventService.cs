// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationEventService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class TeamFoundationEventService : IEventService
  {
    private EventWebService m_eventWebService;

    public TeamFoundationEventService(TfsConnection tfs) => this.m_eventWebService = new EventWebService(tfs);

    public int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences)
    {
      return this.m_eventWebService.SubscribeEvent(userId, eventType, filterExpression, preferences);
    }

    public int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification,
      string projectName = null)
    {
      return this.m_eventWebService.SubscribeEventWithClassification(userId, eventType, filterExpression, preferences, classification, projectName);
    }

    public int SubscribeEvent(
      string eventType,
      string filterExpression,
      DeliveryPreference preferences)
    {
      return this.m_eventWebService.SubscribeEvent(this.m_eventWebService.Connection.AuthorizedIdentity.Descriptor.Identifier, eventType, filterExpression, preferences);
    }

    public int SubscribeEvent(
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification)
    {
      return this.m_eventWebService.SubscribeEventWithClassification(this.m_eventWebService.Connection.AuthorizedIdentity.Descriptor.Identifier, eventType, filterExpression, preferences, classification);
    }

    public void UnsubscribeEvent(int subscriptionId, string projectName = null) => this.m_eventWebService.UnsubscribeEvent(subscriptionId, projectName);

    public Subscription[] GetAllEventSubscriptions() => this.m_eventWebService.EventSubscriptions("*");

    public Subscription[] GetAllEventSubscriptions(string classification, string projectName = null) => this.m_eventWebService.EventSubscriptionsByClassification("*", classification, projectName);

    public Subscription[] GetEventSubscriptions(string user) => this.m_eventWebService.EventSubscriptions(user);

    public Subscription[] GetEventSubscriptions(
      string user,
      string classification,
      string projectName = null)
    {
      return this.m_eventWebService.EventSubscriptionsByClassification(user, classification, projectName);
    }

    public Subscription[] GetEventSubscriptions(IdentityDescriptor user) => this.m_eventWebService.EventSubscriptions(user.Identifier);

    public Subscription[] GetEventSubscriptions(
      IdentityDescriptor user,
      string classification,
      string projectName = null)
    {
      return this.m_eventWebService.EventSubscriptionsByClassification(user.Identifier, classification, projectName);
    }

    public void FireEvent(object theEvent)
    {
      ArgumentUtility.CheckForNull<object>(theEvent, nameof (theEvent));
      this.m_eventWebService.FireAsyncEvent(false, TeamFoundationEventService.SerializeEvent(theEvent));
    }

    public void FireEvents(IEnumerable<object> theEvents)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) theEvents, nameof (theEvents));
      int num = 0;
      List<string> theEvents1 = new List<string>();
      foreach (object theEvent in theEvents)
      {
        ArgumentUtility.CheckForNull<object>(theEvent, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "theEvents[{0}]", (object) num));
        ++num;
        theEvents1.Add(TeamFoundationEventService.SerializeEvent(theEvent));
      }
      this.m_eventWebService.FireBulkAsyncEvents(false, (IEnumerable<string>) theEvents1);
    }

    private static string SerializeEvent(object eventObject)
    {
      if (eventObject == null)
        return string.Empty;
      if (eventObject.GetType() == typeof (string))
        return eventObject as string;
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings))
        {
          new XmlSerializer(eventObject.GetType()).Serialize(xmlWriter, eventObject);
          return output.ToString();
        }
      }
    }

    [Obsolete]
    public Subscription[] EventSubscriptions(string userId) => this.GetEventSubscriptions(userId);

    [Obsolete]
    public Subscription[] EventSubscriptions(string userId, string tag) => this.GetEventSubscriptions(userId, tag, (string) null);

    [Obsolete]
    public void FireAsyncEvent(string theEvent) => this.FireEvent((object) theEvent);

    [Obsolete]
    public void FireAsyncEvent(object theEvent) => this.FireEvent(theEvent);

    [Obsolete]
    public void FireBulkAsyncEvents(string[] theEvents) => this.FireEvents((IEnumerable<object>) theEvents);

    [Obsolete]
    public void FireBulkAsyncEvents(object[] theEvents) => this.FireEvents((IEnumerable<object>) theEvents);
  }
}
