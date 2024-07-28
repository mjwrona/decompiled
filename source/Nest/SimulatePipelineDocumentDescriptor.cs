// Decompiled with JetBrains decompiler
// Type: Nest.SimulatePipelineDocumentDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SimulatePipelineDocumentDescriptor : 
    DescriptorBase<SimulatePipelineDocumentDescriptor, ISimulatePipelineDocument>,
    ISimulatePipelineDocument
  {
    Nest.Id ISimulatePipelineDocument.Id { get; set; }

    IndexName ISimulatePipelineDocument.Index { get; set; }

    object ISimulatePipelineDocument.Source { get; set; }

    public SimulatePipelineDocumentDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ISimulatePipelineDocument, Nest.Id>) ((a, v) => a.Id = v));

    public SimulatePipelineDocumentDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ISimulatePipelineDocument, IndexName>) ((a, v) => a.Index = v));

    public SimulatePipelineDocumentDescriptor Source<T>(T source) where T : class => this.Assign<T>(source, (Action<ISimulatePipelineDocument, T>) ((a, v) =>
    {
      a.Source = (object) v;
      ISimulatePipelineDocument pipelineDocument1 = a;
      IndexName indexName = a.Index;
      if ((object) indexName == null)
        indexName = (IndexName) v.GetType();
      pipelineDocument1.Index = indexName;
      ISimulatePipelineDocument pipelineDocument2 = a;
      Nest.Id id = a.Id;
      if ((object) id == null)
        id = Nest.Id.From<T>(v);
      pipelineDocument2.Id = id;
    }));
  }
}
