// Decompiled with JetBrains decompiler
// Type: Nest.GetCalendarEventsRequest
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
  public class GetCalendarEventsRequest : 
    PlainRequestBase<GetCalendarEventsRequestParameters>,
    IGetCalendarEventsRequest,
    IRequest<GetCalendarEventsRequestParameters>,
    IRequest
  {
    protected IGetCalendarEventsRequest Self => (IGetCalendarEventsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCalendarEvents;

    public GetCalendarEventsRequest(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [SerializationConstructor]
    protected GetCalendarEventsRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetCalendarEventsRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    public DateTimeOffset? End
    {
      get => this.Q<DateTimeOffset?>("end");
      set => this.Q("end", (object) value);
    }

    public string JobId
    {
      get => this.Q<string>("job_id");
      set => this.Q("job_id", (object) value);
    }

    public string Start
    {
      get => this.Q<string>("start");
      set => this.Q("start", (object) value);
    }

    public int? From { get; set; }

    public int? Size { get; set; }
  }
}
