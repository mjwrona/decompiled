// Decompiled with JetBrains decompiler
// Type: Nest.PostCalendarEventsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PostCalendarEventsDescriptor : 
    RequestDescriptorBase<PostCalendarEventsDescriptor, PostCalendarEventsRequestParameters, IPostCalendarEventsRequest>,
    IPostCalendarEventsRequest,
    IRequest<PostCalendarEventsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPostCalendarEvents;

    public PostCalendarEventsDescriptor(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [SerializationConstructor]
    protected PostCalendarEventsDescriptor()
    {
    }

    Id IPostCalendarEventsRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    IEnumerable<ScheduledEvent> IPostCalendarEventsRequest.Events { get; set; }

    public PostCalendarEventsDescriptor Events(IEnumerable<ScheduledEvent> events) => this.Assign<IEnumerable<ScheduledEvent>>(events, (Action<IPostCalendarEventsRequest, IEnumerable<ScheduledEvent>>) ((a, v) => a.Events = v));

    public PostCalendarEventsDescriptor Events(params ScheduledEvent[] events) => this.Assign<ScheduledEvent[]>(events, (Action<IPostCalendarEventsRequest, ScheduledEvent[]>) ((a, v) => a.Events = (IEnumerable<ScheduledEvent>) v));
  }
}
