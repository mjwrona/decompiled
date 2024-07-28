// Decompiled with JetBrains decompiler
// Type: Nest.SimulatePipelineDocument
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SimulatePipelineDocument : ISimulatePipelineDocument
  {
    private object _source;

    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public object Source
    {
      get => this._source;
      set
      {
        this._source = value;
        IndexName indexName = this.Index;
        if ((object) indexName == null)
          indexName = (IndexName) this._source.GetType();
        this.Index = indexName;
        Id id = this.Id;
        if ((object) id == null)
          id = Id.From<object>(this._source);
        this.Id = id;
      }
    }
  }
}
