// Decompiled with JetBrains decompiler
// Type: Nest.DeleteCalendarEventRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteCalendarEventRequest : 
    PlainRequestBase<DeleteCalendarEventRequestParameters>,
    IDeleteCalendarEventRequest,
    IRequest<DeleteCalendarEventRequestParameters>,
    IRequest
  {
    protected IDeleteCalendarEventRequest Self => (IDeleteCalendarEventRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteCalendarEvent;

    public DeleteCalendarEventRequest(Id calendarId, Id eventId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId).Required("event_id", (IUrlParameter) eventId)))
    {
    }

    [SerializationConstructor]
    protected DeleteCalendarEventRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteCalendarEventRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    [IgnoreDataMember]
    Id IDeleteCalendarEventRequest.EventId => this.Self.RouteValues.Get<Id>("event_id");
  }
}
