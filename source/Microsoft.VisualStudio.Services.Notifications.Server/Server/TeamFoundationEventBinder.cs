// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationEventBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class TeamFoundationEventBinder : ObjectBinder<TeamFoundationEvent>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder EventTypeNameColumn = new SqlColumnBinder("EventTypeName");
    private SqlColumnBinder EventColumn = new SqlColumnBinder("Event");
    private SqlColumnBinder CreatedTimeColumn = new SqlColumnBinder("CreatedTime");
    private SqlColumnBinder ProcessQueueColumn = new SqlColumnBinder("ProcessQueue");
    private SqlColumnBinder InitiatorColumn = new SqlColumnBinder("Initiator");
    private SqlColumnBinder ProcessedTimeColumn = new SqlColumnBinder("ProcessedTime");
    private SqlColumnBinder MatchesColumn = new SqlColumnBinder("Matches");
    private SqlColumnBinder NextProcessTimeColumn = new SqlColumnBinder("NextProcessTime");
    private SqlColumnBinder ExpirationTimeColumn = new SqlColumnBinder("ExpirationTime");
    private SqlColumnBinder SourceEventCreatedTimeColumn = new SqlColumnBinder("SourceEventCreatedTime");

    protected override TeamFoundationEvent Bind()
    {
      string str1 = this.EventTypeNameColumn.GetString((IDataReader) this.Reader, true);
      string str2 = this.EventColumn.GetString((IDataReader) this.Reader, true);
      TeamFoundationEvent teamFoundationEvent;
      try
      {
        ArgumentUtility.CheckForNull<string>(str1, "EventTypeName");
        ArgumentUtility.CheckForNull<string>(str2, "Event");
        JObject jObject = JObject.Parse(str2);
        string notifObjectType = NotificationsSerialization.GetNotifObjectType(jObject);
        if (string.IsNullOrEmpty(notifObjectType))
          throw new InvalidEventSerializerAttributeException(notifObjectType ?? string.Empty);
        SerializedNotificationEvent notificationEvent = jObject.ToObject<SerializedNotificationEvent>();
        teamFoundationEvent = TeamFoundationEventFactory.GetEvent(str1, notificationEvent.Data as string);
        teamFoundationEvent.AllowedChannels = notificationEvent.AllowedChannels;
        if (notificationEvent.HasActors)
          teamFoundationEvent.Actors.AddRange((IEnumerable<EventActor>) notificationEvent.Actors);
        if (notificationEvent.HasScopes)
          teamFoundationEvent.Scopes.AddRange((IEnumerable<EventScope>) notificationEvent.Scopes);
        if (notificationEvent.HasArtifactUris)
          teamFoundationEvent.ArtifactUri = notificationEvent.ArtifactUris.First<string>();
      }
      catch (Exception ex)
      {
        teamFoundationEvent = (TeamFoundationEvent) new TeamFoundationJsonEvent("invalid-" + str1 + "-err-" + ex.GetType().Name, "{ }");
      }
      teamFoundationEvent.Id = this.IdColumn.GetInt32((IDataReader) this.Reader);
      teamFoundationEvent.ProcessQueue = this.ProcessQueueColumn.GetString((IDataReader) this.Reader, string.Empty) ?? string.Empty;
      teamFoundationEvent.CreatedTime = this.CreatedTimeColumn.GetDateTime((IDataReader) this.Reader);
      teamFoundationEvent.Initiator = this.InitiatorColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      teamFoundationEvent.ProcessedTime = this.ProcessedTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      teamFoundationEvent.Matches = this.MatchesColumn.GetInt32((IDataReader) this.Reader, -1, -1);
      teamFoundationEvent.NextProcessTime = this.NextProcessTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      teamFoundationEvent.ExpirationTime = this.ExpirationTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      teamFoundationEvent.SourceEventCreatedTime = this.SourceEventCreatedTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      return teamFoundationEvent;
    }
  }
}
