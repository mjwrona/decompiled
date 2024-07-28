// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationEventFactory
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationEventFactory
  {
    internal static TeamFoundationEvent FromVssNotificationEvent(VssNotificationEvent notifEvent)
    {
      TeamFoundationEvent teamFoundationEvent = TeamFoundationEventFactory.GetEvent(notifEvent.EventType, notifEvent.EventDataString());
      if (notifEvent.HasActors)
        teamFoundationEvent.Actors.AddRange((IEnumerable<EventActor>) notifEvent.Actors);
      if (notifEvent.HasArtifactUris)
        teamFoundationEvent.ArtifactUri = notifEvent.ArtifactUris.FirstOrDefault<string>();
      if (notifEvent.HasScopes)
        teamFoundationEvent.Scopes.AddRange((IEnumerable<EventScope>) notifEvent.Scopes);
      teamFoundationEvent.ItemId = notifEvent.ItemId;
      return teamFoundationEvent;
    }

    public static TeamFoundationEvent GetEvent(VssNotificationEvent notifEvent) => TeamFoundationEventFactory.GetEvent(notifEvent.EventType, notifEvent.EventDataString());

    public static TeamFoundationEvent GetEvent(
      string eventType,
      string eventData,
      IFieldContainer fieldContainer = null)
    {
      if (string.IsNullOrWhiteSpace(eventType))
        return fieldContainer == null ? (TeamFoundationEvent) new TeamFoundationXmlEvent(eventType, eventData) : (TeamFoundationEvent) new TeamFoundationXmlEvent(eventType, fieldContainer);
      TeamFoundationEvent teamFoundationEvent = (TeamFoundationEvent) null;
      SerializedNotificationEvent evt = (SerializedNotificationEvent) null;
      for (int index = 0; index < 2; ++index)
      {
        if ((!string.IsNullOrWhiteSpace(eventData) ? (int) TeamFoundationEventFactory.GetFirstNonWhitespaceChar(eventData) : 9786) == 60)
          teamFoundationEvent = fieldContainer != null ? (TeamFoundationEvent) new TeamFoundationXmlEvent(eventType, fieldContainer) : (TeamFoundationEvent) new TeamFoundationXmlEvent(eventType, eventData);
        else if (fieldContainer == null)
        {
          JObject jObject = JObject.Parse(eventData);
          if (string.Equals(evt == null ? NotificationsSerialization.GetNotifObjectType(jObject) : string.Empty, typeof (SerializedNotificationEvent).FullName))
          {
            evt = jObject.ToObject<SerializedNotificationEvent>();
            eventData = evt.EventDataString();
          }
          else
            teamFoundationEvent = (TeamFoundationEvent) new TeamFoundationJsonEvent(eventType, (IFieldContainer) new JsonDocumentFieldContainer(jObject));
        }
        else
          teamFoundationEvent = (TeamFoundationEvent) new TeamFoundationJsonEvent(eventType, fieldContainer);
        if (teamFoundationEvent != null)
          break;
      }
      if (teamFoundationEvent == null)
        teamFoundationEvent = (TeamFoundationEvent) new TeamFoundationJsonEvent(eventType, string.Empty);
      teamFoundationEvent.AllowedChannels = evt?.AllowedChannels;
      return teamFoundationEvent;
    }

    private static char GetFirstNonWhitespaceChar(string str)
    {
      int index = 0;
      while (char.IsWhiteSpace(str[index]) && index < str.Length - 1)
        ++index;
      return str[index];
    }
  }
}
