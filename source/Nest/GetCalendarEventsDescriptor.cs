// Decompiled with JetBrains decompiler
// Type: Nest.GetCalendarEventsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetCalendarEventsDescriptor : 
    RequestDescriptorBase<GetCalendarEventsDescriptor, GetCalendarEventsRequestParameters, IGetCalendarEventsRequest>,
    IGetCalendarEventsRequest,
    IRequest<GetCalendarEventsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCalendarEvents;

    public GetCalendarEventsDescriptor(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [SerializationConstructor]
    protected GetCalendarEventsDescriptor()
    {
    }

    Id IGetCalendarEventsRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    public GetCalendarEventsDescriptor End(DateTimeOffset? end) => this.Qs(nameof (end), (object) end);

    public GetCalendarEventsDescriptor JobId(string jobid) => this.Qs("job_id", (object) jobid);

    public GetCalendarEventsDescriptor Start(string start) => this.Qs(nameof (start), (object) start);

    int? IGetCalendarEventsRequest.From { get; set; }

    int? IGetCalendarEventsRequest.Size { get; set; }

    public GetCalendarEventsDescriptor From(int? from) => this.Assign<int?>(from, (Action<IGetCalendarEventsRequest, int?>) ((a, v) => a.From = v));

    public GetCalendarEventsDescriptor Size(int? size) => this.Assign<int?>(size, (Action<IGetCalendarEventsRequest, int?>) ((a, v) => a.Size = v));
  }
}
