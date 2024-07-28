// Decompiled with JetBrains decompiler
// Type: Nest.PutCalendarDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutCalendarDescriptor : 
    RequestDescriptorBase<PutCalendarDescriptor, PutCalendarRequestParameters, IPutCalendarRequest>,
    IPutCalendarRequest,
    IRequest<PutCalendarRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutCalendar;

    public PutCalendarDescriptor(Id calendarId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId)))
    {
    }

    [SerializationConstructor]
    protected PutCalendarDescriptor()
    {
    }

    Id IPutCalendarRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    string IPutCalendarRequest.Description { get; set; }

    public PutCalendarDescriptor Description(string description) => this.Assign<string>(description, (Action<IPutCalendarRequest, string>) ((a, v) => a.Description = v));
  }
}
