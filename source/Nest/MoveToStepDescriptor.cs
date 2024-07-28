// Decompiled with JetBrains decompiler
// Type: Nest.MoveToStepDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class MoveToStepDescriptor : 
    RequestDescriptorBase<MoveToStepDescriptor, MoveToStepRequestParameters, IMoveToStepRequest>,
    IMoveToStepRequest,
    IRequest<MoveToStepRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementMoveToStep;

    public MoveToStepDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected MoveToStepDescriptor()
    {
    }

    IndexName IMoveToStepRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public MoveToStepDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMoveToStepRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public MoveToStepDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IMoveToStepRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    IStepKey IMoveToStepRequest.CurrentStep { get; set; }

    IStepKey IMoveToStepRequest.NextStep { get; set; }

    public MoveToStepDescriptor CurrentStep(Func<StepKeyDescriptor, IStepKey> selector) => this.Assign<Func<StepKeyDescriptor, IStepKey>>(selector, (Action<IMoveToStepRequest, Func<StepKeyDescriptor, IStepKey>>) ((a, v) => a.CurrentStep = v != null ? v(new StepKeyDescriptor()) : (IStepKey) null));

    public MoveToStepDescriptor NextStep(Func<StepKeyDescriptor, IStepKey> selector) => this.Assign<Func<StepKeyDescriptor, IStepKey>>(selector, (Action<IMoveToStepRequest, Func<StepKeyDescriptor, IStepKey>>) ((a, v) => a.NextStep = v != null ? v(new StepKeyDescriptor()) : (IStepKey) null));
  }
}
