// Decompiled with JetBrains decompiler
// Type: Nest.LikeFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class LikeFormatter : IJsonFormatter<Like>, IJsonFormatter
  {
    private static readonly Nest.UnionFormatter<string, ILikeDocument> UnionFormatter = new Nest.UnionFormatter<string, ILikeDocument>();

    public Like Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Union<string, ILikeDocument> union = LikeFormatter.UnionFormatter.Deserialize(ref reader, formatterResolver);
      if (union == null)
        return (Like) null;
      switch (union.Tag)
      {
        case 0:
          return new Like(union.Item1);
        case 1:
          return new Like(union.Item2);
        default:
          return (Like) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      Like value,
      IJsonFormatterResolver formatterResolver)
    {
      LikeFormatter.UnionFormatter.Serialize(ref writer, (Union<string, ILikeDocument>) value, formatterResolver);
    }
  }
}
