// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteWatchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExecuteWatchRequest : 
    PlainRequestBase<ExecuteWatchRequestParameters>,
    IExecuteWatchRequest,
    IRequest<ExecuteWatchRequestParameters>,
    IRequest
  {
    protected IExecuteWatchRequest Self => (IExecuteWatchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherExecute;

    public ExecuteWatchRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public ExecuteWatchRequest()
    {
    }

    [IgnoreDataMember]
    Id IExecuteWatchRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public bool? Debug
    {
      get => this.Q<bool?>("debug");
      set => this.Q("debug", (object) value);
    }

    public IDictionary<string, ActionExecutionMode> ActionModes { get; set; }

    public IDictionary<string, object> AlternativeInput { get; set; }

    public bool? IgnoreCondition { get; set; }

    public bool? RecordExecution { get; set; }

    public SimulatedActions SimulatedActions { get; set; }

    public IScheduleTriggerEvent TriggerData { get; set; }

    public IWatch Watch { get; set; }
  }
}
