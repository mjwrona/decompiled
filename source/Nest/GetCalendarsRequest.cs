// Decompiled with JetBrains decompiler
// Type: Nest.GetCalendarsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetCalendarsRequest : 
    PlainRequestBase<GetCalendarsRequestParameters>,
    IGetCalendarsRequest,
    IRequest<GetCalendarsRequestParameters>,
    IRequest
  {
    protected IGetCalendarsRequest Self => (IGetCalendarsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetCalendars;

    public GetCalendarsRequest()
    {
    }

    public GetCalendarsRequest(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [IgnoreDataMember]
    Id IGetCalendarsRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    public IPage Page { get; set; }
  }
}
