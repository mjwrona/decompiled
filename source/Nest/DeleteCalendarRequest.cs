// Decompiled with JetBrains decompiler
// Type: Nest.DeleteCalendarRequest
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
  public class DeleteCalendarRequest : 
    PlainRequestBase<DeleteCalendarRequestParameters>,
    IDeleteCalendarRequest,
    IRequest<DeleteCalendarRequestParameters>,
    IRequest
  {
    protected IDeleteCalendarRequest Self => (IDeleteCalendarRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteCalendar;

    public DeleteCalendarRequest(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [SerializationConstructor]
    protected DeleteCalendarRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteCalendarRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");
  }
}
