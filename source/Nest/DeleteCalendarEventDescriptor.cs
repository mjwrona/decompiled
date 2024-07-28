// Decompiled with JetBrains decompiler
// Type: Nest.DeleteCalendarEventDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteCalendarEventDescriptor : 
    RequestDescriptorBase<DeleteCalendarEventDescriptor, DeleteCalendarEventRequestParameters, IDeleteCalendarEventRequest>,
    IDeleteCalendarEventRequest,
    IRequest<DeleteCalendarEventRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteCalendarEvent;

    public DeleteCalendarEventDescriptor(Id calendarId, Id eventId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId).Required("event_id", (IUrlParameter) eventId)))
    {
    }

    [SerializationConstructor]
    protected DeleteCalendarEventDescriptor()
    {
    }

    Id IDeleteCalendarEventRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    Id IDeleteCalendarEventRequest.EventId => this.Self.RouteValues.Get<Id>("event_id");
  }
}
