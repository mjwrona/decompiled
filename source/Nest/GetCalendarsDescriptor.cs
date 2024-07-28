// Decompiled with JetBrains decompiler
// Type: Nest.GetCalendarsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetCalendarsDescriptor : 
    RequestDescriptorBase<GetCalendarsDescriptor, GetCalendarsRequestParameters, IGetCalendarsRequest>,
    IGetCalendarsRequest,
    IRequest<GetCalendarsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCalendars;

    public GetCalendarsDescriptor()
    {
    }

    public GetCalendarsDescriptor(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    Id IGetCalendarsRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    public GetCalendarsDescriptor CalendarId(Id calendarId) => this.Assign<Id>(calendarId, (Action<IGetCalendarsRequest, Id>) ((a, v) => a.RouteValues.Optional("calendar_id", (IUrlParameter) v)));

    IPage IGetCalendarsRequest.Page { get; set; }

    public GetCalendarsDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetCalendarsRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));
  }
}
