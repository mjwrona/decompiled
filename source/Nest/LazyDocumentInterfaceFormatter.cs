// Decompiled with JetBrains decompiler
// Type: Nest.LazyDocumentInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class LazyDocumentInterfaceFormatter : IJsonFormatter<ILazyDocument>, IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      ILazyDocument value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value != null)
      {
        if (value is LazyDocument lazyDocument)
        {
          JsonReader reader = new JsonReader(lazyDocument.Bytes);
          LazyDocumentFormatter.WriteUnindented(ref reader, ref writer);
        }
        else
          writer.WriteNull();
      }
      else
        writer.WriteNull();
    }

    public ILazyDocument Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNextBlock();
        return (ILazyDocument) null;
      }
      ArraySegment<byte> src = reader.ReadNextBlockSegment();
      return (ILazyDocument) new LazyDocument(BinaryUtil.ToArray(ref src), formatterResolver);
    }
  }
}
