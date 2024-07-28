// Decompiled with JetBrains decompiler
// Type: Nest.DeleteCalendarJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteCalendarJobDescriptor : 
    RequestDescriptorBase<DeleteCalendarJobDescriptor, DeleteCalendarJobRequestParameters, IDeleteCalendarJobRequest>,
    IDeleteCalendarJobRequest,
    IRequest<DeleteCalendarJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteCalendarJob;

    public DeleteCalendarJobDescriptor(Id calendarId, Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("calendar_id", (IUrlParameter) calendarId).Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected DeleteCalendarJobDescriptor()
    {
    }

    Id IDeleteCalendarJobRequest.CalendarId => this.Self.RouteValues.Get<Id>("calendar_id");

    Id IDeleteCalendarJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");
  }
}
