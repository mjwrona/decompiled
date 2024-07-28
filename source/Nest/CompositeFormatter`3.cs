// Decompiled with JetBrains decompiler
// Type: Nest.CompositeFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class CompositeFormatter<T, TRead, TWrite> : IJsonFormatter<T>, IJsonFormatter
    where TRead : IJsonFormatter<T>, new()
    where TWrite : IJsonFormatter<T>, new()
  {
    public CompositeFormatter()
    {
      this.Read = new TRead();
      this.Write = new TWrite();
    }

    private TRead Read { get; set; }

    private TWrite Write { get; set; }

    public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => this.Read.Deserialize(ref reader, formatterResolver);

    public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver) => this.Write.Serialize(ref writer, value, formatterResolver);
  }
}
