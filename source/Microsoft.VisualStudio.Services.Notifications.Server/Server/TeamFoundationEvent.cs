// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationEvent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class TeamFoundationEvent : ICloneable
  {
    protected IFieldContainer m_fieldContainer;
    private List<EventActor> m_actors;
    private List<EventScope> m_scopes;
    private EventStats m_eventStats;

    public TeamFoundationEvent()
    {
      this.m_actors = new List<EventActor>();
      this.m_scopes = new List<EventScope>();
      this.m_eventStats = new EventStats(this);
    }

    public int Id { get; set; }

    public string EventType { get; set; }

    public string EventData => this.m_fieldContainer.GetDocumentString();

    public List<EventActor> Actors => this.m_actors;

    public string ArtifactUri { get; set; }

    public Guid Initiator { get; set; }

    public List<EventScope> Scopes => this.m_scopes;

    public DateTime CreatedTime { get; set; }

    public DateTime ProcessedTime { get; set; }

    public DateTime NextProcessTime { get; set; } = DateTimeUtils.SqlDateMaxValue;

    public DateTime ExpirationTime { get; set; } = DateTimeUtils.SqlDateMinValue;

    public DateTime SourceEventCreatedTime { get; set; }

    public string ProcessQueue { get; set; }

    public string Status { get; set; }

    public int Matches { get; set; }

    public bool IsBlocked { get; set; }

    public string ItemId { get; set; }

    public EventStats EventStats => this.m_eventStats;

    public HashSet<string> AllowedChannels { get; set; }

    public object Clone()
    {
      TeamFoundationEvent teamFoundationEvent = TeamFoundationEventFactory.GetEvent(this.EventType, this.EventData, this.m_fieldContainer);
      teamFoundationEvent.Id = this.Id;
      teamFoundationEvent.Matches = this.Matches;
      teamFoundationEvent.ArtifactUri = this.ArtifactUri;
      teamFoundationEvent.m_eventStats = this.m_eventStats;
      teamFoundationEvent.Actors.AddRange((IEnumerable<EventActor>) this.Actors);
      HashSet<string> allowedChannels = this.AllowedChannels;
      teamFoundationEvent.AllowedChannels = allowedChannels != null ? allowedChannels.ToHashSet() : (HashSet<string>) null;
      teamFoundationEvent.NextProcessTime = this.NextProcessTime;
      teamFoundationEvent.CreatedTime = this.CreatedTime;
      teamFoundationEvent.ExpirationTime = this.ExpirationTime;
      teamFoundationEvent.Initiator = this.Initiator;
      teamFoundationEvent.Status = this.Status;
      teamFoundationEvent.ProcessQueue = this.ProcessQueue;
      teamFoundationEvent.ProcessedTime = this.ProcessedTime;
      teamFoundationEvent.SourceEventCreatedTime = this.SourceEventCreatedTime;
      teamFoundationEvent.ItemId = this.ItemId;
      return (object) teamFoundationEvent;
    }

    public abstract IFieldContainer GetFieldContainer();

    public virtual void UpdateFieldContainer(string eventData) => throw new NotImplementedException();

    public FieldResult GetFieldResult(string fieldPath) => FieldResult.GetFieldResult(this.GetFieldContainer().GetFieldValue(fieldPath));

    public void SetFieldContainer(IFieldContainer fieldContainer) => this.m_fieldContainer = fieldContainer;

    public void PrepareEvent()
    {
      if (!this.EventType.Equals("WorkItemChangedEvent") && !this.EventType.Equals("BuildStatusChangeEvent") && !this.EventType.Equals("CodeReviewChangedEvent") || string.IsNullOrEmpty(this.EventData))
        return;
      this.UpdateFieldContainer(TeamFoundationEvent.PrepareEvent(this.EventData));
    }

    public static string PrepareEvent(string eventData)
    {
      StringBuilder stringBuilder = new StringBuilder(eventData.Length);
      int startIndex1 = 0;
      char ch1 = '|';
      char ch2 = '%';
      do
      {
        int startIndex2 = eventData.IndexOf(ch1, startIndex1);
        int count = startIndex2 > -1 ? startIndex2 - startIndex1 : eventData.Length - startIndex1;
        stringBuilder.Append(eventData, startIndex1, count);
        if (startIndex2 >= 0 && startIndex2 + 1 < eventData.Length)
        {
          int num1 = eventData.IndexOf(ch2, startIndex2 + 1);
          int num2 = eventData.IndexOf(ch1, startIndex2 + 1);
          if (num1 < 0 || num2 < 0)
          {
            stringBuilder.Append(eventData, startIndex2, eventData.Length - startIndex2);
            break;
          }
          bool flag = num2 < num1;
          if (!flag)
          {
            int length = num2 - (num1 + 1);
            string input = eventData.Substring(num1 + 1, length);
            if (length == 36 && Guid.TryParseExact(input, "D", out Guid _))
              stringBuilder.Append(eventData, startIndex2 + 1, num1 - (startIndex2 + 1));
            else
              flag = true;
          }
          if (!flag)
          {
            startIndex1 = num2 + 1;
          }
          else
          {
            stringBuilder.Append(eventData, startIndex2, num2 - startIndex2);
            startIndex1 = num2;
          }
        }
        else
          break;
      }
      while (startIndex1 < eventData.Length - 1);
      return stringBuilder.ToString();
    }
  }
}
