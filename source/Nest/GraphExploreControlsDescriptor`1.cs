// Decompiled with JetBrains decompiler
// Type: Nest.GraphExploreControlsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class GraphExploreControlsDescriptor<T> : 
    DescriptorBase<GraphExploreControlsDescriptor<T>, IGraphExploreControls>,
    IGraphExploreControls
    where T : class
  {
    Nest.SampleDiversity IGraphExploreControls.SampleDiversity { get; set; }

    int? IGraphExploreControls.SampleSize { get; set; }

    Time IGraphExploreControls.Timeout { get; set; }

    bool? IGraphExploreControls.UseSignificance { get; set; }

    public GraphExploreControlsDescriptor<T> UseSignificance(bool? useSignificance = true) => this.Assign<bool?>(useSignificance, (Action<IGraphExploreControls, bool?>) ((a, v) => a.UseSignificance = v));

    public GraphExploreControlsDescriptor<T> SampleSize(int? sampleSize) => this.Assign<int?>(sampleSize, (Action<IGraphExploreControls, int?>) ((a, v) => a.SampleSize = v));

    public GraphExploreControlsDescriptor<T> Timeout(Time time) => this.Assign<Time>(time, (Action<IGraphExploreControls, Time>) ((a, v) => a.Timeout = v));

    public GraphExploreControlsDescriptor<T> SampleDiversity(Field field, int? maxDocumentsPerValue) => this.Assign<Nest.SampleDiversity>(new Nest.SampleDiversity()
    {
      Field = field,
      MaxDocumentsPerValue = maxDocumentsPerValue
    }, (Action<IGraphExploreControls, Nest.SampleDiversity>) ((a, v) => a.SampleDiversity = v));

    public GraphExploreControlsDescriptor<T> SampleDiversity<TValue>(
      Expression<Func<T, TValue>> field,
      int? maxDocumentsPerValue)
    {
      return this.Assign<Nest.SampleDiversity>(new Nest.SampleDiversity()
      {
        Field = (Field) (Expression) field,
        MaxDocumentsPerValue = maxDocumentsPerValue
      }, (Action<IGraphExploreControls, Nest.SampleDiversity>) ((a, v) => a.SampleDiversity = v));
    }
  }
}
