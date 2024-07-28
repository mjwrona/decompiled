// Decompiled with JetBrains decompiler
// Type: Nest.ScrollAllRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public class ScrollAllRequest : IScrollAllRequest, IHelperCallable
  {
    public ScrollAllRequest(Time scrollTime, int numberOfSlices)
    {
      IScrollAllRequest scrollAllRequest = (IScrollAllRequest) this;
      scrollAllRequest.ScrollTime = scrollTime;
      scrollAllRequest.Slices = numberOfSlices;
    }

    public ScrollAllRequest(Time scrollTime, int numberOfSlices, Field routingField)
      : this(scrollTime, numberOfSlices)
    {
      this.RoutingField = routingField;
    }

    public ProducerConsumerBackPressure BackPressure { get; set; }

    public int? MaxDegreeOfParallelism { get; set; }

    public Field RoutingField { get; set; }

    internal RequestMetaData ParentMetaData { get; set; }

    public ISearchRequest Search { get; set; }

    RequestMetaData IHelperCallable.ParentMetaData
    {
      get => this.ParentMetaData;
      set => this.ParentMetaData = value;
    }

    Time IScrollAllRequest.ScrollTime { get; set; }

    int IScrollAllRequest.Slices { get; set; }
  }
}
