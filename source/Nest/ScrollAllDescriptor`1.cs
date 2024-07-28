// Decompiled with JetBrains decompiler
// Type: Nest.ScrollAllDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class ScrollAllDescriptor<T> : 
    DescriptorBase<ScrollAllDescriptor<T>, IScrollAllRequest>,
    IScrollAllRequest
    where T : class
  {
    public ScrollAllDescriptor(Time scrollTime, int numberOfSlices)
    {
      this.Self.ScrollTime = scrollTime;
      this.Self.Slices = numberOfSlices;
    }

    ProducerConsumerBackPressure IScrollAllRequest.BackPressure { get; set; }

    int? IScrollAllRequest.MaxDegreeOfParallelism { get; set; }

    Field IScrollAllRequest.RoutingField { get; set; }

    Time IScrollAllRequest.ScrollTime { get; set; }

    ISearchRequest IScrollAllRequest.Search { get; set; }

    int IScrollAllRequest.Slices { get; set; }

    public ScrollAllDescriptor<T> MaxDegreeOfParallelism(int? maxDegreeOfParallelism) => this.Assign<int?>(maxDegreeOfParallelism, (Action<IScrollAllRequest, int?>) ((a, v) => a.MaxDegreeOfParallelism = v));

    public ScrollAllDescriptor<T> RoutingField(Field field) => this.Assign<Field>(field, (Action<IScrollAllRequest, Field>) ((a, v) => a.RoutingField = v));

    public ScrollAllDescriptor<T> RoutingField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IScrollAllRequest, Expression<Func<T, TValue>>>) ((a, v) => a.RoutingField = (Field) (Expression) v));

    public ScrollAllDescriptor<T> Search(Func<SearchDescriptor<T>, ISearchRequest> selector) => this.Assign<Func<SearchDescriptor<T>, ISearchRequest>>(selector, (Action<IScrollAllRequest, Func<SearchDescriptor<T>, ISearchRequest>>) ((a, v) => a.Search = v != null ? v(new SearchDescriptor<T>()) : (ISearchRequest) null));

    public ScrollAllDescriptor<T> BackPressure(int maxConcurrency, int? backPressureFactor = null) => this.Assign<ProducerConsumerBackPressure>(new ProducerConsumerBackPressure(backPressureFactor, maxConcurrency), (Action<IScrollAllRequest, ProducerConsumerBackPressure>) ((a, v) => a.BackPressure = v));
  }
}
